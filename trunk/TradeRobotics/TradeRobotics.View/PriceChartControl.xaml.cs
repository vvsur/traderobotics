using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TradeRobotics.Model;
using Visifire.Charts;

namespace TradeRobotics.View
{
    /// <summary>
    /// Interaction logic for PriceChartControl.xaml
    /// </summary>
    public partial class PriceChartControl : UserControl
    {
        public PriceChartControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Show chart with data series
        /// </summary>
        /// <param name="dataSeries"></param>
        public void LoadPriceChart(TradeRobotics.Model.StockDataSeries dataSeries)
        {

            //List<Bar> bars = dataSeries.Bars;
            Visifire.Charts.DataSeries ds = new DataSeries();
            ds.RenderAs = RenderAs.CandleStick;
            ds.MarkerEnabled = true;
            ds.MovingMarkerEnabled = true;
            ds.LightingEnabled = true;
            ds.LineThickness = 1.5;
            //ds.XValueType = ChartValueTypes.DateTime;
            ds.LegendText = string.Concat(dataSeries.Symbol, " ", dataSeries.Period);
            ds.PriceUpColor = new SolidColorBrush(Colors.Green);
            ds.PriceDownColor = new SolidColorBrush(Colors.Red);

            //PriceChart.AxesY[0].AxisMinimum = bars.Min(bar => bar.Low);
            //PriceChart.AxesY[0].AxisMaximum = bars.Max(bar => bar.High);
            PriceChart.AxesY[0].StartFromZero = false;
            PriceChart.AxesY[0].ViewportRangeEnabled = true;

            // Add points
            //for (int i = 0; i < dataSeries.Count; i++)
            foreach (Bar bar in dataSeries.Bars)
            {
                ds.DataPoints.Add(new DataPoint
                {
                    AxisXLabel = bar.Time.ToString("yyyy-MM-dd HH:mm"),
                    ToolTipText="aa\nbb",
                    //XValue = bar.Time, // a DateTime value
                    YValues = new double[] { bar.Open, bar.Close, bar.High, bar.Low }
                    //dataSeries.Close[i], dataSeries.High[i], dataSeries.Low[i] } // a double value
                });
            }

            PriceChart.Series.Add(ds);
            PriceChart.ZoomingEnabled = true;

        }
    }
}
