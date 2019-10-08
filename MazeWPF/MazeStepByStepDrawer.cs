using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MazeWPF
{
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
