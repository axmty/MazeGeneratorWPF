using System;
using System.Windows;

namespace MazeWPF
{
    public partial class MainWindow : Window
    {
        private static readonly int CellSize = 30;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            var mazeDrawer = new MazeDrawer(Area, 20);
            var mazeEngine = new MazeEngine(mazeDrawer);

            mazeEngine.GenerateRandom(20, 20);
        }
    }
}
