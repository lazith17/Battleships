using Battleships.Core.Models;
using Battleships.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleships.Core
{
    public class BoardService : IBoardService
    {
        private const int BoardSize = 10;
        private readonly List<Ship> _ships;
        private readonly List<Coordinate> _misses;
        private readonly Random _random;

        public IEnumerable<Ship> Ships => _ships.AsReadOnly();

        public BoardService()
        {
            _ships = new List<Ship>();
            _misses = new List<Coordinate>();
            _random = new Random();
            InitializeBoard();
        }

        public void InitializeBoard()
        {
            _ships.Clear();
            _misses.Clear();

            PlaceShip(new Ship(ShipClass.Battleship, 5));
            PlaceShip(new Ship(ShipClass.Destroyer, 4));
            PlaceShip(new Ship(ShipClass.Destroyer, 4));
        }

        public bool ShootAt(Coordinate coordinate)
        {
            foreach (var ship in _ships)
            {
                var position = ship.Positions.FirstOrDefault(p => p.Row == coordinate.Row && p.Column == coordinate.Column);
                if (position != null)
                {
                    if (!ship.Hits.Contains(position))
                    {
                        ship.Hits.Add(position);
                        return true;
                    }
                    return false;
                }
            }

            if (!_misses.Contains(coordinate))
            {
                _misses.Add(coordinate);
            }
            return false;
        }

        public bool AllShipsSunk() => _ships.All(ship => ship.IsSunk);

        public string GetBoardStatus()
        {
            var status = new System.Text.StringBuilder();
            status.AppendLine("   A B C D E F G H I J");

            for (int row = 0; row < BoardSize; row++)
            {
                status.Append($"{row + 1,2} ");
                for (int col = 0; col < BoardSize; col++)
                {
                    var coordinate = new Coordinate(row, col);
                    var ship = GetShipAtCoordinate(coordinate);

                    if (ship != null && ship.Hits.Any(h => h.Row == row && h.Column == col))
                    {
                        status.Append("X ");
                    }
                    else if (_misses.Any(m => m.Row == row && m.Column == col))
                    {
                        status.Append("O ");
                    }
                    else if (ship != null)
                    {
                        status.Append("S ");
                    }
                    else
                    {
                        status.Append(". ");
                    }
                }
                status.AppendLine();
            }

            return status.ToString();
        }

        public Ship GetShipAtCoordinate(Coordinate coordinate)
        {
            return _ships.FirstOrDefault(ship =>
                ship.Positions.Any(p => p.Row == coordinate.Row && p.Column == coordinate.Column));
        }

        private void PlaceShip(Ship ship)
        {
            bool placed = false;
            int attempts = 0;
            const int maxAttempts = 100;

            while (!placed && attempts++ < maxAttempts)
            {
                var direction = _random.Next(2);
                var startRow = _random.Next(BoardSize);
                var startCol = _random.Next(BoardSize);

                if (direction == 0 && startCol + ship.Size <= BoardSize)
                {
                    if (CanPlaceShip(startRow, startCol, ship.Size, true))
                    {
                        for (int i = 0; i < ship.Size; i++)
                        {
                            ship.Positions.Add(new Coordinate(startRow, startCol + i));
                        }
                        placed = true;
                    }
                }
                else if (direction == 1 && startRow + ship.Size <= BoardSize)
                {
                    if (CanPlaceShip(startRow, startCol, ship.Size, false))
                    {
                        for (int i = 0; i < ship.Size; i++)
                        {
                            ship.Positions.Add(new Coordinate(startRow + i, startCol));
                        }
                        placed = true;
                    }
                }
            }

            if (placed)
            {
                _ships.Add(ship);
            }
            else
            {
                throw new InvalidOperationException($"Failed to place {ship.Class} after {maxAttempts} attempts");
            }
        }

        private bool CanPlaceShip(int startRow, int startCol, int size, bool isHorizontal)
        {
            for (int i = 0; i < size; i++)
            {
                var row = isHorizontal ? startRow : startRow + i;
                var col = isHorizontal ? startCol + i : startCol;

                if (row >= BoardSize || col >= BoardSize || GetShipAtCoordinate(new Coordinate(row, col)) != null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}