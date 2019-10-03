using MazeWPF.Models;
using Xunit;
using FluentAssertions;
using System.Linq;

namespace MazeWPF.UnitTests
{
    public class Maze_Tests
    {
        [Fact]
        public void GetNeighbours_OfCornerCell_ReturnsTwoNeighbours()
        {
            var startCellPosition = (0, 0);
            var maze = new Maze(10, 10, startCellPosition, (0, 3));

            var neighbours = maze.GetNeighbours(maze.StartCell);

            neighbours
                .Select(c => (c.X, c.Y))
                .Should()
                .BeEquivalentTo((0, 1), (1, 0));
        }

        [Fact]
        public void GetNeighbours_OfStrictBorderCell_ReturnsThreeNeighbours()
        {
            var startCellPosition = (4, 0);
            var maze = new Maze(10, 10, startCellPosition, (0, 3));

            var neighbours = maze.GetNeighbours(maze.StartCell);

            neighbours
                .Select(c => (c.X, c.Y))
                .Should()
                .BeEquivalentTo((3, 0), (5, 0), (4, 1));
        }

        [Fact]
        public void GetNeighbours_OfInsideCell_ReturnsFourNeighbours()
        {
            var maze = new Maze(10, 10, (0, 0), (0, 3));
            var insideCell = maze.Cells.First(c => (c.X, c.Y) == (3, 3));

            var neighbours = maze.GetNeighbours(insideCell);

            neighbours
                .Select(c => (c.X, c.Y))
                .Should()
                .BeEquivalentTo((3, 4), (3, 2), (4, 3), (2, 3));
        }
    }
}
