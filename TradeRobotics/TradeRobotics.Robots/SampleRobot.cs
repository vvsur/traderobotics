using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeRobotics.TradeLibrary;
using TradeRobotics.Model;

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
                    TradeAdapter.BuyAtMarket(DataProvider.DataSeries.Symbol, 1);
                }

            }
            State = RobotState.Idle;
        }
    }
}
