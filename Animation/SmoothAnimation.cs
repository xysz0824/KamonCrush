using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KamonCrush
{
    class SmoothAnimation : Animation
    {
        public SmoothAnimation(FloatProperty origin, float target, int totalTime)
            : base(origin, target, totalTime)
        {
        }
        protected override void ActionUpdate()
        {
            time = 0;  //由于渐近线无限趋近目标值，因此不能计时停止而要比较
            current.Value += (target - current.Value) / totalTime;
            if (Math.Abs(current.Value - target) < 0.001f)
            {
                current.Value = target;
                State = AnimationState.Finished;
            }
        }
    }
}
