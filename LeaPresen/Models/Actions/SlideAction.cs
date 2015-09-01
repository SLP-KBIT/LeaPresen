using Leap;
using System;
using System.Diagnostics;

namespace LeaPresen.Models.Actions
{
    public static class SlideAction
    {
        private const long Interval = 750;

        private static Action<int> turnAction;

        private static Stopwatch stopWatch = new Stopwatch();
        private static bool gesturedFlag = false;

        public static void SetAction(Action<int> action)
        {
            turnAction = action;
        }

        public static void Turn(Frame frame)
        {
            if (stopWatch.ElapsedMilliseconds > Interval)
            {
                gesturedFlag = false;
                stopWatch.Reset();
            }

            if (gesturedFlag)
            {
                return;
            }

            foreach (Gesture gesture in frame.Gestures())
            {
                switch (gesture.Type)
                {
                    case Gesture.GestureType.TYPE_SWIPE:
                        gesturedFlag = true;
                        stopWatch.Start();
                        turnAction(GetSwipeType(gesture));
                        return;
                    default:
                        break;
                }
            }
        }

        private static int GetSwipeType(Gesture gesture)
        {
            SwipeGesture swipe = new SwipeGesture(gesture);
            return swipe.Direction.x > 0 ? -1 : 1;
        }
    }
}
