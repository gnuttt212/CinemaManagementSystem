namespace Cinema.Web.Services
{
    public interface IClaudeService
    {
        // Nhận prompt từ người dùng, trả về câu trả lời từ Claude
        Task<string> GetCompletionAsync(string userPrompt);
    }
}
