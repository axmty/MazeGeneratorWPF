using System;
using System.Windows;

namespace MazeWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            var mazeDrawer = new MazeStepByStepDrawer(Area, 20, 200);
            var mazeEngine = new MazeEngine(mazeDrawer);

            mazeEngine.GenerateRandom(10, 10);
        }
    }
}
