using System;
using System.Collections.Generic;

namespace MazeWPF.Models
{
    /// <summary>
    /// Maze graph-like implementation using an adjacency matrix.
    /// </summary>
    public class Maze
    {
        private readonly Cell[,] _cells;
        private readonly bool[,] _passes;

        /// <summary>
        /// Create a maze from the given dimensions, walls all around each cell.
        /// </summary>
        public Maze(int width, int height, (int x, int y) startPosition, (int x, int y) exitPosition)
        {
            var totalCells = width * height;

            _cells = new Cell[width, height];
            _passes = new bool[totalCells, totalCells];

            this.CheckIsInGrid(startPosition.x, startPosition.y);
            this.CheckIsInGrid(exitPosition.x, exitPosition.y);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var newCell = new Cell(j, i);

                    _cells[i, j] = newCell;

                    if (startPosition == (j, i))
                    {
                        this.StartCell = newCell;
                    }

                    if (exitPosition == (j, i))
                    {
                        this.ExitCell = newCell;
                    }
                }
            }
        }

        public int Height => _cells.GetLength(0);

        public int Width => _cells.GetLength(1);

        public Cell StartCell { get; }

        public Cell ExitCell { get; }

        /// <summary>
        /// Get the neighbours of the specified cell.
        /// </summary>
        public IEnumerable<Cell> GetNeighbours(Cell cell)
        {
            var neighbours = new List<Cell>();

            this.AddCellAtPositionIfExists(neighbours, cell.X - 1, cell.Y);
            this.AddCellAtPositionIfExists(neighbours, cell.X + 1, cell.Y);
            this.AddCellAtPositionIfExists(neighbours, cell.X, cell.Y - 1);
            this.AddCellAtPositionIfExists(neighbours, cell.X, cell.Y + 1);

            return neighbours;
        }

        /// <summary>
        /// Remove a wall between two neighboring cells using the adjacency matrix.
        /// </summary>
        public void RemoveWall(Cell cell, Cell neighbour)
        {
            var cellNumber = this.GetCellNumber(cell);
            var neighbourNumber = this.GetCellNumber(neighbour);

            _passes[cellNumber, neighbourNumber] = true;
            _passes[neighbourNumber, cellNumber] = true;
        }

        private int GetCellNumber(Cell cell)
        {
            return cell.Y * this.Width + cell.X;
        }

        private void AddCellAtPositionIfExists(ICollection<Cell> cells, int x, int y)
        {
            if (this.PositionIsInGrid(x, y))
            {
                cells.Add(_cells[y, x]);
            }
        }

        private void CheckIsInGrid(int x, int y)
        {
            if (!this.PositionIsInGrid(x, y))
            {
                throw new InvalidOperationException(
                    $"Cell position ({x}, {y}) is not within the maze.");
            }
        }

        private bool PositionIsInGrid(int x, int y)
        {
            return x >= 0 && x < this.Width && y >= 0 && y < this.Height;
        }
    }
}
