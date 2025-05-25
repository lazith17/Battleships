using Battleships.Core.Models;
using Battleships.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Battleships.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BattleshipsController : ControllerBase
    {
        private readonly IBoardService _boardService;
        private readonly ILogger<BattleshipsController> _logger;

        public BattleshipsController(IBoardService boardService, ILogger<BattleshipsController> logger)
        {
            _boardService = boardService;
            _logger = logger;
            _boardService.InitializeBoard();
        }

        [HttpPost("shoot")]
        public ActionResult<ShootResponse> Shoot([FromBody] string coordinate)
        {
            try
            {
                var (row, col) = ParseCoordinate(coordinate);
                var hit = _boardService.ShootAt(new Coordinate(row, col));

                var response = new ShootResponse
                {
                    Hit = hit,
                    SunkShip = hit ? CheckForSunkShip(row, col) : null,
                    GameOver = _boardService.AllShipsSunk(),
                    Board = _boardService.GetBoardStatus()
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("board")]
        public IActionResult GetBoard()
        {
            return Ok(_boardService.GetBoardStatus());
        }

        [HttpPost("new")]
        public IActionResult NewGame()
        {
            _boardService.InitializeBoard();
            return Ok(new { Message = "New game started", Board = _boardService.GetBoardStatus() });
        }

        internal (int row, int col) ParseCoordinate(string coordinate)
        {
            if (string.IsNullOrWhiteSpace(coordinate))
                throw new ArgumentException("Coordinate cannot be empty");

            if (coordinate.Length < 2)
                throw new ArgumentException("Coordinate must be at least 2 characters");

            var colChar = char.ToUpper(coordinate[0]);
            if (colChar < 'A' || colChar > 'J')
                throw new ArgumentException("Column must be between A and J");

            if (!int.TryParse(coordinate.Substring(1), out var row) || row < 1 || row > 10)
                throw new ArgumentException("Row must be between 1 and 10");

            return (row - 1, colChar - 'A');
        }

        internal string CheckForSunkShip(int row, int col)
        {
            var ship = _boardService.GetShipAtCoordinate(new Coordinate(row, col));
            return ship?.IsSunk == true ? ship.Class.ToString() : null;
        }
    }

    public class ShootResponse
    {
        public bool Hit { get; set; }
        public string SunkShip { get; set; }
        public bool GameOver { get; set; }
        public string Board { get; set; }
    }
}