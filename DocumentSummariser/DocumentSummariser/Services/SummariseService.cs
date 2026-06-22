using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;

namespace DocumentSummariser.Services
{
    public class SummariseService : ISummariseService
    {
        private const string ExtractActionsPrompt = "You are a helpful assistant that extracts actionable items and decisions from documents. Return only a concise numbered list of clear action points or decisions identified in the document. If no action points are found, say so clearly.";
        private const string NoSummaryAvailable = "No summary available."; 
        private const string SummarisePrompt = "You are a helpful assistant that summarises documents clearly and concisely. Return a brief summary followed by a list of key points.";

        private readonly IAnthropicMessageClient _anthropicMessageClient;

        public SummariseService(IAnthropicMessageClient anthropicMessageClient)
        {
            _anthropicMessageClient = anthropicMessageClient ?? throw new ArgumentNullException(nameof(anthropicMessageClient));
        }

        public async Task<string> ExtractActionsAsync(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return NoSummaryAvailable;
            }

            return await _anthropicMessageClient.GetResponseAsync(content, ExtractActionsPrompt);
        }

        public async Task<string> SummariseAsync(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return NoSummaryAvailable;
            }

            return await _anthropicMessageClient.GetResponseAsync(content, SummarisePrompt);
        }
    }
}
