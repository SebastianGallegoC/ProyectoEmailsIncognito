using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAIService
    {
        Task<string> RefactorTextAsync(string text);
    }
}
