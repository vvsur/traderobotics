using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeRobotics.TradeLibrary
{
    public class TickEventArgs:EventArgs
    {
        public DateTime TickTime { get; set; }
    }
}
