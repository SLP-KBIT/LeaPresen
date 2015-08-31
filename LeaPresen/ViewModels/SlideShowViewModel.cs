using GalaSoft.MvvmLight;
using LeaPresen.Models;

namespace LeaPresen.ViewModels
{
    /// <summary>
    /// SlideShowを総括するクラス
    /// </summary>
    public class SlideShowViewModel : ViewModelBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SlideShowViewModel()
        {
            LeapManager.SetPointAction(PointAction);
            LeapManager.Start();
        }

        /// <summary>
        /// ポインタ表示のアクション
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        private void PointAction(double x, double y)
        {
            X = x;
            Y = y;
        }

        private double _X;

        /// <summary>
        /// X座標の取得または設定
        /// </summary>
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

        /// <summary>
        /// Y座標の取得または設定
        /// </summary>
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