using GalaSoft.MvvmLight;
using LeaPresen.Models;
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
            PointerVM = new PointerViewModel();
            LeapManager.SetPointAction(PointerVM.PointAction);
            LeapManager.SetSlideAction(SlideShowVM.SlideAction);
            LeapManager.Start();
        }

        /// <summary>
        /// SlideShowViewModel
        /// </summary>
        public SlideShowViewModel SlideShowVM { get; set; }

        /// <summary>
        /// PointerViewModel
        /// </summary>
        public PointerViewModel PointerVM { get; set; }
    }
}