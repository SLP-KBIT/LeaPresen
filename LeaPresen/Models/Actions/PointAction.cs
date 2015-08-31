using Leap;
using System;

namespace LeaPresen.Models.Actions
{
    public static class PointAction
    {
        private static Action<double, double> drawAction;

        public static void SetAction(Action<double, double> action)
        {
            drawAction = action;
        }

        public static void Draw(Frame frame)
        {
            if (drawAction == null)
            {
                return;
            }
            var pointable = frame.Pointables.Extended().Leftmost;
            var box = frame.InteractionBox;
            var normalizedPosition = box.NormalizePoint(pointable.StabilizedTipPosition);
            drawAction(normalizedPosition.x * 940, (1 - normalizedPosition.y) * 680);
        }
    }
}
