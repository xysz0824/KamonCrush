using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KamonCrush
{
    class TimerAnimation : Animation 
    {
        public TimerAnimation(int totalTime)
            : base(new FloatProperty(), 0, totalTime)
        {
        }
        protected override void ActionUpdate()
        {
            if (time >= totalTime)
                State = AnimationState.Finished;
        }
    }
}
