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

namespace TradeRobotics.View
{
    /// <summary>
    /// Interaction logic for TestProgressControl.xaml
    /// </summary>
    public partial class TestProgressControl : UserControl
    {
        public TestProgressControl()
        {
            InitializeComponent();
        }

        public double Value
        {
            get
            {
                return ProgressBar1.Value;
            }
            set
            {
                ProgressBar1.Value = value;
            }
        }

    }
}
