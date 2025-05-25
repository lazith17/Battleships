using Battleships.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.Core.Services
{
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
