namespace DocumentSummariser.Services
{
    public interface ISummariseService
    {
        Task<string> ExtractActionsAsync(string content);
        Task<string> SummariseAsync(string content);
    }
}
