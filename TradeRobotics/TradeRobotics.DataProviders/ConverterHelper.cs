using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeRobotics.Model;
using System.Globalization;

namespace TradeRobotics.DataProviders
{
    public static class ConverterHelper
    {
        /// <summary>
        /// Get bar from line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Bar LoadBar(string data)
        {
            NumberFormatInfo numberFormatInfo = DataContext.NumberFormatInfo;
            Bar bar = new Bar();


            string[] dataArray = data.Split(DataContext.Delimiter);
            // Symbol
            bar.Symbol = dataArray[0];

            // Date
            string dateString = dataArray[2];
            int year = int.Parse(dateString.Substring(0, 4));
            int month = int.Parse(dateString.Substring(4, 2));
            int day = int.Parse(dateString.Substring(6, 2));
            // Time
            string timeString = dataArray[3];
            int hour = int.Parse(timeString.Substring(0, 2));
            int minute = int.Parse(timeString.Substring(2, 2));
            int second = int.Parse(timeString.Substring(4, 2));
            // Set time
            bar.Time = new DateTime(year, month, day, hour, minute, second);

            // OHLCV
            string openString = dataArray[4];
            bar.Open = double.Parse(openString, numberFormatInfo);
            string highString = dataArray[5];
            bar.High = double.Parse(highString, numberFormatInfo);
            string lowString = dataArray[6];
            bar.Low = double.Parse(lowString, numberFormatInfo);
            string closeString = dataArray[7];
            bar.Close = double.Parse(closeString, numberFormatInfo);
            string volString = dataArray[8];
            bar.Volume = int.Parse(volString, numberFormatInfo);

            return bar;

        }    
    }
}
