using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WealthLab;
using WealthLab.Indicators;
using TradeRobotics.DataProviders.Quik.Dom;


namespace TradeRobotics.TradeLibrary
{
    /// <summary>
    /// Main class
    /// </summary>
    public abstract class WealthScriptBase : WealthScript
    {
        protected Level2History Level2History;
        
        public StrategyParameter StopLoss;
        public StrategyParameter TakeProfit;
        public StrategyParameter TrailingStop;

        /// <summary>
        /// Constructor
        /// </summary>
        public WealthScriptBase()
            : base()
        {
            //StrategyParameter CreateParameter(string name, double value, double start, double stop, double step);

            ////StopLoss 
            StopLoss = CreateParameter("StopLoss", 0.1, 0, 5, 0.1);
            TakeProfit = CreateParameter("TakeProfit", 0.2, 0, 5, 0.1);
            //// Trailing stop sets when price is reached TakeProfit. If zero, close on TakeProfit
            TrailingStop = CreateParameter("TrailingStop", 0.1, 0, 5, 0.1);
        }

        /// <summary>
        /// Entry point
        /// </summary>
        protected override void Execute()
        {
            if (Bars is BarsAndDom)
                Level2History = (Bars as BarsAndDom).Level2History;

        }


        /// <summary>
        /// Close stops if needed
        /// </summary>
        /// <returns></returns>
        public void CloseStops(int bar)
        {
            if (!IsLastPositionActive)
                return;
            // If long position
            if (LastPosition.PositionType == PositionType.Long)
            {
                // Stop loss
                if (StopLoss.Value != 0
                    && LastPosition.EntryPrice - Low[bar] >= StopLoss.Value)
                {
                    ExitAtMarket(bar + 1, LastPosition, "StopLoss");
                }
                // Stop at trailing stop
                else if (LastPosition.TrailingStop != 0
                    && LastPosition.TrailingStop >= Low[bar])
                {
                    ExitAtMarket(bar + 1, LastPosition, "TrailingStop robot");
                }
                // Take profit
                else if (TakeProfit.Value != 0
                    && High[bar] - LastPosition.EntryPrice >= TakeProfit.Value)
                {
                    // In robot's TrailingStop=0 just take a profit
                    if (TrailingStop.Value == 0)
                    {
                        ExitAtMarket(bar + 1, LastPosition, "TakeProfit");
                    }
                    // If robot's TrailingStop parameter != 0,  move trailing stop up 
                    else if (LastPosition.TrailingStop == 0
                        || LastPosition.TrailingStop < High[bar] - TrailingStop.Value)
                    {
                        ExitAtTrailingStop(bar + 1, LastPosition, High[bar] - TrailingStop.Value, "TrailingStop");
                    }
                }
            }
            // If short position
            else
            {
                // Stop loss
                if (StopLoss.Value != 0
                    && High[bar] - LastPosition.EntryPrice >= StopLoss.Value)
                {
                    //Stop loss
                    ExitAtMarket(bar + 1, LastPosition, "StopLoss");
                }
                // Trailing stop
                else if (LastPosition.TrailingStop != 0
                    && LastPosition.TrailingStop <= High[bar])
                {
                    ExitAtMarket(bar + 1, LastPosition, "TrailingStop robot");
                }
                // Take profit
                else if (TakeProfit.Value != 0
                    && LastPosition.EntryPrice - Low[bar] >= TakeProfit.Value)
                {
                    // Just take profit
                    if (TrailingStop.Value == 0)
                    {
                        ExitAtMarket(bar + 1, LastPosition, "TakeProfit");
                    }
                    // If TrailingStop parameter != 0, move it down
                    else if (LastPosition.TrailingStop == 0
                        || LastPosition.TrailingStop > Low[bar] + TrailingStop.Value)
                    {

                        ExitAtTrailingStop(bar + 1, LastPosition, Low[bar] + TrailingStop.Value, "TrailingStop");
                    }
                }
            }
        }
    }
}