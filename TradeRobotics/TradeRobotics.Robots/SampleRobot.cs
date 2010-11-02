using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeRobotics.TradeLibrary;

namespace TradeRobotics.Robots
{
    public class SampleRobot:IRobot
    {
        public IDataProvider DataProvider
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public RobotState State
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<RobotStateChangedEventArgs> StateChanged;
    }
}
