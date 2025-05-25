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
            var client = new GameClient("https://localhost:1717/api/battleships/");
            await client.RunGame();
        }
    }

    public class GameClient
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public GameClient(string baseUrl)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task RunGame()
        {
            Console.WriteLine("BATTLESHIPS GAME");
            Console.WriteLine("Enter coordinates (e.g., A5) to shoot");
            Console.WriteLine("Commands: board (show board), new (new game), exit");
            Console.WriteLine();

            await DisplayBoard();

            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine()?.Trim().ToUpper();

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

        private async Task ProcessShot(string coordinate)
        {
            try
            {
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

        private async Task DisplayBoard()
        {
            try
            {
                var board = await _client.GetStringAsync("board");
                Console.WriteLine(board);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving board: {ex.Message}");
            }
        }

        private async Task StartNewGame()
        {
            try
            {
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

    public class ShootResponse
    {
        public bool Hit { get; set; }
        public string SunkShip { get; set; }
        public bool GameOver { get; set; }
        public string Board { get; set; }
    }
}