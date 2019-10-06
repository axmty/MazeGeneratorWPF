using System;
using System.Collections.Generic;
using System.Linq;

namespace MazeWPF.Models
{
    public class OldMaze
    {
        public OldMaze(int width, int height, (int x, int y) startPosition, (int x, int y) exitPosition)
        {
            this.Cells = new Cell[height * width];
            this.Width = width;
            this.Height = height;

            this.CheckIsPositionOnBorder(startPosition.x, startPosition.y);
            this.CheckIsPositionOnBorder(exitPosition.x, exitPosition.y);

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

        private void CheckIsPositionOnBorder(int x, int y)
        {
            var onBorder =
                ((x == 0 || x == this.Width - 1) && (y >= 0 && y < this.Height)) ||
                ((y == 0 || y == this.Height - 1) && (x >= 0 && x < this.Width));

            if (!onBorder)
            {
                throw new InvalidOperationException($"Cell position ({x}, {y}) is not on a border.");
            }
        }

        private bool PositionIsInGrid(int x, int y)
        {
            return x >= 0 && x < this.Width && y >= 0 && y < this.Height;
        }
    }
}
