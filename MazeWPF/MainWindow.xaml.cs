using System;
using System.Windows;
using System.Windows.Input;

namespace MazeWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            var mazeDrawer = new MazeStepByStepDrawer(Area, 30, 60);
            var mazeEngine = new MazeEngine(mazeDrawer);

            mazeEngine.GenerateRandom(10, 10);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}