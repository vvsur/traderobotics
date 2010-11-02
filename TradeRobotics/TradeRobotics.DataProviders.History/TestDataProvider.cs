using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeRobotics.TradeLibrary;
using TradeRobotics.Model;

namespace TradeRobotics.DataProviders.History
{
    public class TestDataProvider:HistoryDataProvider
    {
        /// <summary>
        /// Testing robot
        /// </summary>
        public IRobot Robot
        {
            get
            {
                return robot;
            }
            set
            {
                robot = value;
                if(robot != null)
                    robot.StateChanged += OnRobotStateChanged;
            }
        }
        private IRobot robot;

        private DateTime lastTickTime;
        protected void OnRobotStateChanged(object sender,  RobotStateChangedEventArgs e)
        {
            
        }

        public void BeginTest(StockDataSeries bars, IRobot robot)
        {
            robot.DataProvider = this;
            this.Robot = robot;
        }
        

    }
}
