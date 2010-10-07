using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Xml.Serialization;

namespace TradeRobotics.DataProviders.Quik.Dom
{
    /// <summary>
    /// Dom order info
    /// </summary>
    public class Order
    {
        public double Volume;
        public double Price;
        public OrderType OrderType;


        /// <summary>
        /// Constructor from string
        /// </summary>
        /// <param name="dataString"></param>
        public Order(string dataString) : this(dataString.Split(QuikDdeServer.Delimiter)) { }
        
        /// <summary>
        /// Constructor from array
        /// </summary>
        /// <param name="dataArray"></param>
        public Order(object[] dataArray)
        {
            NumberFormatInfo numberFormatInfo = QuikDdeServer.NumberFormatInfo;

            // If ask is empty, this request is bid
            string askVolume = dataArray[0].ToString();
            if (!string.IsNullOrEmpty(askVolume))
            {
                OrderType = OrderType.Ask;
                Volume = double.Parse(askVolume, numberFormatInfo);
            }
            // If ask is not empty, this request is ask
            else
            {
                OrderType = OrderType.Bid;
                Volume = double.Parse(dataArray[2].ToString(), numberFormatInfo);
            }
            //Price = double.Parse(dataArray[1].ToString(), numberFormatInfo);
            Price = Convert.ToDouble(dataArray[1], numberFormatInfo);
        }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public Order() { }


   
    }
}
