using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeRobotics.Model;

namespace TradeRobotics.TradeLibrary
{
    /// <summary>
    /// Robot interface
    /// </summary>
    public interface IRobot
    {
        /// <summary>
        /// Robot data provider
        /// </summary>
        IDataProvider DataProvider {get;set;}

        ITradeAdapter TradeAdapter { get; set; }

        /// <summary>
        /// Robot state idle or processing
        /// </summary>
        RobotState State { get; set; }
        
        /// <summary>
        /// Robot state changed event
        /// </summary>
        event EventHandler<RobotStateChangedEventArgs> StateChanged;
    }
}
