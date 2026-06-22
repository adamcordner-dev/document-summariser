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

        private readonly AnthropicClient _anthropicClient;

        public SummariseService(AnthropicClient anthropicClient)
        {
            _anthropicClient = anthropicClient ?? throw new ArgumentNullException(nameof(anthropicClient));
        }

        public async Task<string> ExtractActionsAsync(string content)
        {
            return await SendMessageAsync(content, ExtractActionsPrompt);
        }

        public async Task<string> SummariseAsync(string content)
        {
            return await SendMessageAsync(content, SummarisePrompt);
        }

        private async Task<string> SendMessageAsync(string content, string systemPrompt)
        {
            if (string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(systemPrompt))
            {
                return NoSummaryAvailable;
            }

            List<Message> messages = new List<Message>
            {
                new Message(RoleType.User, content)
            };

            MessageParameters messageParameters = new MessageParameters
            {
                Model = AnthropicModels.Claude46Sonnet,
                MaxTokens = 1000,
                System = new List<SystemMessage>
                {
                    new SystemMessage(systemPrompt)
                },
                Messages = messages,
                Stream = false,
                Temperature = 1.0m
            };

            MessageResponse response = await _anthropicClient.Messages.GetClaudeMessageAsync(messageParameters);

            return response.Content.OfType<TextContent>().FirstOrDefault()?.Text ?? NoSummaryAvailable;
        }
    }
}
