using System.Collections.Generic;
using System.Windows.Shapes;

namespace MazeWPF.Models
{
    public class Cell
    {
        public Cell(int positionX, int positionY)
        {
            this.X = positionX;
            this.Y = positionY;
        }

        public int X { get; }

        public int Y { get; }

        public List<Wall> Walls { get; } = new List<Wall>();
    }
}
