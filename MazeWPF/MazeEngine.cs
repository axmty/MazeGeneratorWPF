using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

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

        public int Height { get; }

        public int Width { get; }

        public Node this[int x, int y] => this._nodes[y, x];

        public Node FirstNode => this[0, 0];

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

    public class MazeEngine
    {
        private readonly MazeStepByStepDrawer _drawer;
        private readonly Random _random = new Random();
        private Maze _maze;

        public MazeEngine(MazeStepByStepDrawer drawer)
        {
            _drawer = drawer;
        }

        public void GenerateRandom(int width, int height)
        {
            _maze = new Maze(width, height);
            this.InitMazeDrawing(width, height);
            
            var visitedNodes = new bool[height, width];
            var backtrackStack = new Stack<Node>();
            var currentNode = _maze.FirstNode;

            visitedNodes[0, 0] = true;
            _drawer.ChangeNodeState(currentNode.X, currentNode.Y, NodeState.Current);

            do
            {
                var unvisitedNeighbours = this.GetUnvisitedNeighbours(currentNode, visitedNodes);
                var unvisitedNeigboursCount = unvisitedNeighbours.Count();
                Node nextNode;

                if (unvisitedNeigboursCount > 0)
                {
                    backtrackStack.Push(currentNode);
                    nextNode = this.ChooseRandomNode(unvisitedNeighbours);
                    _drawer.RemoveWallBetween(currentNode.X, currentNode.Y, nextNode.X, nextNode.Y);
                    _drawer.ChangeNodesState(
                        (currentNode.X, currentNode.Y, NodeState.Visited),
                        (nextNode.X, nextNode.Y, NodeState.Current));
                    visitedNodes[nextNode.Y, nextNode.X] = true;
                    currentNode = nextNode;
                }
                else if (backtrackStack.Count > 0)
                {
                    nextNode = backtrackStack.Pop();
                    _drawer.ChangeNodesState(
                        (currentNode.X, currentNode.Y, NodeState.Backtracked),
                        (nextNode.X, nextNode.Y, NodeState.Current));
                    currentNode = nextNode;
                }
            } while (backtrackStack.Count > 0);

            _drawer.Draw();
        }

        public void Solve()
        {
            if (_maze == null)
            {
                throw new InvalidOperationException("Maze must be generated before being solved.");
            }

            throw new NotImplementedException();
        }

        private IEnumerable<Node> GetUnvisitedNeighbours(Node node, bool[,] visitedCells)
        {
            return _maze.GetNeighbours(node).Where(n => !visitedCells[n.Y, n.X]);
        }

        private Node ChooseRandomNode(IEnumerable<Node> nodes)
        {
            var randomIndex = _random.Next(0, nodes.Count());

            return nodes.ElementAt(randomIndex);
        }

        private void InitMazeDrawing(int width, int height)
        {
            _drawer.InitMazeArea(width, height);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    _drawer.InitNode(j, i);

                    if (j < width - 1)
                    {
                        _drawer.DrawWallBetween(j, i, j + 1, i);
                    }

                    if (i < height - 1)
                    {
                        _drawer.DrawWallBetween(j, i, j, i + 1);
                    }
                }
            }
        }
    }

    public enum NodeState
    {
        Current,
        Backtracked,
        Unvisited,
        Visited
    }

    public class MazeStepByStepDrawer
    {
        private static readonly Dictionary<NodeState, Brush> NodeStatesColors = new Dictionary<NodeState, Brush>
        {
            [NodeState.Backtracked] = new SolidColorBrush(Color.FromRgb(94, 226, 167)),
            [NodeState.Current] = new SolidColorBrush(Color.FromRgb(202, 250, 140)),
            [NodeState.Unvisited] = new SolidColorBrush(Color.FromRgb(45, 141, 175)),
            [NodeState.Visited] = new SolidColorBrush(Color.FromRgb(226, 166, 94))
        };

        private static readonly Brush WallColor = new SolidColorBrush(Color.FromRgb(48, 57, 80));

        private readonly Canvas _drawingArea;
        private readonly int _nodeSize;
        private readonly Dictionary<(int, int), Line> _wallsFromNodesAddition = new Dictionary<(int, int), Line>();
        private readonly Dictionary<int, Rectangle> _nodesFromHashCode = new Dictionary<int, Rectangle>();
        private readonly Queue<Action> _steps = new Queue<Action>();
        private readonly int _interval;

        public MazeStepByStepDrawer(Canvas drawingArea, int nodeSize, int interval)
        {
            _drawingArea = drawingArea;
            _nodeSize = nodeSize;
            _interval = interval;
        }

        public void InitMazeArea(int mazeWidth, int mazeHeight)
        {
            this.ClearArea();

            _drawingArea.Width = mazeWidth * _nodeSize;
            _drawingArea.Height = mazeHeight * _nodeSize;
        }

        public void Draw()
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(_interval)
            };

            timer.Tick += (object sender, EventArgs eventArgs) =>
            {
                if (_steps.TryDequeue(out var step))
                {
                    step();
                }
                else
                {
                    timer.Stop();
                }
            };
            timer.Start();
        }

        public void InitNode(int x, int y)
        {
            var node = new Rectangle
            {
                Width = _nodeSize,
                Height = _nodeSize,
                Fill = NodeStatesColors[NodeState.Unvisited]
            };

            Canvas.SetLeft(node, x * _nodeSize);
            Canvas.SetTop(node, y * _nodeSize);

            _drawingArea.Children.Add(node);
            _nodesFromHashCode.Add((x, y).GetHashCode(), node);
        }

        public void DrawWallBetween(int x1, int y1, int x2, int y2)
        {
            var isVerticalWall = y1 == y2;
            var wall = isVerticalWall
                ? this.DrawVerticalLine(Math.Max(x1, x2) * _nodeSize, y1 * _nodeSize, (y1 + 1) * _nodeSize, WallColor)
                : this.DrawHorizontalLine(Math.Max(y1, y2) * _nodeSize, x1 * _nodeSize, (x1 + 1) * _nodeSize, WallColor);

            this.SaveWall(wall, x1, y1, x2, y2);
        }

        public void RemoveWallBetween(int x1, int y1, int x2, int y2)
        {
            _steps.Enqueue(() =>
            {
                var nodesAddition = (x1 + x2, y1 + y2);

                _drawingArea.Children.Remove(_wallsFromNodesAddition[nodesAddition]);
                _wallsFromNodesAddition.Remove(nodesAddition);
            });
        }

        public void ChangeNodeState(int x, int y, NodeState newState)
        {
            _steps.Enqueue(() =>
            {
                _nodesFromHashCode[(x, y).GetHashCode()].Fill = NodeStatesColors[newState];
            });
        }

        public void ChangeNodesState(params (int x, int y, NodeState state)[] newNodesState)
        {
            _steps.Enqueue(() =>
            {
                foreach (var (x, y, state) in newNodesState)
                {
                    _nodesFromHashCode[(x, y).GetHashCode()].Fill = NodeStatesColors[state];
                }
            });
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

        private Line DrawLine(int x1, int x2, int y1, int y2, Brush color)
        {
            var line = new Line
            {
                Stroke = color,
                StrokeThickness = 2,
                X1 = x1,
                X2 = x2,
                Y1 = y1,
                Y2 = y2
            };

            _drawingArea.Children.Add(line);

            return line;
        }

        private Line DrawVerticalLine(int x, int y1, int y2, Brush color)
        {
            return this.DrawLine(x, x, y1, y2, color);
        }

        private Line DrawHorizontalLine(int y, int x1, int x2, Brush color)
        {
            return this.DrawLine(x1, x2, y, y, color);
        }

        private void ClearArea()
        {
            _drawingArea.Children.Clear();
        }
    }
}

