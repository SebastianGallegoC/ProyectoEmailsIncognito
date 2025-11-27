# -*- coding: utf-8 -*-
"""
Unit tests for local_ai_server.py
Tests validate Flask routes, HTTP methods, request/response formats
"""

import pytest
import json
from unittest.mock import patch, MagicMock
import sys
import os

# Add parent directory to path to import local_ai_server
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

# Mock transformers and Flask before importing local_ai_server
sys.modules['transformers'] = MagicMock()

# Import after mocking
from local_ai_server import app


@pytest.fixture
def client():
    """Create a test client for the Flask app"""
    app.config['TESTING'] = True
    with app.test_client() as client:
        yield client


@pytest.fixture
def mock_model_response():
    """Mock the model pipeline response"""
    with patch('local_ai_server.model') as mock_model:
        mock_model.return_value = [{'generated_text': 'Este es un texto formalizado de prueba.'}]
        yield mock_model


class TestHealthEndpoint:
    """Test suite for /health endpoint"""
    
    def test_health_endpoint_exists(self, client):
        """Verify /health endpoint exists and responds"""
        response = client.get('/health')
        assert response.status_code == 200
    
    def test_health_endpoint_correct_method(self, client):
        """Verify /health only accepts GET method"""
        response = client.get('/health')
        assert response.status_code == 200
        
        # POST should not be allowed
        response = client.post('/health')
        assert response.status_code == 405  # Method Not Allowed
    
    def test_health_endpoint_response_format(self, client):
        """Verify /health returns correct JSON format"""
        response = client.get('/health')
        data = json.loads(response.data)
        
        assert 'status' in data
        assert 'model' in data
        assert data['status'] == 'ok'
        assert data['model'] == 'google/flan-t5-base'
    
    def test_health_endpoint_content_type(self, client):
        """Verify /health returns JSON content type"""
        response = client.get('/health')
        assert response.content_type == 'application/json'


class TestGenerateEndpoint:
    """Test suite for /generate endpoint"""
    
    def test_generate_endpoint_exists(self, client, mock_model_response):
        """Verify /generate endpoint exists"""
        response = client.post('/generate', 
                               json={'inputs': 'hola como estas'},
                               content_type='application/json')
        assert response.status_code in [200, 400, 500]  # Exists, may fail validation
    
    def test_generate_endpoint_correct_method(self, client):
        """Verify /generate only accepts POST method"""
        # GET should not be allowed
        response = client.get('/generate')
        assert response.status_code == 405  # Method Not Allowed
        
        # PUT should not be allowed
        response = client.put('/generate')
        assert response.status_code == 405
        
        # DELETE should not be allowed
        response = client.delete('/generate')
        assert response.status_code == 405
    
    def test_generate_requires_json_body(self, client):
        """Verify /generate requires JSON body"""
        response = client.post('/generate')
        # May return 400 or 500 depending on how Flask handles missing content-type
        assert response.status_code in [400, 500]
        
        data = json.loads(response.data)
        assert 'error' in data
    
    def test_generate_requires_inputs_field(self, client):
        """Verify /generate requires 'inputs' field in JSON"""
        response = client.post('/generate', 
                               json={'wrong_field': 'test'},
                               content_type='application/json')
        assert response.status_code == 400
        
        data = json.loads(response.data)
        assert 'error' in data
        assert 'inputs' in data['error'].lower()
    
    def test_generate_rejects_empty_input(self, client):
        """Verify /generate rejects empty input text"""
        response = client.post('/generate', 
                               json={'inputs': ''},
                               content_type='application/json')
        assert response.status_code == 400
        
        data = json.loads(response.data)
        assert 'error' in data
        assert 'empty' in data['error'].lower()
    
    def test_generate_rejects_whitespace_only(self, client):
        """Verify /generate rejects whitespace-only input"""
        response = client.post('/generate', 
                               json={'inputs': '   '},
                               content_type='application/json')
        assert response.status_code == 400
        
        data = json.loads(response.data)
        assert 'error' in data
    
    def test_generate_successful_response(self, client, mock_model_response):
        """Verify /generate returns correct response format on success"""
        response = client.post('/generate', 
                               json={'inputs': 'hola como estas'},
                               content_type='application/json')
        assert response.status_code == 200
        
        data = json.loads(response.data)
        assert 'generated_text' in data
        assert isinstance(data['generated_text'], str)
        assert len(data['generated_text']) > 0
    
    def test_generate_with_optional_parameters(self, client, mock_model_response):
        """Verify /generate accepts optional parameters"""
        response = client.post('/generate', 
                               json={
                                   'inputs': 'test text',
                                   'max_new_tokens': 150,
                                   'temperature': 0.5
                               },
                               content_type='application/json')
        assert response.status_code == 200
        
        # Verify model was called with custom parameters
        mock_model_response.assert_called_once()
        call_kwargs = mock_model_response.call_args[1]
        assert call_kwargs['max_new_tokens'] == 150
        assert call_kwargs['temperature'] == 0.5
    
    def test_generate_default_parameters(self, client, mock_model_response):
        """Verify /generate uses default parameters when not provided"""
        response = client.post('/generate', 
                               json={'inputs': 'test text'},
                               content_type='application/json')
        assert response.status_code == 200
        
        # Verify model was called with default parameters
        call_kwargs = mock_model_response.call_args[1]
        assert call_kwargs['max_new_tokens'] == 200
        assert call_kwargs['temperature'] == 0.3
    
    def test_generate_content_type(self, client, mock_model_response):
        """Verify /generate returns JSON content type"""
        response = client.post('/generate', 
                               json={'inputs': 'test'},
                               content_type='application/json')
        assert response.content_type == 'application/json'
    
    def test_generate_handles_model_exception(self, client):
        """Verify /generate handles model errors gracefully"""
        with patch('local_ai_server.model') as mock_model:
            mock_model.side_effect = Exception('Model error')
            
            response = client.post('/generate', 
                                   json={'inputs': 'test text'},
                                   content_type='application/json')
            assert response.status_code == 500
            
            data = json.loads(response.data)
            assert 'error' in data


