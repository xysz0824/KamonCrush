using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KamonCrush
{
    class AnimationManager
    {
        List<Animation> animations;

        private static readonly AnimationManager instance = new AnimationManager();
        private AnimationManager()
        {
            animations = new List<Animation>();
        }
        public static void Add(Animation animation)
        {
            for (int i = 0; i < instance.animations.Count; ++i)
                if (instance.animations[i].SameTarget(animation))
                {
                    instance.animations[i] = animation;
                    return;
                }
            
            instance.animations.Add(animation);
        }
        public static void Delete(Animation animation)
        {
            instance.animations.Remove(animation);
        }
        public static bool Empty()
        {
            return instance.animations.Count == 0;
        }
        public static void Update()
        {
            for (int i = 0; i < instance.animations.Count; ++i)
                if (instance.animations[i].PlayState != AnimationState.Finished)
                    instance.animations[i].Update();
                else
                {
                    instance.animations.RemoveAt(i);
                    i--;
                }
        }   
    }
}
