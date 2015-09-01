﻿using GalaSoft.MvvmLight;
using System.IO;
using System.Linq;

namespace LeaPresen.ViewModels
{
    /// <summary>
    /// SlideShowを総括するクラス
    /// </summary>
    public class SlideShowViewModel : ViewModelBase
    {
        private static readonly string ImagePath = Directory.GetCurrentDirectory() + @"\Images";

        private string[] sources;
        private int currentId = 0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SlideShowViewModel()
        {
            sources = Directory.GetFiles(ImagePath);
            Source = sources[currentId];
        }

        public void SlideAction(int slideType)
        {
            currentId += slideType;
            currentId = currentId < 0 ? sources.Count() - 1 : currentId;
            currentId = currentId >= sources.Count() ? 0 : currentId;
            Source = sources[currentId];
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