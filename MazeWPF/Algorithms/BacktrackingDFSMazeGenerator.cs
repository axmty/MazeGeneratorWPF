using MazeWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MazeWPF.Algorithms
{
    public class BacktrackingDFSMazeGenerator : IMazeGenerator
    {
        private readonly Random _random = new Random();

        public Maze Generate(int width, int height, (int x, int y) startPosition, (int x, int y) exitPosition)
        {
            var maze = new Maze(width, height, startPosition, exitPosition);
            var visitedCells = new bool[maze.Height, maze.Width];
            var numberVisited = 1;
            var totalCells = maze.Height * maze.Width;
            var currentCell = maze.StartCell;
            var backtrackStack = new Stack<Cell>();

            visitedCells[currentCell.Y, currentCell.X] = true;

            while (numberVisited < totalCells)
            {
                var unvisitedNeighbours = this.GetUnvisitedNeighbours(maze, currentCell, visitedCells);
                var unvisitedNeighbourCount = unvisitedNeighbours.Count();
                Cell nextCell;

                if (unvisitedNeighbourCount > 0)
                {
                    if (unvisitedNeighbourCount > 1)
                    {
                        backtrackStack.Push(currentCell);
                    }

                    nextCell = this.ChooseRandomCell(unvisitedNeighbours);
                    maze.RemoveWall(currentCell, nextCell);
                    visitedCells[nextCell.Y, nextCell.X] = true;
                    numberVisited++;
                    currentCell = nextCell;
                }
                else if (backtrackStack.Count > 0)
                {
                    while (backtrackStack.TryPop(out nextCell) && !this.GetUnvisitedNeighbours(maze, currentCell, visitedCells).Any())
                    {

                    }

                    currentCell = nextCell;
                }
            }

            return maze;
        }

        private IEnumerable<Cell> GetUnvisitedNeighbours(Maze maze, Cell cell, bool[,] visitedCells)
        {
            return maze.GetNeighbours(cell).Where(n => !visitedCells[n.Y, n.X]);
        }

        private Cell ChooseRandomCell(IEnumerable<Cell> cells)
        {
            var randomIndex = _random.Next(0, cells.Count());

            return cells.ElementAt(randomIndex);
        }
    }
}