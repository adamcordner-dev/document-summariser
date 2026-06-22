using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;

namespace DocumentSummariser.Services
{
    public class AnthropicMessageClient : IAnthropicMessageClient
    {
        private const string NoSummaryAvailable = "No summary available.";

        private readonly AnthropicClient _anthropicClient;

        public AnthropicMessageClient(AnthropicClient anthropicClient)
        {
            _anthropicClient = anthropicClient ?? throw new ArgumentNullException(nameof(anthropicClient));
        }

        public async Task<string> GetResponseAsync(string content, string systemPrompt)
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
