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
    public class GapCatcherHelper:StrategyHelper
    {
        /// <summary>
        /// Strategy GUID
        /// </summary>
        public override Guid ID { get { return new Guid("{DBA939F1-B499-401E-86B1-A3E5C512D42B}"); } }   
        
        /// <summary>
        /// Author
        /// </summary>
        public override string Author { get { return "Free Trader";  } }

        /// <summary>
        /// Strategy name
        /// </summary>
        public override string Name{get { return "GapCatcher"; }}

        /// <summary>
        /// Sescription
        /// </summary>
        public override string Description{get{return "Trade on morning gaps";}}        

        /// <summary>
        /// Creation date
        /// </summary>
        public override DateTime CreationDate { get { return DateTime.Parse("01.08.2010 20:19"); } }

        /// <summary>
        /// Last modified
        /// </summary>
        public override DateTime LastModifiedDate { get { return DateTime.Parse("01.08.2010 20:19"); } }

        /// <summary>
        /// Entrance type for WealthLab
        /// </summary>
        public override Type WealthScriptType
        {
            get { return typeof(GapCatcher); }
        }
    }
}