class TestRouteURLs:
    """Test suite to validate exact route URLs"""
    
    def test_health_route_exact_path(self, client):
        """Verify /health route is exactly at /health"""
        # Correct path
        assert client.get('/health').status_code == 200
        
        # Wrong paths should fail
        assert client.get('/Health').status_code == 404
        assert client.get('/HEALTH').status_code == 404
        assert client.get('/health/').status_code in [200, 404, 308]  # May redirect
        assert client.get('/healths').status_code == 404
        assert client.get('/api/health').status_code == 404
    
    def test_generate_route_exact_path(self, client, mock_model_response):
        """Verify /generate route is exactly at /generate"""
        # Correct path
        response = client.post('/generate', 
                               json={'inputs': 'test'},
                               content_type='application/json')
        assert response.status_code == 200
        
        # Wrong paths should fail
        response = client.post('/Generate', 
                               json={'inputs': 'test'},
                               content_type='application/json')
        assert response.status_code == 404
        
        response = client.post('/GENERATE', 
                               json={'inputs': 'test'},
                               content_type='application/json')
        assert response.status_code == 404
        
        response = client.post('/generates', 
                               json={'inputs': 'test'},
                               content_type='application/json')
        assert response.status_code == 404
        
        response = client.post('/api/generate', 
                               json={'inputs': 'test'},
                               content_type='application/json')
        assert response.status_code == 404


class TestHTTPMethods:
    """Test suite to validate HTTP methods are strictly enforced"""
    
    def test_health_only_get(self, client):
        """Verify /health only accepts GET"""
        assert client.get('/health').status_code == 200
        assert client.post('/health').status_code == 405
        assert client.put('/health').status_code == 405
        assert client.delete('/health').status_code == 405
        assert client.patch('/health').status_code == 405
    
    def test_generate_only_post(self, client):
        """Verify /generate only accepts POST"""
        valid_data = json.dumps({'inputs': 'test'})
        headers = {'Content-Type': 'application/json'}
        
        assert client.get('/generate').status_code == 405
        assert client.put('/generate', data=valid_data, headers=headers).status_code == 405
        assert client.delete('/generate').status_code == 405
        assert client.patch('/generate', data=valid_data, headers=headers).status_code == 405


if __name__ == '__main__':
    pytest.main([__file__, '-v'])
