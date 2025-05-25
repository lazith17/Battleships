using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.Core.Models
{
    public enum ShipClass
    {
        Battleship,
        Destroyer
    }

    public class Ship
    {
        public ShipClass Class { get; }
        public int Size { get; }
        public List<Coordinate> Positions { get; }
        public List<Coordinate> Hits { get; }
        public bool IsSunk => Hits.Count == Size;

        public Ship(ShipClass shipClass, int size)
        {
            Class = shipClass;
            Size = size;
            Positions = new List<Coordinate>();
            Hits = new List<Coordinate>();
        }

        
    }
}
