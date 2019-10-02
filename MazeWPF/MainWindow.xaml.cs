using MazeWPF.Algorithms;
using MazeWPF.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MazeWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            var mazeGenerator = new BacktrackingDFSMazeGenerator();
            var maze1 = new Maze(10, 10, (3, 0), (7, 0));
            var maze2 = mazeGenerator.Generate(20, 20, (3, 0), (14, 0));

            this.DrawMaze(maze2);
        }

        private void DrawMaze(Maze maze)
        {
            var drawnWalls = new HashSet<Wall>();

            var rect = new Rectangle();
            rect.Fill = Brushes.Red;
            rect.Width = 30;
            rect.Height = 30;

            Canvas.SetLeft(rect, maze.StartCell.X * 30);
            Canvas.SetTop(rect, maze.StartCell.Y * 30);

            var rect2 = new Rectangle();
            rect2.Fill = Brushes.Green;
            rect2.Width = 30;
            rect2.Height = 30;

            Canvas.SetLeft(rect2, maze.ExitCell.X * 30);
            Canvas.SetTop(rect2, maze.ExitCell.Y * 30);

            Area.Children.Add(rect);
            Area.Children.Add(rect2);

            foreach (var cell in maze.Cells)
            {
                foreach (var wall in cell.Walls)
                {
                    if (!drawnWalls.Contains(wall) && !wall.Opened)
                    {
                        var neighbour = wall.Cell1 == cell ? wall.Cell2 : wall.Cell1;
                        var line = new Line();
                        var isVerticalLine = cell.X != neighbour.X;

                        line.Stroke = Brushes.Black;
                        line.X1 = Math.Max(cell.X, neighbour.X) * 30;
                        line.Y1 = Math.Max(cell.Y, neighbour.Y) * 30;

                        if (isVerticalLine)
                        {
                            line.X2 = line.X1;
                            line.Y2 = line.Y1 + 30;
                        }
                        else
                        {
                            line.X2 = line.X1 + 30;
                            line.Y2 = line.Y1;
                        }

                        Area.Children.Add(line);
                    }
                }
            }
        }
    }
}
