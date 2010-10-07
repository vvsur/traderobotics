using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WealthLab;
using System.Reflection;

namespace TradeRobotics.Robots
{
    /// <summary>
    /// Strategy helper. Needed for WealthLab
    /// </summary>
    public class MomentumStochHelper:StrategyHelper
    {
        /// <summary>
        /// Strategy GUID
        /// </summary>
        public override Guid ID { get { return new Guid("{D4EC4ECA-FE2A-40ED-BF60-8656A384CA8C}"); } }   
        
        /// <summary>
        /// Author
        /// </summary>
        public override string Author { get { return "Free Trader";  } }

        /// <summary>
        /// Strategy name
        /// </summary>
        public override string Name{get { return "MomentumStochastic"; }}

        /// <summary>
        /// Sescription
        /// </summary>
        public override string Description{get{return "Trade by signals from momentum and stochastic";}}        

        /// <summary>
        /// Creation date
        /// </summary>
        public override DateTime CreationDate { get { return DateTime.Parse("15.00.2010 19:37"); } }

        /// <summary>
        /// Last modified
        /// </summary>
        public override DateTime LastModifiedDate { get { return DateTime.Parse("15.00.2010 19:37"); } }

        /// <summary>
        /// Entrance type for WealthLab
        /// </summary>
        public override Type WealthScriptType
        {
            get { return typeof(MomentumStoch); }
        }
    }
}
