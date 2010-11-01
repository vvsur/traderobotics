using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeRobotics.TradeLibrary
{
    public interface IRobot
    {
        IDataProvider DataProvider {get;set;}
    }
}
