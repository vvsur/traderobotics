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
    public class VertexExplorerHelper:StrategyHelper
    {
        /// <summary>
        /// Strategy GUID
        /// </summary>
        public override Guid ID { get { return new Guid("{259200FC-B27E-4b54-BD9B-135463957433}"); } }   
        
        /// <summary>
        /// Author
        /// </summary>
        public override string Author { get { return "Free Trader";  } }

        /// <summary>
        /// Strategy name
        /// </summary>
        public override string Name{get { return "VertexExplorer"; }}

        /// <summary>
        /// Sescription
        /// </summary>
        public override string Description{get{return "Find last vertex, join the movement";}}        

        /// <summary>
        /// Creation date
        /// </summary>
        public override DateTime CreationDate { get { return DateTime.Parse("02.02.2010 21:13"); } }

        /// <summary>
        /// Last modified
        /// </summary>
        public override DateTime LastModifiedDate { get { return DateTime.Parse("02.02.2010 21:13"); } }

        /// <summary>
        /// Entrance type for WealthLab
        /// </summary>
        public override Type WealthScriptType
        {
            get { return typeof(VertexExplorer); }
        }
    }
}
