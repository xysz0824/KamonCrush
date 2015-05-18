using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KamonCrush
{
    class FloatProperty
    {
        public event EventHandler ValueChanged;
        float value;
        public float Value
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
