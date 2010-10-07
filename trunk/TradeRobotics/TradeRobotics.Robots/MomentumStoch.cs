using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using WealthLab;
using WealthLab.Indicators;

using TradeRobotics.TradeLibrary;
using TradeRobotics.DataProviders.Quik.Dom;


namespace TradeRobotics.Robots
{
    /// <summary>
    /// Robot class
    /// </summary>
    public class MomentumStoch:WealthScriptBase
    {
        // Input parameters
        private StrategyParameter momentumPeriod;

        private StrategyParameter stochDPeriod;
        private StrategyParameter stochDSmooth;
        private StrategyParameter stochKPeriod;


        DataSeries momentumSeries;
        DataSeries stochDSeries;
        DataSeries stochKSeries;

        /// <summary>
		/// Constructor
		/// </summary>
		public MomentumStoch():base() 
		{
            InitParameters();
		}

        /// <summary>
        /// Parameters initialization
        /// </summary>
        protected void InitParameters()
        {

            StopLoss.Start = 0;
            StopLoss.Stop = 50;
            StopLoss.DefaultValue = 10;
            StopLoss.Step = 5;

            TakeProfit.Start = 0;
            TakeProfit.Stop = 100;
            TakeProfit.DefaultValue = 10;
            TakeProfit.Step = 10;

            TakeProfit.Start = 0;
            TakeProfit.Stop = 50;
            TakeProfit.DefaultValue = 0;
            TakeProfit.Step = 10;


            momentumPeriod = CreateParameter("MomentumPeriod", 12, 5, 100, 5);
            stochKPeriod = CreateParameter("StockKPeriod", 11, 11, 100, 5);
            stochDSmooth = CreateParameter("StockKSmoothPeriod", 5, 5, 50, 5);
            stochDPeriod = CreateParameter("StockDPeriod", 5, 5, 100, 5);
        }

        protected void CalculateIndicators()
        {
            // Init indicators
            momentumSeries = Momentum.Series(Close, momentumPeriod.ValueInt);
            DataSeries stochDSeries = StochD.Series(Bars, stochDSmooth.ValueInt, stochDPeriod.ValueInt);
            DataSeries stochKSeries = StochK.Series(Bars, stochKPeriod.ValueInt);
        }
		
        /// <summary>
        /// Main function - robot Entry point
        /// </summary>
        protected override void Execute()
		{
            base.Execute();

            CalculateIndicators();

            for(int bar = 1; bar < Bars.Count; bar++)
			{
                // Calculate signal
                SignalType signal = GetSignal(bar);
                
                // Close opened
                if (IsLastPositionActive)
				{
                    // If isn't closed by stops
                    if(IsLastPositionActive)
                    {
                        CloseStops(bar);

                        // Close long if go below low level
                        if(LastPosition.PositionType == PositionType.Long
                            && signal == SignalType.Sell)
                        {
                            ExitAtMarket(bar+1, LastPosition, "Sell signal");
                            ShortAtMarket(bar + 1, "Sell signal");
                        }
                        // Close short if go upper high level
                        else if (LastPosition.PositionType == PositionType.Short 
                            && signal == SignalType.Buy)
                        {
                            ExitAtMarket(bar+1, LastPosition, "Buy signal");
                            BuyAtMarket(bar + 1, "Buy signal");
                        }
                    }
				}
				// Open new positions
                else //if(bar > 1)
				{
                    // Buy if go up more then deltaUp
                    if (signal == SignalType.Buy)
                    {
                        BuyAtMarket(bar + 1, "Buy signal");
                    }
                    else if(signal == SignalType.Sell)
                    {
                        ShortAtMarket(bar + 1, "Sell signal");
                    }
				}
			}
            
            // Export Equity and Drawdown to Excel
            //Export export = new Export(this.Bars, this.Positions, this.MarketPosition);
            //export.OpenExcel();
		}




        /// <summary>
        /// Get signal for the bar
        /// </summary>
        /// <returns></returns>
        protected SignalType GetSignal(int bar)
        {
            SignalType result = SignalType.None;
            if (this.IsLastPositionActive)
                return result;
            
            if (stochKSeries[bar - 1] > stochDSeries[bar - 1] // Fast stochK cross down low stochD
                    && stochKSeries[bar] < stochKSeries[bar]  // 
                    && stochDSeries[bar] > 0
                    // momentum go down
                    && momentumSeries[bar-1] > momentumSeries[bar])
            {
                return SignalType.Sell;
            }
            else if (stochKSeries[bar - 1] < stochDSeries[bar - 1] // Fast stochK cross up low stochD
                    && stochKSeries[bar] > stochKSeries[bar]  // 
                    && stochDSeries[bar] < 0
                // momentum go up
                    && momentumSeries[bar - 1] < momentumSeries[bar])
            {
                return SignalType.Buy;
            }
            return SignalType.None;
        }

    }
}
