using Leap;
using System;

namespace LeaPresen.Models.Actions
{
    public class SlideAction
    {
        private static Action<int> turnAction;

        public static void SetAction(Action<int> action)
        {
            turnAction = action;
        }

        public static void Turn(Frame frame)
        {
            foreach (Gesture gesture in frame.Gestures())
            {
                switch (gesture.Type)
                {
                    case Gesture.GestureType.TYPE_SWIPE:
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
            return swipe.Direction.x > 0 ? 1 : -1;
        }
    }
}
