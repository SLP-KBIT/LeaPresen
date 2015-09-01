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
            Source = Directory.GetCurrentDirectory() + @"\Images\スライド1.PNG";
            System.Console.WriteLine(Source);
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