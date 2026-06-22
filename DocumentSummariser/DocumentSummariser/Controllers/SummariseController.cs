using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using DocumentSummariser.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocumentSummariser.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SummariseController : ControllerBase
{
    private readonly AnthropicClient _anthropicClient;

    public SummariseController(AnthropicClient anthropicClient)
    {
        _anthropicClient = anthropicClient;
    }

    [HttpPost]
    public async Task<ActionResult<SummariseResponse>> Summarise([FromBody] SummariseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest("Content cannot be empty.");
        }

        List<Message> messages = new List<Message>
        {
            new Message(RoleType.User, request.Content)
        };

        MessageParameters messageParameters = new MessageParameters
        {
            Model = AnthropicModels.Claude46Sonnet,
            MaxTokens = 1000,
            System = new List<SystemMessage>
            {
                new SystemMessage("You are a helpful assistant that summarises documents clearly and concisely. Return a brief summary followed by a list of key points.")
            },
            Messages = messages,
            Stream = false,
            Temperature = 1.0m
        };

        MessageResponse response = await _anthropicClient.Messages.GetClaudeMessageAsync(messageParameters);

        string summary = response.Content.OfType<TextContent>().FirstOrDefault()?.Text ?? "No summary available.";

        return Ok(new SummariseResponse { Summary = summary });
    }
}