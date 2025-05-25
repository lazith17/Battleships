using Battleships.Core;
using Battleships.Core.Models;
using Battleships.Tests;

namespace Battleships.Tests
{
    // Unit tests for the BoardService class
    public class BoardServiceTests
    {
        private readonly BoardService _service;

        public BoardServiceTests()
        {
            _service = new BoardService();
        }

        // Test to ensure the board is initialized correctly
        [Fact]
        public void InitializeBoard_PlacesCorrectShips()
        {
            Assert.Equal(3, _service.Ships.Count());
            Assert.Single(_service.Ships.Where(s => s.Class == ShipClass.Battleship));
            Assert.Equal(2, _service.Ships.Where(s => s.Class == ShipClass.Destroyer).Count());
        }

        // Test to ensure the board status is generated correctly
        [Fact]
        public void ShootAt_Miss_ReturnsFalse()
        {
            // Find an empty coordinate
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    var coordinate = new Coordinate(row, col);
                    if (_service.GetShipAtCoordinate(coordinate) == null)
                    {
                        Assert.False(_service.ShootAt(coordinate));
                        return;
                    }
                }
            }
            Assert.True(false, "Couldn't find empty coordinate");
        }

        // Test to ensure the board status is generated correctly
        [Fact]
        public void AllShipsSunk_InitiallyFalse()
        {
            Assert.False(_service.AllShipsSunk());
        }
    }
}