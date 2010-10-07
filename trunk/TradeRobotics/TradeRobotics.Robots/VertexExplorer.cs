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
    /// Robot class
    /// </summary>
    public class VertexExplorer : WealthScriptBase
    {
        // Input parameters
        private StrategyParameter deltaDown;
        private StrategyParameter deltaUp;
        private StrategyParameter priceMaPeriod;
        private StrategyParameter trendMaPeriod;

        // Maximum, mininum values
        private double lastMax = double.MinValue;
        private int lastMaxBar = 0;
        private double lastMin = double.MaxValue;
        private int lastMinBar = 0;

        // Indicators
        private SMA smaPrice;
        private SMA smaTrend;



        /// <summary>
        /// Constructor
        /// </summary>
        public VertexExplorer()
            : base()
        {

            // Base class parameters.
            /*            StopLoss = CreateParameter("StopLoss", 0.75, 0, 5, 0.1);
                        TakeProfit = CreateParameter("TakeProfit", 3.56, 0, 5, 0.1);
                        // Trailing stop sets when price is reached TakeProfit. If zero, close on TakeProfit
                        TrailingStop = CreateParameter("TrailingStop", 3.63, 0, 5, 0.1);
                        */
            deltaDown = CreateParameter("DeltaDown", 0.53, 0.1, 1, 0.05);
            deltaUp = CreateParameter("DeltaUp", 0.98, 0.1, 1, 0.05);
            priceMaPeriod = CreateParameter("PriceMAPeriod", 4, 1, 10, 1);
            trendMaPeriod = CreateParameter("TrendMAPeriod", 174, 30, 500, 10);


        }

        /// <summary>
        /// Main function - robot Entry point
        /// </summary>
        protected override void Execute()
        {
            base.Execute();

            // Create sma
            smaPrice = new SMA(Bars.Close, priceMaPeriod.ValueInt, "Moving Average to smooth price volatility");
            PlotSeries(PricePane, smaPrice, Color.Green, LineStyle.Solid, 1);
            smaTrend = new SMA(Bars.Close, trendMaPeriod.ValueInt, "Trend Moving Average");
            PlotSeries(PricePane, smaTrend, Color.Brown, LineStyle.Solid, 1);

            for (int bar = 1; bar < Bars.Count; bar++)
            {
                // Calculate signal
                SignalType signal = GetSignal(bar);

                // Close opened
                if (IsLastPositionActive)
                {
                    // If isn't closed by stops
                    if (IsLastPositionActive)
                    {
                        //CheckStops(bar);

                        // Close long if go below low level
                        if (LastPosition.PositionType == PositionType.Long
                            && signal == SignalType.Sell)
                        {
                            ExitAtMarket(bar + 1, LastPosition, "Sell signal");
                            ShortAtMarket(bar + 1, "Sell signal");
                        }
                        // Close short if go upper high level
                        else if (LastPosition.PositionType == PositionType.Short
                            && signal == SignalType.Buy)
                        {
                            ExitAtMarket(bar + 1, LastPosition, "Buy signal");
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
                    else if (signal == SignalType.Sell)
                    {
                        ShortAtMarket(bar + 1, "Sell signal");
                    }
                }
            }

            if (IsLastPositionActive)
                ExitAtClose(Bars.Count - 1, LastActivePosition);
        }

        /// <summary>
        /// Get trend type for the bar
        /// </summary>
        /// <param name="bar"></param>
        /// <returns></returns>
        protected TrendType GetTrendType(int bar)
        {
            TrendType result = TrendType.Unknown;
            if (bar == 0)
                result = TrendType.Unknown;
            else if (smaTrend[bar - 1] < smaTrend[bar])
                result = TrendType.Bull;
            else if (smaTrend[bar - 1] > smaTrend[bar])
                result = TrendType.Bear;
            return result;
        }

        /// <summary>
        /// Get signal for the bar
        /// </summary>
        /// <returns></returns>
        protected SignalType GetSignal(int bar)
        {
            SignalType result = SignalType.None;
            //double price = Close[bar];
            double price = smaPrice[bar];
            TrendType trendType = GetTrendType(bar);

            // Move last maximum, minimum
            if (price < lastMin)
            {
                lastMin = price;
                lastMinBar = bar;
            }
            if (price > lastMax)
            {
                lastMax = price;
                lastMaxBar = bar;
            }

            // Buy if go up more then deltaUp
            if (price - lastMin >= deltaUp.Value
                && trendType == TrendType.Bear
                && (!IsLastPositionActive ||
                        (IsLastPositionActive
                        && LastActivePosition.PositionType != PositionType.Long)))
            {
                result = SignalType.Buy;
                AnnotateBar("Min", lastMinBar, true, Color.DarkRed);
                AnnotateBar("Buy", bar, true, Color.DarkBlue);
                lastMax = lastMin = price;
            }
            // Sell if go down more then deltaDown
            else if (lastMax - price >= deltaDown.Value
                && trendType == TrendType.Bull
                && (!IsLastPositionActive ||
                        (IsLastPositionActive
                        && LastActivePosition.PositionType != PositionType.Short)))
            {
                result = SignalType.Sell;
                AnnotateBar("Max", lastMaxBar, true, Color.DarkBlue);
                AnnotateBar("Sell", bar, true, Color.DarkRed);
                lastMax = lastMin = price;
            }

            return result;
        }
    }
}
