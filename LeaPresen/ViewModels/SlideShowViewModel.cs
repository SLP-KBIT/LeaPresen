using GalaSoft.MvvmLight;
using LeaPresen.Models;
using System.IO;

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
            Source = Directory.GetCurrentDirectory() + @"\Images\スライド1.PNG";
            System.Console.WriteLine(Source);
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

        private string _Source;

        /// <summary>
        /// 表示する画像のパス
        /// </summary>
        public string Source
        {
            get { return _Source; }
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                    RaisePropertyChanged("Source");
                }
            }
        }
    }
}