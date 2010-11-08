using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeRobotics.Model;

namespace TradeRobotics.TradeLibrary
{
    /// <summary>
    /// Robot base functionality class
    /// </summary>
    public abstract class RobotBase:IRobot
    {
        #region Data provider interop
        public IDataProvider DataProvider
        {
            get
            {
                return dataProvider;
            }
            set
            {
                // Set data provider, subscribe tick event
                if (dataProvider != null)
                    dataProvider.Tick -= OnTick;
                dataProvider = value;
                if(dataProvider != null)
                    dataProvider.Tick += OnTick;
            }
        }
        private IDataProvider dataProvider;


        /// <summary>
        /// State idle or in process
        /// </summary>
        public RobotState State 
        {
            get 
            {
                return state;
            }
            set
            {
                state = value;
                if(StateChanged != null)
                    StateChanged(this, new RobotStateChangedEventArgs(state));
            }
        }
        private RobotState state;
        public event EventHandler<RobotStateChangedEventArgs> StateChanged;
        #endregion

        /// <summary>
        /// Robot trade adapter
        /// </summary>
        public ITradeAdapter TradeAdapter { get; set; }
        
        /// <summary>
        /// New data tick processing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public abstract void OnTick(object sender, TickEventArgs args);

    }
}
