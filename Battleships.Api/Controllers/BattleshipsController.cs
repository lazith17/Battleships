using Battleships.Core.Models;
using Battleships.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Battleships.Api.Controllers
{
    // Controller for handling battleship game requests.
    [Route("api/[controller]")]
    [ApiController]
    public class BattleshipsController : ControllerBase
    {
        // Fields for the board service and logger
        private readonly IBoardService _boardService;
        private readonly ILogger<BattleshipsController> _logger;

        // Dependency injection of the board service and logger.
        public BattleshipsController(IBoardService boardService, ILogger<BattleshipsController> logger)
        {
            _boardService = boardService;
            _logger = logger;
            _boardService.InitializeBoard();
        }

        // Endpoint to shoot at a coordinate
        [HttpPost("shoot")]
        public ActionResult<ShootResponse> Shoot([FromBody] string coordinate)
        {
            try
            {
                // Validate the coordinate format
                var (row, col) = ParseCoordinate(coordinate);
                var hit = _boardService.ShootAt(new Coordinate(row, col));

                // Log the shoot action
                var response = new ShootResponse
                {
                    Hit = hit,
                    SunkShip = hit ? CheckForSunkShip(row, col) : null,
                    GameOver = _boardService.AllShipsSunk(),
                    Board = _boardService.GetBoardStatus()
                };

                return Ok(response);
            }
            // Handle any argument exceptions from coordinate parsing
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Endpoint to get the current board status
        [HttpGet("board")]
        public IActionResult GetBoard()
        {
            return Ok(_boardService.GetBoardStatus());
        }

        // Endpoint to start a new game
        [HttpPost("new")]
        public IActionResult NewGame()
        {
            _boardService.InitializeBoard();
            return Ok(new { Message = "New game started", Board = _boardService.GetBoardStatus() });
        }

        // Endpoint to get the list of ships
        internal (int row, int col) ParseCoordinate(string coordinate)
        {
            // Validate and parse the coordinate string (e.g., "A1", "B2")
            if (string.IsNullOrWhiteSpace(coordinate))
                throw new ArgumentException("Coordinate cannot be empty");

            // Ensure the coordinate is in the correct format
            if (coordinate.Length < 2)
                throw new ArgumentException("Coordinate must be at least 2 characters");

            // 
            var colChar = char.ToUpper(coordinate[0]);
            if (colChar < 'A' || colChar > 'J')
                throw new ArgumentException("Column must be between A and J");

            // Parse the row part of the coordinate
            if (!int.TryParse(coordinate.Substring(1), out var row) || row < 1 || row > 10)
                throw new ArgumentException("Row must be between 1 and 10");

            return (row - 1, colChar - 'A');
        }

        // Internal method to check if a ship is sunk at the given coordinate
        internal string CheckForSunkShip(int row, int col)
        {
            // Check if the ship at the given coordinate is sunk
            var ship = _boardService.GetShipAtCoordinate(new Coordinate(row, col));
            return ship?.IsSunk == true ? ship.Class.ToString() : null;
        }
    }

    // Response model for the shoot endpoint
    public class ShootResponse
    {
        public bool Hit { get; set; }
        public string SunkShip { get; set; }
        public bool GameOver { get; set; }
        public string Board { get; set; }
    }
}