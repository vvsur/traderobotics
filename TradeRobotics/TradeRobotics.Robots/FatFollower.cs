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
    public class FatFollower:WealthScriptBase
    {
        // Input parameters
        private StrategyParameter fatOrderCriteria;
        private StrategyParameter deltaUp;
        private StrategyParameter priceMaPeriod;
        private StrategyParameter trendMaPeriod;



        /// <summary>
		/// Constructor
		/// </summary>
		public FatFollower():base() 
		{
            //// Base class parameters.
            //StopLoss = CreateParameter("StopLoss", 0.75, 0, 5, 0.1);
            //TakeProfit = CreateParameter("TakeProfit", 3.56, 0, 5, 0.1);
            //// Trailing stop sets when price is reached TakeProfit. If zero, close on TakeProfit
            //TrailingStop = CreateParameter("TrailingStop", 3.63, 0, 5, 0.1);

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

            // FatOrderCriteria = FatOrder.Volume / averageVolume
            fatOrderCriteria = CreateParameter("FatOrderCriteria", 2, 1, 5, 0.5);

		}
		
        /// <summary>
        /// Main function - robot Entry point
        /// </summary>
        protected override void Execute()
		{

            
            base.Execute();
            lastSignalTime = DateTime.MinValue;
            if(Bars.Count > 1)
                Level2History.LoadIfNotLoaded(Date[1]);

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



        protected TradeRobotics.DataProviders.Quik.Dom.Order lastProcessedFatOrder = null;
        protected DateTime lastSignalTime = DateTime.MinValue;
        /// <summary>
        /// Get signal for the bar
        /// </summary>
        /// <returns></returns>
        protected SignalType GetSignal(int bar)
        {
            SignalType result = SignalType.None;
            TradeRobotics.DataProviders.Quik.Dom.Order fatOrder = GetFatOrder(Date[bar]);
            if (fatOrder == null 
                || fatOrder == lastProcessedFatOrder
                || (Date[bar] - lastSignalTime).Minutes <1
                )
            {
                return SignalType.None;
            }
            else if (fatOrder.OrderType == TradeRobotics.DataProviders.Quik.Dom.OrderType.Ask)
            {
                lastProcessedFatOrder = fatOrder;
                lastSignalTime = Date[bar];
                return SignalType.Sell;
            }
            else if (fatOrder.OrderType == TradeRobotics.DataProviders.Quik.Dom.OrderType.Bid)
            {
                lastProcessedFatOrder = fatOrder;
                lastSignalTime = Date[bar];
                return SignalType.Buy;
            }
            return result;
        }

        /// <summary>
        /// Get order with largest volume
        /// </summary>
        /// <returns></returns>
        protected TradeRobotics.DataProviders.Quik.Dom.Order GetFatOrder(DateTime time)
        {
            if (Level2History.Count == 0)
                return null;
            
            // Get latest level 2 
            Level2 level2 =  Level2History.LastOrDefault(curLevel2 => curLevel2.Time <= time);
            if (level2 == null)
                return null;

            TradeRobotics.DataProviders.Quik.Dom.Order firstRequest = new TradeRobotics.DataProviders.Quik.Dom.Order();
            TradeRobotics.DataProviders.Quik.Dom.Order secondRequest = new TradeRobotics.DataProviders.Quik.Dom.Order();
            
            foreach(TradeRobotics.DataProviders.Quik.Dom.Order request in level2.Orders)
            {
                if (request.Volume > firstRequest.Volume)
                {
                    secondRequest = firstRequest;
                    firstRequest = request;
                }
            }
            //double averageVolume = level2.Orders.Average(curLevel2 => curLevel2.Volume);

            // If order is fat, return it
            if(firstRequest.Volume >= secondRequest.Volume * fatOrderCriteria.Value)

            {
                return firstRequest;
            }
            return null;
        }
    }
}
