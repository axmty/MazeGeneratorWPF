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
            var numberVisited = 0;
            var numberCells = maze.Height * maze.Width;
            var currentCell = maze.StartCell;
            var backtrackStack = new Stack<Cell>();

            do
            {
                visitedCells[currentCell.Y, currentCell.X] = true;
                numberVisited++;

                var neighbours = maze.GetNeighbours(currentCell);
                var unvisitedNeighbours = neighbours.Where(n => !visitedCells[n.Y, n.X]);
                var unvisitedNeighbourCount = unvisitedNeighbours.Count();
                Cell chosenCell;

                if (unvisitedNeighbourCount > 0)
                {
                    chosenCell = this.ChooseRandomCell(unvisitedNeighbours);
                    if (unvisitedNeighbourCount > 1)
                    {
                        backtrackStack.Push(currentCell);
                    }

                    maze.RemoveWall(currentCell, chosenCell);
                    visitedCells[chosenCell.Y, chosenCell.Y] = true;
                    numberVisited++;
                    currentCell = chosenCell;
                }
                else if (backtrackStack.Count > 0)
                {
                    do
                    {
                        chosenCell = backtrackStack.Pop();
                        neighbours = maze.GetNeighbours(chosenCell);
                        unvisitedNeighbourCount = this.WhereUnvisited(visitedCells, neighbours).Count();
                    } while (backtrackStack.Count > 0 && unvisitedNeighbourCount == 0);

                    currentCell = chosenCell;
                }
            } while (numberVisited < numberCells);

            return maze;
        }

        private IEnumerable<Cell> WhereUnvisited(bool[,] visitedCells, IEnumerable<Cell> cells)
        {
            return cells.Where(n => !visitedCells[n.Y, n.X]);
        }

        private Cell ChooseRandomCell(IEnumerable<Cell> cells)
        {
            var randomIndex = _random.Next(0, cells.Count());

            return cells.ElementAt(randomIndex);
        }
    }
}