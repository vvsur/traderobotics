using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeRobotics;
using WealthLab.Extensions.Attribute;
using WealthLab;
using System.Diagnostics;


namespace TradeRobotics.DataProviders.Quik
{
    class QuikStaticProvider:StaticDataProvider
    {
        private BarDataStore barDataStore;

        #region Current state properties
        private bool UpdateRequired(DataSource ds, string symbol)
        {
            Trace.WriteLine("Update required()");
            return true;
            bool result = false;

            // Last update time of a symbol as reported by the helpful BarsDataStore
            DateTime updateTime = this.barDataStore.SymbolLastUpdated(symbol, ds.Scale, ds.BarInterval);

            // Update is required when symbol's date isn't current
            // As Quik's got no partial bar for today, let's not request today's data if the last updated bar is "yesterday":
            if (DateTime.Now.Date > updateTime.Date.AddDays(1))
                result = true;

            return result;
        }
 
        #endregion


        #region Implement static data provider
        public override void Initialize(IDataHost dataHost)
        {
            Trace.WriteLine("Initialize()");
            base.Initialize(dataHost);
            this.barDataStore = new BarDataStore(dataHost, this);
        }

        
        public override DataSource CreateDataSource()
        {
            Trace.WriteLine("CreateDataSource");
            if (this.Page == null)
                return null;

            DataSource ds = new DataSource(this);

            //QuikSettings settings = new QuikSettings();
            //settings.Symbols = Page.Symbols.ToString().ToUpper();

            //ds.DSString = settings.SerializeToString();

            // Main data is a tick data
            ds.Scale = BarScale.Tick;
            ds.BarInterval = 1;

            return ds;
        }

        public override void PopulateSymbols(DataSource ds, List<string> symbols)
        {
            Trace.WriteLine("PopulateSymbols()");
            // Get csv files
            List<string> availableSymbols = QuikQuotesLoader.GetHistorySymbols();
            symbols.AddRange(availableSymbols);
            if (string.IsNullOrEmpty(ds.DSString))
                return;
            QuikSettings settings = (QuikSettings)QuikSettings.DeserializeFromString(ds.DSString);
            SymbolList symbolList = new SymbolList(settings.Symbols);
            symbols.AddRange(symbolList.Items);
        }

        /// <summary>
        /// History request
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="symbol"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="maxBars"></param>
        /// <param name="includePartialBar"></param>
        /// <returns></returns>
        public override Bars RequestData(DataSource ds, string symbol, DateTime startDate, DateTime endDate, int maxBars, bool includePartialBar)
        {
            Trace.WriteLine("RequestData()");
            // History data
            return  QuikQuotesLoader.GetHistoryBars(symbol, ds, startDate, endDate);
        }

    
        #endregion


        #region Wizard
        /// Wizard pages
        private WizardPage Page;

        public override System.Windows.Forms.UserControl WizardFirstPage()
        {
            if (Page == null)
                Page = new WizardPage();

            Page.Initialize(barDataStore.RootPath);

            return Page;
        }

        public override System.Windows.Forms.UserControl WizardNextPage(System.Windows.Forms.UserControl currentPage)
        {
            return null;
        }

        public override System.Windows.Forms.UserControl WizardPreviousPage(System.Windows.Forms.UserControl currentPage)
        {
            return null;
        }        
        #endregion

        #region Provider capabilities



        // Indicates that provider supports modifying dataset composition on-the-fly
        public override bool CanModifySymbols
        {
            get
            {
                Trace.WriteLine("CanModifySymbols property");
                return true;
            }
        }

        // Indicates that provider supports deleting symbols
        public override bool CanDeleteSymbolDataFile
        {
            get
            {
                return true;
            }
        }

        // Strategy Monitor support not implemented
        public override bool CanRequestUpdates
        {
            get
            {
                Trace.WriteLine("CanRequestUpdates property");
                return true;
            }
        }

        // Only Daily bar scale is supported
        public override bool SupportsDynamicUpdate(BarScale scale)
        {
            Trace.WriteLine("SupportsDynamicUpdate property");
            return true;
            /*            if (scale == BarScale.Daily)
                            return true;
                        else
                            return false;*/
        }

        // Indicates that dataset updates are supported by provider
        public override bool SupportsDataSourceUpdate
        {
            get
            {
                Trace.WriteLine("SupportsDataSourceUpdate property");
                return true;
            }
        }

        // Indicates that provider updates are supported ("Update all data for..." in the Data Manager)
        public override bool SupportsProviderUpdate
        {
            get
            {
                Trace.WriteLine("SupportsProviderUpdate property");
                return true;
            }
        }

        #endregion

        #region Descriptive
        
        public override string FriendlyName {
			get { return "Quik Static Data"; }
		}
		
		public override string Description {
			get { return "Provides historical stock data from Quik"; }
		}
		
		public override System.Drawing.Bitmap Glyph 
        {
			get { return Properties.Resources.Quik; }
		}
        
        public override string URL {
            get { return @"http://www2.wealth-lab.com/WL5WIKI/QuikStaticProvider.ashx"; }
        }

        #endregion    
    }
}
