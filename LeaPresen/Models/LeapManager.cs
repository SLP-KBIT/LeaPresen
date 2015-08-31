using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;
using System.Windows.Media;
using LeaPresen.Models.Actions;

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
