using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KamonCrush
{
    interface IInputListener
    {
        bool ItemClicked(GameItem sender, bool preState);
    }
}
