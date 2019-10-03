using MazeWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MazeWPF.Algorithms
{
    public class BacktrackingGenerationAlgoritm : IGenerationAlgorithm
    {
        private readonly Random _random = new Random();

        public IList<Wall> WallsToOpen { get; } = new List<Wall>();

        /// <summary>
        /// Algorithm is following :
        /// 1. Make the initial cell the current cell and mark it as visited
        /// 2. While there are unvisited cells
        ///     1. If the current cell has any neighbours which have not been visited
        ///         1. Choose randomly one of the unvisited neighbours
        ///         2. Push the current cell to the stack if it has more than one unvisited neighbor
        ///         3. Remove the wall between the current cell and the chosen cell
        ///         4. Make the chosen cell the current cell and mark it as visited
        ///     2. Else if stack is not empty
        ///         1. Pop a cell from the stack while the stack is not empty and the popped cell has no unvisited neighbors
        ///         2. Make it the current cell
        /// </summary>
        /// <see cref="https://en.wikipedia.org/wiki/Maze_generation_algorithm#Depth-first_search"/>
        public Maze Generate(int width, int height, (int x, int y) startPosition, (int x, int y) exitPosition)
        {
            var maze = new Maze(width, height, startPosition, exitPosition);
            var visitedCells = new bool[maze.Height, maze.Width];
            var numberVisited = 1;
            var currentCell = maze.StartCell;
            var backtrackStack = new Stack<Cell>();

            visitedCells[currentCell.Y, currentCell.X] = true;

            while (numberVisited < maze.CellCount)
            {
                var unvisitedNeighbours = this.GetUnvisitedNeighbours(maze, currentCell, visitedCells);
                var unvisitedNeighbourCount = unvisitedNeighbours.Count();
                Cell nextCell;

                if (unvisitedNeighbourCount > 0 && currentCell != maze.ExitCell)
                {
                    if (unvisitedNeighbourCount > 1 && numberVisited > 1)
                    {
                        backtrackStack.Push(currentCell);
                    }

                    nextCell = this.ChooseRandomCell(unvisitedNeighbours);
                    this.WallsToOpen.Add(maze.GetWallBetween(currentCell, nextCell));
                    visitedCells[nextCell.Y, nextCell.X] = true;
                    numberVisited++;
                    currentCell = nextCell;
                }
                else if (backtrackStack.Count > 0)
                {
                    while (backtrackStack.TryPop(out nextCell))
                    {
                        var anyUnvisitedNeighbour = this.GetUnvisitedNeighbours(maze, nextCell, visitedCells).Any();

                        if (anyUnvisitedNeighbour)
                        {
                            break;
                        }
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