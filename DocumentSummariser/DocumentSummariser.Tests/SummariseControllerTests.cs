using DocumentSummariser.Controllers;
using DocumentSummariser.Models;
using DocumentSummariser.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DocumentSummariser.Tests
{
    public class SummariseControllerTests
    {
        private readonly Mock<ISummariseService> _mockSummariseService;
        private readonly SummariseController _summariseController;
        
        public SummariseControllerTests()
        {
            _mockSummariseService = new Mock<ISummariseService>();
            _summariseController = new SummariseController(_mockSummariseService.Object);
        }

        [Fact]
        public async Task Summarise_ShouldReturnOk_WhenContentIsValid()
        {
            string content = "This is a test document.";
            string expectedSummary = "This is a summary.";

            _mockSummariseService
                .Setup(summariseService => summariseService.SummariseAsync(content))
                .ReturnsAsync(expectedSummary);

            ActionResult<SummariseResponse> result = await _summariseController.Summarise(new SummariseRequest { Content = content });

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            SummariseResponse response = Assert.IsType<SummariseResponse>(okResult.Value);
            Assert.Equal(expectedSummary, response.Summary);
        }

        [Fact]
        public async Task Summarise_ShouldReturnBadRequest_WhenContentIsEmpty()
        {
            ActionResult<SummariseResponse> result = await _summariseController.Summarise(new SummariseRequest { Content = string.Empty });

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Summarise_ShouldReturnBadRequest_WhenContentIsWhitespace()
        {
            string whitespaceContent = "   ";

            ActionResult<SummariseResponse> result = await _summariseController.Summarise(new SummariseRequest { Content = whitespaceContent });

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }


        [Fact]
        public async Task ExtractActions_ShouldReturnOk_WhenContentIsValid()
        {
            string content = "This is a test document.";
            string expectedActions = "1. Do this. 2. Do that.";

            _mockSummariseService
                .Setup(summariseService => summariseService.ExtractActionsAsync(content))
                .ReturnsAsync(expectedActions);

            ActionResult<SummariseResponse> result = await _summariseController.ExtractActions(new SummariseRequest { Content = content });

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            SummariseResponse response = Assert.IsType<SummariseResponse>(okResult.Value);
            Assert.Equal(expectedActions, response.Summary);
        }

        [Fact]
        public async Task ExtractActions_ShouldReturnBadRequest_WhenContentIsEmpty()
        {
            ActionResult<SummariseResponse> result = await _summariseController.ExtractActions(new SummariseRequest { Content = string.Empty });

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ExtractActions_ShouldReturnBadRequest_WhenContentIsWhitespace()
        {
            string whitespaceContent = "   ";

            ActionResult<SummariseResponse> result = await _summariseController.ExtractActions(new SummariseRequest { Content = whitespaceContent });

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}
