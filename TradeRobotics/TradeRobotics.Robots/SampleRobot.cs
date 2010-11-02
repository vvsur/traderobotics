using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeRobotics.TradeLibrary;

namespace TradeRobotics.Robots
{
    /// <summary>
    /// A simple robot
    /// </summary>
    public class SampleRobot: RobotBase
    {
        /// <summary>
        /// New data tick processing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void OnTick(object sender, TickEventArgs args)
        {
            State = RobotState.Processing;
            for (int i = args.LastBarIndex; i > 0; i--)
            {
                if (i % 10 == 0)
                {
                    // buy
                }

            }
            State = RobotState.Idle;
        }
    }
}
