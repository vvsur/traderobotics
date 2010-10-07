using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Globalization;
using WealthLab;

namespace TradeRobotics.DataProviders.Quik
{
    /// <summary>
    /// One bar values
    /// </summary>
    public class QuikBar
    {
        public QuikBar() { }
        public QuikBar(QuikQuote quote)
        {
            Symbol = quote.Symbol;
            Volume = quote.Volume;
            Open = quote.Close;
            High = quote.Close;
            Low = quote.Close;
            Close = quote.Close;
            OpenTime = quote.Time;
            CloseTime = quote.Time;
        }
        public QuikBar(string data)
        {
            NumberFormatInfo numberFormatInfo = QuikDdeServer.NumberFormatInfo;

            string[] dataArray = data.Split(QuikDdeServer.Delimiter);
            // Symbol
            Symbol = dataArray[0];

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
            CloseTime = new DateTime(year, month, day, hour, minute, second);

            // OHLCV

            string openString = dataArray[4];
            Open = double.Parse(openString, numberFormatInfo);
            string highString = dataArray[5];
            High = double.Parse(highString, numberFormatInfo);
            string lowString = dataArray[6];
            Low = double.Parse(lowString, numberFormatInfo);
            string closeString = dataArray[7];
            Close = double.Parse(closeString, numberFormatInfo);
            string volString = dataArray[8];
            Volume = double.Parse(volString, numberFormatInfo);
        }

        //public QuikBar(string data, NumberFormatInfo numberFormatInfo)
        //{
        //    string[] dataArray = data.Split(QuikDdeServer.Delimiter);
        //    // Symbol
        //    Symbol = dataArray[0];

        //    // Date
        //    string dateString = dataArray[2];
        //    int year = int.Parse(dateString.Substring(0, 4));
        //    int month = int.Parse(dateString.Substring(4, 2));
        //    int day = int.Parse(dateString.Substring(6, 2));
        //    // Time
        //    string timeString = dataArray[3];
        //    int hour = int.Parse(timeString.Substring(0, 2));
        //    int minute = int.Parse(timeString.Substring(2, 2));
        //    int second = int.Parse(timeString.Substring(4, 2));
        //    // Set time
        //    Time = new DateTime(year, month, day, hour, minute, second);

        //    // OHLCV

        //    string closeString = dataArray[4];
        //    Clos = double.Parse(openString, numberFormatInfo);
        //    string highString = dataArray[5];
        //    High = double.Parse(highString, numberFormatInfo);
        //    string lowString = dataArray[6];
        //    Low = double.Parse(lowString, numberFormatInfo);
        //    string closeString = dataArray[7];
        //    Close = double.Parse(closeString, numberFormatInfo);
        //    string volString = dataArray[8];
        //    Volume = double.Parse(volString, numberFormatInfo);
        //}

        public void Update(QuikQuote quote)
        {
            // ToDo: find out, maybe add, not max
            Volume = Math.Max(quote.Volume, Volume);
//            Low = High = Close = quote.Close;
            Low = Math.Min(quote.Close, Low);
            High = Math.Max(quote.Close, High);
            //Close = quote.Price;
            // If new quote is a first quote, update open time and price
            if (CloseTime.Ticks < quote.Time.Ticks)
            {
                CloseTime = quote.Time;
                Close = quote.Close;
            }
            // If new quote is a last quote, update close time and price
            if (OpenTime.Ticks > quote.Time.Ticks)
            {
                OpenTime = quote.Time;
                Open = quote.Close;
            }

        }


        public DateTime CloseTime = DateTime.MinValue;
        public DateTime OpenTime = DateTime.MaxValue;
        public string Symbol;
        public double Open;
        public double Low = double.MaxValue;
        public double High = double.MinValue;
        public double Close;
        public double Volume;

        
        /// <summary>
        /// For serialization
        /// </summary>
        /// <returns></returns>
        public static string TableHeader
        {
            get { return "<TICKER>,<PER>,<DATE>,<TIME>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOL>"; }
        }

        /// <summary>
        /// Serialize to history file
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            // Symbol
            sb.Append(Symbol); sb.Append(QuikDdeServer.Delimiter);
            // Per
            sb.Append(1); sb.Append(QuikDdeServer.Delimiter);
            // DateTime
            sb.Append(CloseTime.Year); sb.Append(CloseTime.Month); sb.Append(CloseTime.Day); sb.Append(QuikDdeServer.Delimiter);
            sb.Append(CloseTime.Hour); sb.Append(CloseTime.Minute); sb.Append(CloseTime.Second); sb.Append(QuikDdeServer.Delimiter);
            // Price
            sb.Append(Open); sb.Append(QuikDdeServer.Delimiter);
            sb.Append(High); sb.Append(QuikDdeServer.Delimiter);
            sb.Append(Low); sb.Append(QuikDdeServer.Delimiter);
            sb.Append(Close); sb.Append(QuikDdeServer.Delimiter);
            // Volume
            sb.Append(Volume); sb.Append(QuikDdeServer.Delimiter);

            return sb.ToString();
        }

    }
}
