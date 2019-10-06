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
            var mazeGenerator = new BacktrackingGenerationAlgoritm();
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
                
            }

            var stepByStepDrawer = new StepByStepGenerationDrawer(100, wallsToOpen, Area.Children);
            stepByStepDrawer.Draw();
        }

        
    }
}
