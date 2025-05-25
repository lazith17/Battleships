using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Battleships.ConsoleApp
{
    class Program
    {

        static async Task Main(string[] args)
        {
            // Initialize the game client with the API base URL
            var client = new GameClient("https://localhost:1717/api/battleships/");
            await client.RunGame();
        }
    }

    public class GameClient
    {
        // Client for interacting with the Battleships API
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;


        // Constructor initializes the HttpClient with the base URL of the API
        public GameClient(string baseUrl)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Main method to run the Battleships game
        public async Task RunGame()
        {
            // Display game instructions and initial board
            Console.WriteLine("BATTLESHIPS GAME");
            Console.WriteLine("Enter coordinates (e.g., A5) to shoot");
            Console.WriteLine("Commands: board (show board), new (new game), exit");
            Console.WriteLine();

            await DisplayBoard();

            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine()?.Trim().ToUpper();

                // Handle user input commands
                switch (input)
                {
                    case "EXIT":
                        return;
                    case "BOARD":
                        await DisplayBoard();
                        break;
                    case "NEW":
                        await StartNewGame();
                        break;
                    default:
                        if (!string.IsNullOrEmpty(input))
                        {
                            await ProcessShot(input);
                        }
                        break;
                }
            }
        }

        // Processes the shot at the given coordinate
        private async Task ProcessShot(string coordinate)
        {
            try
            {
                // Validate the coordinate format (e.g., A5, B10)
                var response = await _client.PostAsJsonAsync("shoot", coordinate);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {error}");
                    return;
                }

                var result = await response.Content.ReadFromJsonAsync<ShootResponse>(_jsonOptions);

                Console.WriteLine(result.Hit ? "HIT!" : "MISS");

                if (!string.IsNullOrEmpty(result.SunkShip))
                {
                    Console.WriteLine($"You sunk the {result.SunkShip}!");
                }

                Console.WriteLine(result.Board);

                if (result.GameOver)
                {
                    // Game is over, all ships sunk
                    Console.WriteLine("CONGRATULATIONS! You won!");
                    Console.WriteLine("Starting new game...");
                    await StartNewGame();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Displays the current game board
        private async Task DisplayBoard()
        {
            try
            {
                // Get the current board status from the API
                var board = await _client.GetStringAsync("board");
                Console.WriteLine(board);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving board: {ex.Message}");
            }
        }

        // Starts a new game by calling the API endpoint
        private async Task StartNewGame()
        {
            try
            {
                // Reset the board by calling the new game endpoint
                await _client.PostAsync("new", null);
                Console.WriteLine("New game started");
                await DisplayBoard();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting new game: {ex.Message}");
            }
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