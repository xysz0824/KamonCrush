using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KamonCrush
{
    class LinearAnimation : Animation
    {
        public LinearAnimation(FloatProperty origin, float target, int totalTime)
            : base(origin, target, totalTime)
        {
        }
        protected override void ActionUpdate()
        {
            if (time >= totalTime)
                State = AnimationState.Finished;
            else
                current.Value += (target - origin) / totalTime;
        }
    }
}
