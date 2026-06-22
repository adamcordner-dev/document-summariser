namespace DocumentSummariser.Services
{
    public interface IAnthropicMessageClient
    {
        Task<string> GetResponseAsync(string content, string systemPrompt);
    }
}
