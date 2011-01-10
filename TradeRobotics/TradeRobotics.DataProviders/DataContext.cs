using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace TradeRobotics.DataProviders
{
    /// <summary>
    /// General data settings
    /// </summary>
    public static class DataContext
    {
        /// <summary>
        /// NumberFormatInfo for  "." separator in price values
        /// </summary>
        public static NumberFormatInfo NumberFormatInfo
        {
            get
            {
                if (_numberFormatInfo == null)
                {
                    lock (lockObject)
                    {
                        if (_numberFormatInfo == null)
                        {
                            // Set decimal separator to "."
                            System.Globalization.CultureInfo ci =
                               System.Globalization.CultureInfo.InstalledUICulture;
                            System.Globalization.NumberFormatInfo ni = null;
                            _numberFormatInfo = (System.Globalization.NumberFormatInfo)
                               ci.NumberFormat.Clone();
                            _numberFormatInfo.NumberDecimalSeparator = ".";
                            _numberFormatInfo.CurrencyDecimalSeparator = ".";
                        }
                    }
                }
                return _numberFormatInfo;
            }
        }
        private static NumberFormatInfo _numberFormatInfo = null;
        private static object lockObject = new object();
        /// <summary>
        /// Delimiter in row
        /// </summary>
        public static char Delimiter = ';';
        public static string DataDirectory = @".\Data\";
        public static void Init()
        {
            DataDirectory = string.Concat(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "\\Data\\");
        }
    }
}
