using System.Collections.Generic;

namespace MazeWPF
{
    public class Node
    {
        public Node(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; }

        public int Y { get; }

        public List<Node> ConnectedNodes { get; } = new List<Node>();
    }
}
