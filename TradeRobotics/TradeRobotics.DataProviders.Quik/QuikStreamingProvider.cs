using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Timers;
using WealthLab;

namespace TradeRobotics.DataProviders.Quik
{
    /// <summary>
    /// Streaming data provider. Gets data from quik to wealth lab
    /// </summary>
    public  class QuikStreamingProvider:StreamingDataProvider
    {
        #region Descriptive
        public override string FriendlyName
        {
            get { return "Quik Streaming Data"; }
        }

        public override string Description
        {
            get { return "Provides streaming stock data from Quik"; }
        }

        public override System.Drawing.Bitmap Glyph
        {
            get { return Properties.Resources.Quik; }
        }
        #endregion

        private QuikDdeServer ddeServer;
        private string ddeServerName = "TradeRobot";
        private QuikStaticProvider staticProvider = new QuikStaticProvider();

        
        public override bool IsConnected
        {
            get 
            {
                return ddeServer.IsRegistered;
            }
        }

        protected override void Subscribe(string symbol)
        {
            Trace.WriteLine(string.Format("Subscribe({0})", symbol));
        }

        protected override void UnSubscribe(string symbol)
        {
            Trace.WriteLine(string.Format("UnSubscribe({0})", symbol));
        }

        public override void Initialize(IDataHost dataHost)
        {
            QuikQuotesLoader.StreamingProvider = this;
            Trace.WriteLine("Initialize(IDataHost)");
            base.Initialize(dataHost);
            // Create dde server
            ddeServer = new QuikDdeServer(ddeServerName);
            ddeServer.Register();
            
        }

        public override StaticDataProvider GetStaticProvider()
        {
            Trace.WriteLine("GetStaticProvider()");
            if (staticProvider == null)
                staticProvider = new QuikStaticProvider();
            return staticProvider;
        }
        public override void ConnectStreaming(IConnectionStatus connStatus)
        {
            Trace.WriteLine("ConnectStreaming()");
            base.ConnectStreaming(connStatus);
            connStatus.StatusUpdate(ConnStatus.OK, 1, "Connected");
        }
        public override void DisconnectStreaming()
        {
            Trace.WriteLine("Disconnect Streaming()");
            base.DisconnectStreaming();
        }
        public override void DisconnectStreaming(IConnectionStatus connStatus)
        {
            Trace.WriteLine("Disconnect Streaming(IConnectionStatus)");
            base.DisconnectStreaming(connStatus);
        }
        public override string URL
        {
            get
            {
                Trace.WriteLine("Url Property");
                return base.URL;
            }
        }
    }
}
