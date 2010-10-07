using System;
using System.Collections.Generic;
using System.Text;
using WealthLab.Extensions.Attribute;

/// Description for the Extension Manager
[assembly: ExtensionInfo(
    ExtensionType.Provider,
    "Quik Data",
    "Quik Data Provider",
    "Quik and Wealth lab integration",
    "2009.11",
    "TradeRobotics",
    "TradeRobotics.DataProviders.Quik.Resources.quik.png",
    ExtensionLicence.Freeware,
    new string[] { "TradeRobotics.DataProviders.Quik.dll" },
    MinProVersion = "5.4",
    MinDeveloperVersion = "5.4",
    PublisherUrl = "http://www2.wealth-lab.com/WL5WIKI/CommunityProvidersMain.ashx")
    ]

namespace TradeRobotics.DataProviders.Quik
{
    class EM
    {
    }
}