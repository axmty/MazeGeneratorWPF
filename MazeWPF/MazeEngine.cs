using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MazeWPF
{
    public class Maze
    {
        public Maze(int width, int height)
        {
            this.Cells = new Cell[height * width];
            this.Width = width;
            this.Height = height;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    this.Cells[i * width + j] = new Cell(j, i);
                }
            }

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var cell = this[j, i];
            
                    this.AddWallWithNeighbourAtPosition(cell, j - 1, i);
                    this.AddWallWithNeighbourAtPosition(cell, j + 1, i);
                    this.AddWallWithNeighbourAtPosition(cell, j, i - 1);
                    this.AddWallWithNeighbourAtPosition(cell, j, i + 1);
                }
            }

            this.StartCell = this.Cells[startPosition.y * width + startPosition.x];
            this.ExitCell = this.Cells[exitPosition.y * width + exitPosition.x];
        }

        public int Height { get; }

        public int Width { get; }

        public Cell StartCell { get; }

        public Cell ExitCell { get; }

        public Cell[] Cells { get; }

        public int CellCount => this.Cells.Length;

        public Cell this[int x, int y] => this.Cells[y * this.Width + x];

        public IEnumerable<Cell> GetNeighbours(Cell cell)
        {
            return cell.Walls.Select(w => w.Cell1 == cell ? w.Cell2 : w.Cell1);
        }

        public Wall GetWallBetween(Cell cell, Cell neighbour)
        {
            if (!this.AreNeighbours(cell, neighbour))
            {
                throw new ArgumentException("The two provided cells are not neighbours.");
            }

            return cell.Walls.First(w => w.Cell1 == neighbour || w.Cell2 == neighbour);
        }

        private bool AreNeighbours(Cell cell, Cell neighbour)
        {
            return
                ((cell.X == neighbour.X) && Math.Abs(cell.Y - neighbour.Y) == 1) ||
                ((cell.Y == neighbour.Y) && Math.Abs(cell.X - neighbour.X) == 1);
        }

        private void AddWallWithNeighbourAtPosition(Cell cell, int x, int y)
        {
            if (this.PositionIsInGrid(x, y))
            {
                var neighbour = this[x, y];
                var alreadyLinked =
                    cell.Walls.Any(w => w.Cell1 == neighbour || w.Cell2 == neighbour) ||
                    neighbour.Walls.Any(w => w.Cell1 == cell || w.Cell2 == cell);

                if (!alreadyLinked)
                {
                    var wall = new Wall { Cell1 = cell, Cell2 = neighbour };

                    cell.Walls.Add(wall);
                    neighbour.Walls.Add(wall);
                }
            }
        }

        private bool PositionIsInGrid(int x, int y)
        {
            return x >= 0 && x < this.Width && y >= 0 && y < this.Height;
        }
    }

    public class Node
    {
        public int X { get; set; }

        public int Y { get; set; }

        public List<Node> ConnectedNodes { get; } = new List<Node>();
    }

    public class MazeEngine
    {
        private readonly MazeDrawer _drawer;
        
        private Maze _maze;

        public MazeEngine(MazeDrawer drawer)
        {
            _drawer = drawer;
        }

        public Maze GenerateRandom(int width, int height)
        {
            _drawer.InitMazeArea(width, height);
            _maze = new Maze(width, height);

            var visitedCells = new bool[height, width];
            var backtrackStack = new Stack<Node>();
            var walls = this.PutWallsEverywhere();
        }

        public void Solve()
        {
            if (_maze == null)
            {
                throw new InvalidOperationException("Maze must be generated before being solved.");
            }
        }

        private void PutWallsEverywhere()
        {

        }
    }

    public class MazeDrawer
    {
        private readonly Canvas _drawingArea;
        private readonly int _nodeSize;
        private readonly Dictionary<(int, int), Line> _wallsFromNodesAddition = new Dictionary<(int, int), Line>();
        private readonly List<Rectangle> _nodes = new List<Rectangle>();

        public MazeDrawer(Canvas drawingArea, int nodeSize)
        {
            _drawingArea = drawingArea;
            _nodeSize = nodeSize;
        }

        public void InitMazeArea(int mazeWidth, int mazeHeight)
        {
            this.ClearArea();

            _drawingArea.Width = mazeWidth * _nodeSize;
            _drawingArea.Height = mazeHeight * _nodeSize;
        }

        public void DrawWallBetween(int x1, int y1, int x2, int y2)
        {
            var isVerticalWall = y1 == y2;
            var wall = isVerticalWall
                ? this.DrawVerticalLine(Math.Max(x1, x2) * _nodeSize, y1 * _nodeSize, (y1 + 1) * _nodeSize)
                : this.DrawHorizontalLine(Math.Max(y1, y2) * _nodeSize, x1 * _nodeSize, (x1 + 1) * _nodeSize);

            this.SaveWall(wall, x1, y1, x2, y2);
        }

        private void SaveWall(Line wall, int x1, int y1, int x2, int y2)
        {
            var nodesAddition = (x1 + x2, y1 + y2);

            if (_wallsFromNodesAddition.ContainsKey(nodesAddition))
            {
                throw new InvalidOperationException("There is already a wall saved for these two nodes.");
            }

            _wallsFromNodesAddition.Add(nodesAddition, wall);
        }

        private Line DrawLine(int x1, int x2, int y1, int y2)
        {
            var line = new Line
            {
                Stroke = Brushes.Black,
                X1 = x1,
                X2 = x2,
                Y1 = y1,
                Y2 = y2
            };

            _drawingArea.Children.Add(line);

            return line;
        }

        private Line DrawVerticalLine(int x, int y1, int y2)
        {
            return this.DrawLine(x, x, y1, y2);
        }

        private Line DrawHorizontalLine(int y, int x1, int x2)
        {
            return this.DrawLine(x1, x2, y, y);
        }

        private void ClearArea()
        {
            _drawingArea.Children.Clear();
        }
    }
}












