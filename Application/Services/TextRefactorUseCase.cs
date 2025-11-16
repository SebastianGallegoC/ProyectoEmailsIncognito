using Domain.Interfaces;

namespace Application.Services
{
    public class TextRefactorUseCase
    {
        private readonly IAIService _aiService;

        public TextRefactorUseCase(IAIService aiService)
        {
            _aiService = aiService;
        }

        public async Task<string> ExecuteAsync(string rawText)
        {
            return await _aiService.RefactorTextAsync(rawText);
        }
    }
}
