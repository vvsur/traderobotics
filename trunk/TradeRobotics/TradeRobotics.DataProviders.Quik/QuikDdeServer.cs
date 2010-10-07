using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDde.Server;
using System.Timers;
using System.Diagnostics;
using FTFHelper;
using System.Text.RegularExpressions;
using System.Globalization;

namespace TradeRobotics.DataProviders.Quik
{

        /// <summary>
        /// Dde server. Function OnPoke gets data from quick
        /// Pass data to QuikLoader or DomLoader(загрузка стакана)
        /// </summary>
        public class QuikDdeServer : DdeServer
        {

            public QuikDdeServer(string service)
                : base(service)
            {
            }

            /// <summary>
            /// Catch DDE Data
            /// </summary>
            /// <param name="conversation"></param>
            /// <param name="item"></param>
            /// <param name="data"></param>
            /// <param name="format"></param>
            /// <returns></returns>
            protected override PokeResult OnPoke(DdeConversation conversation, string item, byte[] data, int format)
            {
                //QuikLoader.StaticDataLoadedSync.WaitOne();
                object[][] table = XLTable.Cells(data);
                
                
                string symbol = null; string dataType = null;
                GetDataType(conversation, out symbol, out dataType);
                switch (dataType.ToLower())
                {
                    case "dom":
                        // Dom data load
                        Dom.DomLoader.AddSnapShotFromDde(symbol, table);
                        break;

                    case "quotes":
                    default:
                        // Load quotes data
                        QuikQuotesLoader.AddQuotesFromDde(table);
                        break;
                }
                
                Trace.WriteLine("OnPoke:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString()
                    + " Item='" + item + "'"
                    + " Data=" + data.Length.ToString()
                    + " Format=" + format.ToString());


                // Static data is loaded sygnal
//                QuikLoader.StaticDataLoadedSync.Set();


                // Tell the client that the data was processed.
                return PokeResult.Processed;
            }

            /// <summary>
            /// Get Symbol and data type (quotes or dom) from dde conversation object
            /// </summary>
            /// <returns></returns>
            protected void GetDataType(DdeConversation conversation, out string symbol, out string dataType)
            {
                // Check format like "[SBER03]Quotes"
                Regex regex = new Regex(@"\[(?<Symbol>.*)\](?<DataType>.*)");
                Match match = regex.Match(conversation.Topic);
                symbol = match.Groups["Symbol"].Value;
                dataType = match.Groups["DataType"].Value;

                // If no symbol, format like "SBER03" 
                if (string.IsNullOrEmpty(symbol))
                    symbol = conversation.Topic;
            }


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

        } // class
}
