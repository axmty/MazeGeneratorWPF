using MazeWPF.Models;

namespace MazeWPF.Algorithms
{
    public interface IMazeGenerator
    {
        Maze Generate(int width, int height, (int x, int y) startCell, (int x, int y) exitCell);
    }
}
