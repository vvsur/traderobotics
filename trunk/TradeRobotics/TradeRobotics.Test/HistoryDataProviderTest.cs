using TradeRobotics.DataProviders.History;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TradeRobotics.Model;

namespace TradeRobotics.Test
{
    
    
    /// <summary>
    ///This is a test class for HistoryDataProviderTest and is intended
    ///to contain all HistoryDataProviderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HistoryDataProviderTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            // Set data directory to test project data folder
            TradeRobotics.DataProviders.DataContext.DataDirectory = @"..\..\..\TradeRobotics.Test\Data\";
        }
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for LoadBars
        ///</summary>
        [TestMethod()]
        public void LoadBarsTest()
        {
            try
            {
                HistoryDataProvider target = new HistoryDataProvider(); // TODO: Initialize to an appropriate value
                string symbol = "sber"; // TODO: Initialize to an appropriate value
                int period = 5; // TODO: Initialize to an appropriate value

                BarCollection bars;
                bars = target.LoadBars(symbol, period);
                Assert.AreNotEqual(bars, null);
                Assert.AreNotEqual(bars.Bars.Count, 0);
                Assert.AreNotEqual(bars.Symbol, null);
                Assert.AreNotEqual(bars.Symbol, string.Empty);
                Assert.AreEqual(bars.Period, period);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            
        }
    }
}
