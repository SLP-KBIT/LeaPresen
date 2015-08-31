using GalaSoft.MvvmLight;
using LeaPresen.Models;

namespace LeaPresen.ViewModels
{
    public class SlideShowViewModel : ViewModelBase
    {
        public SlideShowViewModel()
        {
            LeapManager.SetPointAction(PointAction);
            LeapManager.Start();
        }

        private void PointAction(double x, double y)
        {
            X = x;
            Y = y;
        }

        private double _X;

        public double X
        {
            get { return _X; }
            set
            {
                if (_X != value)
                {
                    _X = value;
                    RaisePropertyChanged("X");
                }
            }
        }

        private double _Y;

        public double Y
        {
            get { return _Y; }
            set
            {
                if (_Y != value)
                {
                    _Y = value;
                    RaisePropertyChanged("Y");
                }
            }
        }

    }
}