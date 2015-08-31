using GalaSoft.MvvmLight;
using LeaPresen.ViewModels;

namespace LeaPresen.ViewModels
{
    /// <summary>
    /// Main
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainViewModel()
        {
            SlideShowVM = new SlideShowViewModel();
        }

        /// <summary>
        /// SlideShowViewModel
        /// </summary>
        public SlideShowViewModel SlideShowVM { get; set; }
    }
}