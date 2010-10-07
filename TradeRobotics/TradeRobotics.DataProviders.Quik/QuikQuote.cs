using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace TradeRobotics.DataProviders.Quik
{
    
    /// <summary>
    /// One quote item from quik
    /// </summary>
    public class QuikQuote
    {
#region Data
        public DateTime Time;
        public string Symbol;
        //public double Ask;
        //public double Bid;
        //public double Price;
        public double Close;
        public double Volume;
#endregion
        
//        public static char Delimiter = ';';
        public QuikQuote() { }

        /// <summary>
        /// Construct from DDE data item
        /// </summary>
        /// <param name="data"></param>
        /// <param name="numberFormatInfo"></param>
        public QuikQuote( object[] dataArray)
        {
            // Date and time
            Time = DateTime.Parse(dataArray[0].ToString());

            Symbol = dataArray[1].ToString();
             
            //if(!string.IsNullOrEmpty(dataArray[1].ToString()))
            //    Ask = double.Parse(dataArray[1].ToString());
            //if (!string.IsNullOrEmpty(dataArray[2].ToString()))
            //    Bid = double.Parse(dataArray[2].ToString());
            //if (!string.IsNullOrEmpty(dataArray[3].ToString()))
            //    Price = double.Parse(dataArray[3].ToString());
            if (!string.IsNullOrEmpty(dataArray[2].ToString()))
                Close = double.Parse(dataArray[2].ToString());

            if (!string.IsNullOrEmpty(dataArray[3].ToString()))
                Volume = double.Parse(dataArray[3].ToString());
        }

        /// <summary>
        /// Constructor from string
        /// </summary>
        /// <param name="dataString"></param>
        public QuikQuote(string dataString):this(dataString.Split(QuikDdeServer.Delimiter)) {}
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="data"></param>
        public QuikQuote(string[] dataArray)
        {
            NumberFormatInfo numberFormatInfo = QuikDdeServer.NumberFormatInfo;
            
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
            Time = new DateTime(year, month, day, hour, minute, second);

            // Close
            string closeString = dataArray[4];
            Close = double.Parse(closeString, numberFormatInfo);
            // Volume
            string volString = dataArray[5];
            Volume = double.Parse(volString, numberFormatInfo);
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
            // DateTime
            sb.Append(Time.ToString()); sb.Append(QuikDdeServer.Delimiter);
            // Price
            //sb.Append(Ask); sb.Append(QuikDdeServer.Delimiter);
            //sb.Append(Bid); sb.Append(QuikDdeServer.Delimiter);
            //sb.Append(Price); sb.Append(QuikDdeServer.Delimiter);
            sb.Append(Close); sb.Append(QuikDdeServer.Delimiter);
            sb.Append(Volume); sb.Append(QuikDdeServer.Delimiter);
            return sb.ToString();
        }

    }
}
