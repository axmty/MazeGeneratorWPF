using System.Collections.Generic;
using System.Linq;

namespace MazeWPF
{
    public class Maze
    {
        private readonly Node[,] _nodes;

        public Maze(int width, int height)
        {
            _nodes = new Node[height, width];

            this.Width = width;
            this.Height = height;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    this._nodes[i, j] = new Node(j, i);
                }
            }
        }

        public Node FirstNode => this[0, 0];

        public int Height { get; }

        public int Width { get; }

        public Node this[int x, int y] => this._nodes[y, x];

        public IEnumerable<Node> GetNeighbours(Node node)
        {
            var surroundingPositions = new (int X, int Y)[]
            {
                (node.X - 1, node.Y),
                (node.X + 1, node.Y),
                (node.X, node.Y - 1),
                (node.X, node.Y + 1)
            };

            return surroundingPositions
                .Where(pos => this.PositionIsInGrid(pos.X, pos.Y))
                .Select(pos => this[pos.X, pos.Y]);
        }

        private bool PositionIsInGrid(int x, int y)
        {
            return x >= 0 && x < this.Width && y >= 0 && y < this.Height;
        }
    }
}