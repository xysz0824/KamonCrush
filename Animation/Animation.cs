using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KamonCrush
{
    public enum AnimationState
    {
        Ready,
        Playing,
        Paused,
        Stoped,
        Finished
    }
    abstract class Animation
    {
        private AnimationState state;
        protected float origin;
        protected FloatProperty current;
        protected float target;
        protected int totalTime;
        protected int time;
        protected AnimationState State 
        { 
            get { return state; }
            set
            {
                state = value;
                if (value == AnimationState.Finished && Finished != null)
                    Finished();
            }
        }
        public AnimationState PlayState { get { return state; } }
        public event Action Finished;
        public Animation(FloatProperty property, float target, int totalTime)
        {
            state = AnimationState.Ready;
            this.origin = property.Value;
            current = property;
            this.target = target;
            this.totalTime = totalTime;
            AnimationManager.Add(this);
        }
        public bool SameTarget(Animation animation)
        {
            return current == animation.current;
        }
        public void Start()
        {
            state = AnimationState.Playing;
        }
        public void Pause()
        {
            state = AnimationState.Paused;
        }
        public void Stop()
        {
            time = 0;
            state = AnimationState.Stoped;
            current.Value = origin;
        }
        public void Finish()
        {
            time = 0;
            state = AnimationState.Finished;
            current.Value = target;
        }
        public void Update()
        {
            if (state == AnimationState.Playing)
            {
                ActionUpdate();
                time++;
            }
        }
        protected abstract void ActionUpdate();
    }
}
