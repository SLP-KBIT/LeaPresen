using GalaSoft.MvvmLight;
using LeaPresen.ViewModels;

namespace LeaPresen.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            SlideShowVM = new SlideShowViewModel();
        }

        public SlideShowViewModel SlideShowVM { get; set; }
    }
}