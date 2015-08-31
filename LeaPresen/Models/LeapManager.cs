using Leap;
using LeaPresen.Models.Actions;
using System;
using System.Windows.Media;

namespace LeaPresen.Models
{
    public static class LeapManager
    {
        private static readonly Controller controller;

        static LeapManager()
        {
            controller = new Controller();
        }

        public static void Start()
        {
            CompositionTarget.Rendering += Update;
        }

        public static void SetPointAction(Action<double, double> pointAction)
        {
            PointAction.SetAction(pointAction);
        }

        private static void Update(object sender, EventArgs e)
        {
            PointAction.Draw(controller.Frame());
        }
    }
}
