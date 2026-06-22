using DocumentSummariser.Services;
using Moq;

namespace DocumentSummariser.Tests
{
    public class SummariseServiceTests
    {
        private const string NoSummaryAvailable = "No summary available.";

        private readonly Mock<IAnthropicMessageClient> _mockMessageClient;
        private readonly SummariseService _summariseService;

        public SummariseServiceTests()
        {
            _mockMessageClient = new Mock<IAnthropicMessageClient>();
            _summariseService = new SummariseService(_mockMessageClient.Object);
        }

        [Fact]
        public async Task SummariseAsync_ShouldReturnSummary_WhenContentIsValid()
        {
            string content = "This is a test document.";
            string expectedSummary = "This is a summary.";

            _mockMessageClient
                .Setup(client => client.GetResponseAsync(content, It.IsAny<string>()))
                .ReturnsAsync(expectedSummary);

            string result = await _summariseService.SummariseAsync(content);

            Assert.Equal(expectedSummary, result);
        }

        [Fact]
        public async Task SummariseAsync_ShouldReturnNoSummaryAvailable_WhenContentIsEmpty()
        {
            string result = await _summariseService.SummariseAsync(string.Empty);

            Assert.Equal(NoSummaryAvailable, result);
            _mockMessageClient.Verify(client => client.GetResponseAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SummariseAsync_ShouldReturnNoSummaryAvailable_WhenContentIsWhitespace()
        {
            string result = await _summariseService.SummariseAsync("   ");

            Assert.Equal(NoSummaryAvailable, result);
            _mockMessageClient.Verify(client => client.GetResponseAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ExtractActionsAsync_ShouldReturnActions_WhenContentIsValid()
        {
            string content = "This is a test document.";
            string expectedActions = "1. Do this. 2. Do that.";

            _mockMessageClient
                .Setup(client => client.GetResponseAsync(content, It.IsAny<string>()))
                .ReturnsAsync(expectedActions);

            string result = await _summariseService.ExtractActionsAsync(content);

            Assert.Equal(expectedActions, result);
        }

        [Fact]
        public async Task ExtractActionsAsync_ShouldReturnNoSummaryAvailable_WhenContentIsEmpty()
        {
            string result = await _summariseService.ExtractActionsAsync(string.Empty);

            Assert.Equal(NoSummaryAvailable, result);
            _mockMessageClient.Verify(client => client.GetResponseAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ExtractActionsAsync_ShouldReturnNoSummaryAvailable_WhenContentIsWhitespace()
        {
            string result = await _summariseService.ExtractActionsAsync("   ");

            Assert.Equal(NoSummaryAvailable, result);
            _mockMessageClient.Verify(client => client.GetResponseAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
