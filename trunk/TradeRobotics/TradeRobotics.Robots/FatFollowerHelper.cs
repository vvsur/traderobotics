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
    public class FatFollowerHelper:StrategyHelper
    {
        /// <summary>
        /// Strategy GUID
        /// </summary>
        public override Guid ID { get { return new Guid("{119E04FB-08C8-4d8d-9ED9-EBD8E196A638}"); } }   
        
        /// <summary>
        /// Author
        /// </summary>
        public override string Author { get { return "Free Trader";  } }

        /// <summary>
        /// Strategy name
        /// </summary>
        public override string Name{get { return "FatFollower"; }}

        /// <summary>
        /// Sescription
        /// </summary>
        public override string Description{get{return "Follow Level2 Fat bull or bear";}}        

        /// <summary>
        /// Creation date
        /// </summary>
        public override DateTime CreationDate { get { return DateTime.Parse("05.04.2010 21:13"); } }

        /// <summary>
        /// Last modified
        /// </summary>
        public override DateTime LastModifiedDate { get { return DateTime.Parse("05.03.2010 21:13"); } }

        /// <summary>
        /// Entrance type for WealthLab
        /// </summary>
        public override Type WealthScriptType
        {
            get { return typeof(FatFollower); }
        }
    }
}
