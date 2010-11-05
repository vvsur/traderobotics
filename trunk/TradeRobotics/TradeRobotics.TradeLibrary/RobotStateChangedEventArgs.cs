using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeRobotics.Model;

namespace TradeRobotics.TradeLibrary
{
    /// <summary>
    /// Robot state changed event args
    /// </summary>
    public class RobotStateChangedEventArgs:EventArgs
    {
        /// <summary>
        /// New robot state
        /// </summary>
        public RobotState NewState {get;set;}
        
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="?"></param>
        public RobotStateChangedEventArgs(RobotState newState)
        {
            NewState = newState;
        }
    }
}
