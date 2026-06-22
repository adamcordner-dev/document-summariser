using DocumentSummariser.Models;
using DocumentSummariser.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocumentSummariser.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SummariseController : ControllerBase
{
    private const string ContentCannotBeEmpty = "Content cannot be empty.";

    private readonly ISummariseService _summariseService;

    public SummariseController(ISummariseService summariseService)
    {
        _summariseService = summariseService ?? throw new ArgumentNullException(nameof(summariseService));
    }

    [HttpPost("actions")]
    public async Task<ActionResult<SummariseResponse>> ExtractActions([FromBody] SummariseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest(ContentCannotBeEmpty);
        }

        string actions = await _summariseService.ExtractActionsAsync(request.Content);

        return Ok(new SummariseResponse { Summary = actions });
    }

    [HttpPost]
    public async Task<ActionResult<SummariseResponse>> Summarise([FromBody] SummariseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest(ContentCannotBeEmpty);
        }

        string summary = await _summariseService.SummariseAsync(request.Content);

        return Ok(new SummariseResponse { Summary = summary });
    }
}