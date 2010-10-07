using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace TradeRobotics.DataProviders.Quik.Dom
{
    /// <summary>
    /// Depth of Market snapshot
    /// </summary>
    public class Level2
    {
        /// <summary>
        /// Constructor from DDE table
        /// </summary>
        /// <param name="table"></param>
        public Level2(string symbol, object[][] table, DateTime time):this()
        {
            Symbol = symbol;
            Time = time;
            // Parse rows to bars
            foreach (object[] row in table)
            {
                try
                {
                    Order order = new Order(row);
                    Orders.Add(order);
                }
                catch (Exception ex)
                {
                    // First row Exception when processing table with header
                }
            }
        }

        /// <summary>
        /// Parameterless
        /// </summary>
        public Level2()
        {
            Orders = new List<Order>();
        }

        public string Symbol {get;set;}
        
        /// <summary>
        /// Snapshot date
        /// </summary>
        public DateTime Time {get;set;}
        
        /// <summary>
        /// Orders list in one level 2 market snapshot
        /// </summary>
        public List<Order> Orders = new List<Order>();

    }
}
