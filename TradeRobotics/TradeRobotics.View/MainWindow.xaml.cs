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
using Visifire.Charts;
using TradeRobotics.Model;

namespace TradeRobotics.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadBarsButton_Click(object sender, RoutedEventArgs e)
        {
            TradeRobotics.DataProviders.History.HistoryDataProvider provider = new DataProviders.History.HistoryDataProvider();
            var barCollection = provider.LoadBars("Sber", 5);
            LoadPriceChart(barCollection);
        }

        private void LoadPriceChart(BarCollection barCollection)
        {

            List<Bar> bars = barCollection.Bars;
            DataSeries ds = new DataSeries();
            ds.RenderAs = RenderAs.CandleStick;
            ds.MarkerEnabled = true;
            ds.MovingMarkerEnabled = true;
            ds.LightingEnabled = true;
            ds.LineThickness = 1.5;
            //ds.XValueType = ChartValueTypes.DateTime;
            ds.LegendText = string.Concat(barCollection.Symbol, " ", barCollection.Period);
            ds.PriceUpColor = new SolidColorBrush(Colors.Green);
            ds.PriceDownColor = new SolidColorBrush(Colors.Red);
            
            //PriceChart.AxesY[0].AxisMinimum = bars.Min(bar => bar.Low);
            //PriceChart.AxesY[0].AxisMaximum = bars.Max(bar => bar.High);
            PriceChart.AxesY[0].StartFromZero = false;
            PriceChart.AxesY[0].ViewportRangeEnabled = true;
            
            // Add points
            foreach (Bar bar in barCollection.Bars)
            {
                ds.DataPoints.Add(new DataPoint
                {
                    AxisXLabel = bar.Time.ToString("yyyy-MM-dd HH:mm"),
                    /*LabelText = "aaa",
                    LabelEnabled = true,*/
                    //XValue = bar.Time, // a DateTime value
                    YValues = new double[] { bar.Open, bar.Close, bar.High, bar.Low } // a double value
                });

            }
            
            PriceChart.Series.Add(ds);
            PriceChart.ZoomingEnabled = true;

        }

    }
}
