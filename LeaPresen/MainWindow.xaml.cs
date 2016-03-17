using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using System.IO;
using System.Windows.Ink;
using System.Windows.Threading;
using Leap;
using System.Windows.Controls;
using System.Diagnostics;
using System.Drawing;

namespace LeaPresen
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        const int DefaultWidth = 960;
        const int DefaultHeight = 720;
        const float TouchBorder = 0.0F;
        const float LineBorder = 0.25F;

        readonly string OutputPath = Directory.GetCurrentDirectory() + @"/tmp";
        Controller leap = new Controller();
        DrawingAttributes pointIndicator = new DrawingAttributes();
        DrawingAttributes touchIndicator = new DrawingAttributes();
        DrawingAttributes lineIndicator = new DrawingAttributes();
        DrawingAttributes waitIndicator = new DrawingAttributes();
        float windowWidth = DefaultWidth;
        float windowHeight = DefaultHeight;
        int totalSlideNum = 0;
        int currentSlideNum = 0;
        BitmapImage currentSlide;
        DispatcherTimer timerGesture = new DispatcherTimer(DispatcherPriority.Normal);
        bool timerGestureFlag = false;
       
        static Stopwatch showStopWatch = new Stopwatch();
        static Stopwatch sleepLine = new Stopwatch();
        bool lineDrawFlag = true;
        static Stopwatch sleepTurn = new Stopwatch();
        bool turnFlag = true;
        protected int rejectTime = 1000; 

        public MainWindow()
        {
            InitializeComponent();
            timerGesture.Interval = new TimeSpan(0, 0, 0, 1);
            timerGesture.Tick += new EventHandler(TimerGesture_Tick);

            timerGesture.Start();
            leap.EnableGesture(Gesture.GestureType.TYPESWIPE);
            touchIndicator.Width = touchIndicator.Height = 10;
            pointIndicator.Width = pointIndicator.Height = 20;
            lineIndicator.Width = lineIndicator.Height = 10;

            waitIndicator.Width = waitIndicator.Height = 10;

            touchIndicator.Color = System.Windows.Media.Color.FromArgb(0xff, 0xff, 0x0, 0x0);
            pointIndicator.Color = System.Windows.Media.Color.FromArgb(0xff, 0x0, 0xff, 0x0);
            lineIndicator.Color = System.Windows.Media.Color.FromArgb(0xf0, 0x0, 0x70, 0xff);
            waitIndicator.Color = System.Windows.Media.Color.FromArgb(0x80, 0x0, 0xff, 0x0);

        }

        ~MainWindow()
        {
            DirectoryInfo delDir = new DirectoryInfo(OutputPath);
            if (delDir.Exists)
            {
                delDir.Delete(true);
            }
        }

        //====================================================================
        //  Leap Motion
        //====================================================================

        protected void LeapUpdate(object sender, EventArgs e)
        {
            Leap.Frame frame = leap.Frame();

            DetectLeapGesture(leap, frame);
            DrawLeapPoint(frame);
            DrawLeapTouch(frame);
            DrawLeapLine(leap, frame);
            DrawLeapPicture(leap, frame);

            if (frame.Fingers.Extended().Count == 5 && leap.Frame(10).Fingers.Extended().Count <= 1)
            {
                this.InkCanvas_LeapPaintLine.Strokes.Clear();
            }
            // ストップウォッチの更新
            TimeCount.Text = showStopWatch.Elapsed.ToString(@"mm\:ss");
        }

        protected void DetectLeapGesture(Controller leap, Leap.Frame frame)
        {
            GestureList gestures = frame.Gestures();

            if ( sleepTurn.ElapsedMilliseconds > 1500 )
            {
                turnFlag = true;
                sleepTurn.Reset();
            }

            if (!turnFlag)
            {
                return;
            }
            

            foreach (Gesture gesture in gestures)
            {
                switch (gesture.Type)
                {
                    case Gesture.GestureType.TYPE_SWIPE:
                        turnFlag = false;
                        sleepTurn.Start();
                        TurnSlide(GetSwipeType(gesture));
                        return;
                    default:
                        break;
                }
            }
        }

        private static int GetSwipeType(Gesture gesture)
        {
            SwipeGesture swipe = new SwipeGesture(gesture);
            return swipe.Direction.x > 0 ? -1 : +1;
        }

        protected void DrawLeapTouch(Leap.Frame frame)
        {
            InteractionBox interactionBox = frame.InteractionBox;

            if (frame.Pointables.Extended().Count != 1)
            {
                return;
            }

            Pointable pointable = frame.Pointables.Extended()[0];

            // InteractionBox を利用した座標変換
            Leap.Vector normalizedPosition = interactionBox.NormalizePoint(pointable.StabilizedTipPosition);

            double tx = normalizedPosition.x * windowWidth;
            double ty = windowHeight - normalizedPosition.y * windowHeight;
            StylusPoint touchPoint = new StylusPoint(tx, ty);
            StylusPointCollection tips = new StylusPointCollection(new StylusPoint[] { touchPoint });

            // タッチ状態
            if (normalizedPosition.z <= TouchBorder)
            {
                Stroke touchStroke = new Stroke(tips, touchIndicator);
                this.InkCanvas_LeapPaintLine.Strokes.Add(touchStroke.Clone());
            }
        }

        protected void DrawLeapPoint(Leap.Frame frame)
        {
            this.InkCanvas_LeapPaint.Strokes.Clear();
            windowHeight = (float)this.MainWindow1.Height;
            windowWidth = (float)this.MainWindow1.Width;

            InteractionBox interactionBox = frame.InteractionBox;

            int i = 0;
            foreach (Pointable pointable in frame.Pointables.Extended())
            {
                if (i == 2) { break; } // 描画を2つまでに
                // InteractionBox を利用した座標変換
                Leap.Vector normalizedPosition = interactionBox.NormalizePoint(pointable.StabilizedTipPosition);

                double tx = normalizedPosition.x * windowWidth;
                double ty = windowHeight - normalizedPosition.y * windowHeight;
                StylusPoint touchPoint = new StylusPoint(tx, ty);
                StylusPointCollection tips = new StylusPointCollection(new StylusPoint[] { touchPoint });



                // ホバー状態
                if (normalizedPosition.z > TouchBorder)
                {
                    Stroke touchStroke = new Stroke(tips, pointIndicator);
                    this.InkCanvas_LeapPaint.Strokes.Add(touchStroke);
                }
                i++;
            }
        }

        protected void DrawLeapLine(Controller leap, Leap.Frame frame)
        {
            FingerList allFingers = frame.Fingers.Extended();

            if (allFingers.Count != 2 || leap.Frame(10).Fingers.Extended().Count != 2)
            {
                return;
            }

            Finger finger1 = allFingers.Leftmost;
            Finger finger2 = allFingers.Rightmost;

            InteractionBox interactionBox = frame.InteractionBox;
            Leap.Vector normalizedPosition1 = interactionBox.NormalizePoint(finger1.StabilizedTipPosition);
            Leap.Vector normalizedPosition2 = interactionBox.NormalizePoint(finger2.StabilizedTipPosition);

            double tx1 = normalizedPosition1.x * windowWidth;
            double ty1 = windowHeight - normalizedPosition1.y * windowHeight;
            double tx2 = normalizedPosition2.x * windowWidth;
            double ty2 = windowHeight - normalizedPosition2.y * windowHeight;

            StylusPointCollection tips = new StylusPointCollection();
            tips.Add(new StylusPoint(tx1, ty1));
            tips.Add(new StylusPoint(tx2, ty2));

            Stroke stroke = new Stroke(tips, lineIndicator);
            Stroke touchStroke = new Stroke(tips, touchIndicator);

            // アンダーラインの軌跡を残す
            Pointable pointable = frame.Pointables.Extended()[0];

            // InteractionBox を利用した座標変換
            
            if (normalizedPosition1.z <= LineBorder && normalizedPosition2.z <= LineBorder)
            {
                sleepLine.Start();
                stroke.DrawingAttributes = waitIndicator;

                if (sleepLine.ElapsedMilliseconds > 1000 && lineDrawFlag == true)
                {
                    lineDrawFlag = false;
                    this.InkCanvas_LeapPaintLine.Strokes.Add(touchStroke);
                }

                if (sleepLine.ElapsedMilliseconds > rejectTime)
                {
                    stroke.DrawingAttributes = lineIndicator;
                    lineDrawFlag = true;
                    sleepLine.Reset();
                }
            }
            else
            {
                sleepLine.Reset();
            }
            this.InkCanvas_LeapPaint.Strokes.Add(stroke);
        }

        protected void DrawLeapPicture(Controller leap, Leap.Frame frame)
        {
            FingerList allFingers = frame.Fingers.Extended();

            if (allFingers.Count != 4 || leap.Frame(10).Fingers.Extended().Count != 4)
            {
                this.Image_Cursor.Visibility = Visibility.Collapsed;
                return;
            }

            this.InkCanvas_LeapPaint.Strokes.Clear();
            windowHeight = (float)this.MainWindow1.Height;
            windowWidth = (float)this.MainWindow1.Width;

            InteractionBox interactionBox = frame.InteractionBox;

            Finger finger = allFingers.Rightmost;
            // InteractionBox を利用した座標変換
            Leap.Vector normalizedPosition = interactionBox.NormalizePoint(finger.StabilizedTipPosition);

            double tx = normalizedPosition.x * windowWidth;
            double ty = windowHeight - normalizedPosition.y * windowHeight;
            StylusPoint touchPoint = new StylusPoint(tx, ty);
            StylusPointCollection tips = new StylusPointCollection(new StylusPoint[] { touchPoint });

            // ホバー状態
            if (normalizedPosition.z > TouchBorder)
            {
                this.Canvas_Cursor.Visibility = Visibility.Visible;
                this.Image_Cursor.Visibility = Visibility.Visible;
                this.DataContext = new { X = tx, Y = ty };
            }

        }

        private void StartTimer(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null)
            {
                this.TextBox_SubmitFile.Text = files[0];
            }
        }

        //====================================================================
        //  パワーポイント系
        //====================================================================

        private void MainWindow1_Drop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null)
            {
                this.TextBox_SubmitFile.Text = files[0];
            }
        }

        private void MainWindow1_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) != null)
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        void TimerGesture_Tick(object sender, EventArgs e)
        {
            timerGestureFlag = false;
        }

        private void TurnSlide(int num)
        {
            if (timerGestureFlag)
            {
                return;
            }
            timerGestureFlag = true;
            timerGesture.Start();

            currentSlideNum += num;
            if (currentSlideNum >= totalSlideNum)
            {
                showStopWatch.Stop();
                currentSlideNum = 0;
            }
            else if (currentSlideNum < 0)
            {
                currentSlideNum = totalSlideNum - 1;
            }

            currentSlide = new BitmapImage(new Uri(OutputPath + String.Format("/slide{0:0000}.jpg", currentSlideNum)));
            this.Image_Slideshow.Source = currentSlide;
            this.InkCanvas_LeapPaintLine.Strokes.Clear();
        }

        private void Button_SubmitFile_Click(object sender, RoutedEventArgs e)
        {
            string fileName = this.TextBox_SubmitFile.Text;
            string fileType = System.IO.Path.GetExtension(fileName);

            if (fileType != ".pptx" && fileType != ".ppt")
            {
                MessageBox.Show("拡張子が違います");
                return;
            }
            OutputImageFiles(fileName);

            CompositionTarget.Rendering += LeapUpdate;   // LeapMotionセンサを有効化
            this.InkCanvas_LeapPaint.Visibility = Visibility.Visible;
            this.InkCanvas_LeapPaintLine.Visibility = Visibility.Visible;
            TurnSlide(0);
            //MainWindow1.WindowState = System.Windows.WindowState.Maximized;

            // ストップウォッチのスタート(TickTimerの定期実行)
            this.Canvas_StopWatch.Visibility = Visibility.Visible;
            showStopWatch.Start();
            
        }

        private void OutputImageFiles(string fileName)
        {
            Directory.CreateDirectory(OutputPath);

            Microsoft.Office.Interop.PowerPoint.Application app = null;
            Presentation ppt = null;
            try
            {
                app = new Microsoft.Office.Interop.PowerPoint.Application();
                ppt = app.Presentations.Open(fileName, MsoTriState.msoTrue, MsoTriState.msoFalse, MsoTriState.msoFalse);
                int width = DefaultWidth * 2;
                int height = DefaultHeight * 2;
                totalSlideNum = ppt.Slides.Count;

                for (int i = 1; i <= totalSlideNum; i++)
                {
                    string output = OutputPath + String.Format("/slide{0:0000}.jpg", i - 1);
                    ppt.Slides[i].Export(output, "jpg", width, height);
                }
            }
            finally
            {
                if (ppt != null)
                {
                    ppt.Close();
                }

                if (app != null)
                {
                    app.Quit();
                    app = null;
                }
            }
        }

        //  緊急用 : カーソルキーでのスライドめくり
        private void MainWindow1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Up)
            {
                TurnSlide(-1);
            }
            else if (e.Key == Key.Right || e.Key == Key.Down)
            {
                TurnSlide(+1);
            }
        }

    }
}
