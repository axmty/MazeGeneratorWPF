using MazeWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MazeWPF.Draw
{
    public class StepByStepGenerationDrawer
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private readonly IEnumerable<Wall> _wallsToOpen;
        private readonly int _totalStep;
        private readonly UIElementCollection _allWallShapes;

        private int _currentStep = 0;

        public StepByStepGenerationDrawer(int interval, IEnumerable<Wall> wallsToOpen, UIElementCollection allWallShapes)
        {
            _allWallShapes = allWallShapes;
            _wallsToOpen = wallsToOpen;
            _totalStep = wallsToOpen.Count();
            _timer.Interval = TimeSpan.FromMilliseconds(interval);
            _timer.Tick += this.Timer_Tick;
        }

        public void Draw()
        {
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_currentStep >= _totalStep)
            {
                _timer.Stop();
            }
            else
            {
                _allWallShapes.Remove(_wallsToOpen.ElementAt(_currentStep).Shape);
                _currentStep++;
            }
        }
    }
}
