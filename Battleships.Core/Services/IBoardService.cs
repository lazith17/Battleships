using Battleships.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.Core.Services
{
    // Interface for the board service that manages the game board and ship placements
    public interface IBoardService
    {
        void InitializeBoard();
        bool ShootAt(Coordinate coordinate);
        bool AllShipsSunk();
        string GetBoardStatus();
        Ship GetShipAtCoordinate(Coordinate coordinate);
        IEnumerable<Ship> Ships { get; }
    }
}
