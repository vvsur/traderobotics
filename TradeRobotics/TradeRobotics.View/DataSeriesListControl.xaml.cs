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
using System.IO;

namespace TradeRobotics.View
{
    /// <summary>
    /// Interaction logic for DataSeriesListControl.xaml
    /// </summary>
    public partial class DataSeriesListControl : UserControl
    {
        public DataSeriesListControl()
        {
            InitializeComponent();
        }

        private void DataSeriesList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Get file names from data directory
            string[] dataFiles = Directory.GetFiles(TradeRobotics.DataProviders.DataContext.DataDirectory, "*.csv", SearchOption.TopDirectoryOnly);
            List<string> fileNames = new List<string>();
            foreach (string file in dataFiles)
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                fileNames.Add(fileName);
            }
            // Add to list
            DataSeriesList.ItemsSource = fileNames;
        }

        private void DataSeriesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SelectionChanged != null)
                SelectionChanged(this, e);
        }
        /// <summary>
        /// Selection changed event
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
    }
}
