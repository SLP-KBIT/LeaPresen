using Leap;
using System;

namespace LeaPresen.Models.Actions
{
    /// <summary>
    /// ポインタを表示するアクションを総括する静的クラス
    /// </summary>
    public static class PointAction
    {
        private static Action<double, double> drawAction;

        /// <summary>
        /// ポインタを表示するアクションを設定する
        /// </summary>
        /// <param name="action">ポインタを表示するアクション</param>
        public static void SetAction(Action<double, double> action)
        {
            drawAction = action;
        }

        /// <summary>
        /// 指定されたアクションに沿ってポインタを表示する
        /// </summary>
        /// <param name="frame">現在のフレーム</param>
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
