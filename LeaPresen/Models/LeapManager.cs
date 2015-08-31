using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;
using System.Windows.Media;

namespace LeaPresen.Models
{
    public static class LeapManager
    {
        private static Controller controller;

        private static Action<double, double> pointAction;

        static LeapManager()
        {
            controller = new Controller();
        }

        public static void Start()
        { 
            CompositionTarget.Rendering += Update;
        }

        public static void SetPointAction(Action<double, double> _pointAction)
        {
            pointAction = _pointAction;
        }

        private static void Update(object sender, EventArgs e)
        {
            DrawPoint(controller.Frame());
        }

        private static void DrawPoint(Frame frame)
        {
            var pointable = frame.Pointables.Extended().Leftmost;
            var box = frame.InteractionBox;
            var normalizedPosition = box.NormalizePoint(pointable.StabilizedTipPosition);
            pointAction(normalizedPosition.x * 940, (1 - normalizedPosition.y) * 680);
        }
    }
}
