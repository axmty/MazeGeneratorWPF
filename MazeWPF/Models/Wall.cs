using System.Windows.Shapes;

namespace MazeWPF.Models
{
    public class Wall
    {
        public Cell Cell1 { get; set; }

        public Cell Cell2 { get; set; }

        public Shape Shape { get; set; }
    }
}
