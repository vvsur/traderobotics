using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using WealthLab;
using WealthLab.Indicators;

using TradeRobotics.TradeLibrary;


namespace TradeRobotics.Robots
{
    /// <summary>
    /// Trade on gaps
    /// </summary>
    public class GapCatcher : WealthScriptBase
    {

        public StrategyParameter MinGapSize;

        /// <summary>
        /// Constructor
        /// </summary>
        public GapCatcher()
            : base()
        {

            MinGapSize = CreateParameter("MinGapSize", 0, 0, 3, 0.1);
            // Base class parameters.
            /*            StopLoss = CreateParameter("StopLoss", 0.75, 0, 5, 0.1);
                        TakeProfit = CreateParameter("TakeProfit", 3.56, 0, 5, 0.1);
                        // Trailing stop sets when price is reached TakeProfit. If zero, close on TakeProfit
                        TrailingStop = CreateParameter("TrailingStop", 3.63, 0, 5, 0.1);
                        */

        }

        /// <summary>
        /// Main function - robot Entry point
        /// </summary>
        protected override void Execute()
        {
            base.Execute();

            if (Bars.Count > 0)
                gapDate = Date[0];
            for (int bar = 1; bar < Bars.Count; bar++)
            {
                // Init gap data
                if (Date[bar].Day != gapDate.Day)
                {
                    yesterdayPrice = Close[bar-1];
                    todayPrice = Open[bar];
                    gapDate = Date[bar];
                    isGapProcessed = false;
                }
                
                // Calculate signal
                SignalType signal = GetSignal(bar);

                // Close opened
                if (IsLastPositionActive)
                {
                    CloseStops(bar);
                }
                // Open new positions
                else //if(bar > 1)
                {
                    // Buy if go up more then deltaUp
                    if (signal == SignalType.Buy)
                    {
                        BuyAtMarket(bar + 1, "Buy signal");
                        isGapProcessed = true;
                    }
                    else if (signal == SignalType.Sell)
                    {
                        ShortAtMarket(bar + 1, "Sell signal");
                        isGapProcessed = true;
                    }
                }
            }

            if (IsLastPositionActive)
                ExitAtClose(Bars.Count - 1, LastActivePosition);
        }

        // Gap information variables
        double yesterdayPrice = 0;
        double todayPrice = 0;
        DateTime gapDate = DateTime.MinValue;
        bool isGapProcessed = true;



        /// <summary>
        /// Get signal for the bar
        /// </summary>
        /// <returns></returns>
        protected SignalType GetSignal(int bar)
        {
            // Process gap
            if (!isGapProcessed)
            {
                if (todayPrice - yesterdayPrice > MinGapSize.Value)
                    return SignalType.Buy;
                else if (yesterdayPrice - todayPrice > MinGapSize.Value)
                    return SignalType.Sell;
            }
            else if (IsLastPositionActive)
            {
                // If long and gap closed
                if (LastActivePosition.PositionType == PositionType.Long
                    && Low[bar] < todayPrice - (todayPrice-yesterdayPrice) /2)
                {
                    return SignalType.Sell;
                }
                // If short and gap closed
                else if (LastActivePosition.PositionType == PositionType.Short
                    && High[bar] > todayPrice + (yesterdayPrice - todayPrice)/2)
                {
                    return SignalType.Buy;
                }
            }
            return SignalType.None;
        }

 

    }
}
