using MazeWPF.Models;
using System.Collections.Generic;

namespace MazeWPF.Algorithms
{
    public interface ISolvingAlgorithm
    {
        IList<Cell> Solve(Maze maze);
    }
}
