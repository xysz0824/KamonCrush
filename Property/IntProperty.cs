using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KamonCrush
{
    class IntProperty
    {
        public event EventHandler ValueChanged;
        int value;
        public int Value
        { 
            get { return value; }
            set 
            { 
                this.value = value;
                if (ValueChanged != null)
                    ValueChanged(this, null);
            }
        }
    }
}
