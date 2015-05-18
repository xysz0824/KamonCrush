using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KamonCrush
{
    class HitEventArgs : EventArgs
    {
        int hit;

        public int Hit { get { return hit; } }
        public HitEventArgs(int hit)
        {
            this.hit = hit;
        }
    }
}
