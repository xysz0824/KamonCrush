using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KamonCrush
{
    class QuadraticAnimation : Animation
    {
        float speed;
        float aspeed;
        public QuadraticAnimation(FloatProperty origin, float target, float aspeed)
            : base(origin, target, 0)
        {
            if (aspeed != 0)
                totalTime = (int)Math.Sqrt((target - origin.Value) * 2.0f / aspeed);
            this.aspeed = aspeed;
        }
        protected override void ActionUpdate()
        {
            speed += aspeed;
            current.Value += speed;
            if (time >= totalTime)
            {
                current.Value = target;
                State = AnimationState.Finished;
            }
        }
    }
}
