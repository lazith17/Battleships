using Battleships.Api.Controllers;
using Battleships.Core;
using Battleships.Core.Models;
using Battleships.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Battleships.Tests
{
    // Unit tests for the BattleshipsController
    internal class BattleshipsControllerTests
    {
        private readonly Mock<IBoardService> _boardServiceMock;
        private readonly BattleshipsController _controller;

        // Constructor to initialize the mock service and controller
        public BattleshipsControllerTests()
        {
            // Initialize mock board service and controller for each test
            _boardServiceMock = new Mock<IBoardService>();
            var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<BattleshipsController>>();
            _controller = new BattleshipsController(_boardServiceMock.Object, loggerMock.Object);
        }

        // Test to ensure the board is initialized correctly
        [Fact]
        public async Task GetBoard_ReturnsCurrentBoardStatus()
        {
            // Arrange
            var expectedBoard = "Test Board";
            _boardServiceMock.Setup(x => x.GetBoardStatus())
                .Returns(expectedBoard);

            // Act
            var result = _controller.GetBoard();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedBoard, okResult.Value);
        }

        // Test to ensure shooting at a coordinate works correctly
        [Fact]
        public async Task NewGame_ResetsBoardAndReturnsOk()
        {
            // Arrange
            _boardServiceMock.Setup(x => x.GetBoardStatus())
                .Returns("Reset Board");

            // Act
            var result = _controller.NewGame();

            // Assert
            _boardServiceMock.Verify(x => x.InitializeBoard(), Times.Once);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        // Test to ensure shooting at a coordinate works correctly
        [Theory]
        [InlineData("A1", 0, 0)]
        [InlineData("J10", 9, 9)]
        [InlineData("D5", 4, 3)]
        public void ParseCoordinate_ValidInput_ReturnsCorrectIndices(string input, int expectedRow, int expectedCol)
        {
            // Act
            var (row, col) = _controller.ParseCoordinate(input);
            //var result = _controller.ParseCoordinate(input); // Now accessible

            // Assert
            Assert.Equal(expectedRow, row);
            Assert.Equal(expectedCol, col);
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("1")]
        [InlineData("K1")]
        [InlineData("A0")]
        [InlineData("A11")]
        [InlineData("A1X")]
        public void ParseCoordinate_InvalidInput_ThrowsArgumentException(string input)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _controller.ParseCoordinate(input));
        }

        // Test to ensure shooting at a coordinate returns the correct response
        [Fact]
        public void CheckForSunkShip_WhenShipSunk_ReturnsShipClass()
        {
            // Arrange
            var testShip = new Ship(ShipClass.Battleship, 5);
            testShip.Hits.Add(new Coordinate(0, 0));
            testShip.Hits.Add(new Coordinate(0, 1));
            testShip.Hits.Add(new Coordinate(0, 2));
            testShip.Hits.Add(new Coordinate(0, 3));
            testShip.Hits.Add(new Coordinate(0, 4));

            _boardServiceMock.Setup(x => x.GetShipAtCoordinate(It.IsAny<Coordinate>()))
                .Returns(testShip);

            // Act
            var result = _controller.CheckForSunkShip(0, 0);

            // Assert
            Assert.Equal(testShip.Class.ToString(), result);
        }
    }
}