# -*- coding: utf-8 -*-
"""
Local AI Server for text formalization
Model: google/flan-t5-base
Endpoint: POST http://localhost:5005/generate
"""

from transformers import pipeline
from flask import Flask, request, jsonify
import logging

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

app = Flask(__name__)

logger.info("Loading model google/flan-t5-base...")
model = pipeline("text2text-generation", model="google/flan-t5-base", device=-1)
logger.info("Model loaded successfully.")

@app.route("/generate", methods=["POST"])
def generate():
    try:
        data = request.get_json()
        if not data or "inputs" not in data:
            return jsonify({"error": "Field inputs required in JSON body"}), 400
        
        text = data.get("inputs", "").strip()
        if not text:
            return jsonify({"error": "Input text is empty"}), 400
        
        prompt = "Reformulate the following text in Spanish with a formal, clear and professional tone. Respond ONLY with the reformulated text: " + text
        
        max_tokens = data.get("max_new_tokens", 200)
        temperature = data.get("temperature", 0.3)
        
        logger.info("Generating response for: %s", text[:50])
        result = model(
            prompt,
            max_new_tokens=max_tokens,
            temperature=temperature,
            do_sample=False,
            early_stopping=True
        )
        
        generated = result[0]['generated_text'].strip()
        logger.info("Generation successful: %s", generated[:50])
        
        return jsonify({"generated_text": generated}), 200
    
    except Exception as e:
        logger.error("Error in generation: %s", str(e))
        return jsonify({"error": str(e)}), 500

@app.route("/health", methods=["GET"])
def health():
    return jsonify({"status": "ok", "model": "google/flan-t5-base"}), 200

if __name__ == "__main__":
    logger.info("AI Server started at http://0.0.0.0:5005")
    app.run(host="0.0.0.0", port=5005, debug=False)
