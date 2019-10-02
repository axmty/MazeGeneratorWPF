using MazeWPF.Models;
using System.Collections.Generic;

namespace MazeWPF.Algorithms
{
    public interface IGenerationAlgorithm
    {
        IList<Wall> WallsToOpen { get; }

        Maze Generate(int width, int height, (int x, int y) startCell, (int x, int y) exitCell);
    }
}
