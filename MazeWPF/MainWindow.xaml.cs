using MazeWPF.Algorithms;
using MazeWPF.Draw;
using MazeWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MazeWPF
{
    public partial class MainWindow : Window
    {
        private static readonly int CellSize = 30;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            var mazeGenerator = new BacktrackingDSFAlgorithm();
            var maze2 = mazeGenerator.Generate(20, 20, (3, 0), (19, 16));

            this.DrawMaze(maze2, mazeGenerator.WallsToOpen);
        }

        private void DrawMaze(Maze maze, IEnumerable<Wall> wallsToOpen)
        {
            var drawnWalls = new HashSet<Wall>();

            Area.Width = maze.Width * CellSize;
            Area.Height = maze.Height * CellSize;

            var rect = new Rectangle
            {
                Fill = Brushes.Red,
                Width = 30,
                Height = 30
            };

            Canvas.SetLeft(rect, maze.StartCell.X * CellSize);
            Canvas.SetTop(rect, maze.StartCell.Y * CellSize);

            var rect2 = new Rectangle
            {
                Fill = Brushes.Green,
                Width = 30,
                Height = 30
            };

            Canvas.SetLeft(rect2, maze.ExitCell.X * CellSize);
            Canvas.SetTop(rect2, maze.ExitCell.Y * CellSize);

            Area.Children.Add(rect);
            Area.Children.Add(rect2);

            foreach (var wall in maze.Cells.SelectMany(c => c.Walls))
            {
                if (wall.Shape != null)
                {
                    continue;
                }

                var cell = wall.Cell1;
                var otherCell = wall.Cell2;
                var isVerticalWall = cell.Y == otherCell.Y;
                var border = isVerticalWall
                    ? this.DrawVerticalLine(Math.Max(cell.X, otherCell.X) * CellSize, cell.Y * CellSize, (cell.Y + 1) * CellSize)
                    : this.DrawHorizontalLine(Math.Max(cell.Y, otherCell.Y) * CellSize, cell.X * CellSize, (cell.X + 1) * CellSize);

                wall.Shape = border;
            }

            var stepByStepDrawer = new StepByStepGenerationDrawer(100, wallsToOpen, Area.Children);
            stepByStepDrawer.Draw();
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

            Area.Children.Add(line);

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
    }
}
