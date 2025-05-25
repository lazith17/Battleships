using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.Core.Models
{
    // Represents a coordinate on the game board
    public class Coordinate
    {
        public int Row { get; }
        public int Column { get; }

        // Creates a new coordinate
        // Row index (0 to 9)
        // Column index (0 to 9)
        public Coordinate(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
