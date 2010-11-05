using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeRobotics.TradeLibrary;
using TradeRobotics.Model;

namespace TradeRobotics.DataProviders
{
    /// <summary>
    /// Robot test data provider
    /// </summary>
    public class TestDataProvider:HistoryDataProvider, IDataProvider
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
                    if (robot != null)
                        robot.StateChanged -= OnRobotStateChanged;
                    robot = value;
                    if (robot != null)
                        robot.StateChanged += OnRobotStateChanged;
            }
        }
        private IRobot robot;

        /// <summary>
        /// Current timeline bar index
        /// </summary>
        public int BarIndex = 0;
        
        
        /// <summary>
        /// When robot came to idle state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnRobotStateChanged(object sender,  RobotStateChangedEventArgs e)
        {
            if (e.NewState == RobotState.Idle && BarIndex < DataSeries.Count)
                Tick(this, new TickEventArgs(DataSeries, BarIndex++));
        }

        /// <summary>
        /// Start new test
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="robot"></param>
        public void BeginTest( IRobot robot)
        {
            // Connect robot to provider
            robot.DataProvider = this;
            this.Robot = robot;
            BarIndex = 0;
            TestContext.CurrentBarIndex = BarIndex;
            TestContext.CurrentTime = DataSeries.Times[BarIndex];
            // First tick
            Tick(this, new TickEventArgs(DataSeries,BarIndex++));
        }

        /// <summary>
        /// Stop test, disconnect from robot
        /// </summary>
        public void StopTest()
        {
            this.Robot = null;

        }
        /// <summary>
        /// New data tick
        /// </summary>
        public new event EventHandler<TickEventArgs> Tick;
    }
}
