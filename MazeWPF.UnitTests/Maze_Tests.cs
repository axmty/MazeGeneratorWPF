using MazeWPF.Models;
using Xunit;
using FluentAssertions;
using System.Linq;
using System;

namespace MazeWPF.UnitTests
{
    public class Maze_Tests
    {
        [Fact]
        public void GetWallBetween_NonNeighboringCells_ThrowsException()
        {
            var maze = new Maze(10, 10, (0, 0), (0, 3));
            var cell1 = maze[0, 0];
            var cell2 = maze[1, 1];

            Action act = () => maze.GetWallBetween(cell1, cell2);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetWallBetween_NeighboringCells_ReturnsTheWall()
        {
            var maze = new Maze(10, 10, (0, 0), (0, 3));
            var cell1 = maze[0, 0];
            var cell2 = maze[0, 1];

            var wall = maze.GetWallBetween(cell1, cell2);

            wall
                .Should()
                .Match<Wall>(w => (w.Cell1 == cell1 && w.Cell1 == cell2) || (w.Cell2 == cell1 && w.Cell2 == cell2));
        }

        [Fact]
        public void GetNeighbours_OfCornerCell_ReturnsTwoNeighbours()
        {
            var maze = new Maze(10, 10, (0, 0), (0, 3));
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
            var maze = new Maze(10, 10, (0, 0), (0, 3));
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
            var maze = new Maze(10, 10, (0, 0), (0, 3));
            var cell = maze[3, 3];

            var neighbours = maze.GetNeighbours(cell);

            neighbours
                .Select(c => (c.X, c.Y))
                .Should()
                .BeEquivalentTo((3, 4), (3, 2), (4, 3), (2, 3));
        }
    }
}
