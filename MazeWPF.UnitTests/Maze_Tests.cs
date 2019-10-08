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
            var maze = new Maze(10, 10);
            var cell = maze[0, 0];

            var neighbours = maze.GetNeighbours(cell);

            neighbours
                .Select(c => (c.X, c.Y))
                .Should()
                .BeEquivalentTo((0, 1), (1, 0));
        }

        [Fact]
        public void GetNeighbours_OfStrictBorderCell_ReturnsThreeNeighbours()
        {
            var maze = new Maze(10, 10);
            var cell = maze[4, 0];

            var neighbours = maze.GetNeighbours(cell);
            
            neighbours
                .Select(c => (c.X, c.Y))
                .Should()
                .BeEquivalentTo((3, 0), (5, 0), (4, 1));
        }

        [Fact]
        public void GetNeighbours_OfInsideCell_ReturnsFourNeighbours()
        {
            var maze = new Maze(10, 10);
            var cell = maze[3, 3];

            var neighbours = maze.GetNeighbours(cell);

            neighbours
                .Select(c => (c.X, c.Y))
                .Should()
                .BeEquivalentTo((3, 4), (3, 2), (4, 3), (2, 3));
        }
    }
}
