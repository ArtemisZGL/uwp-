using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tally.model
{
    class TileService
    {
        public static Windows.Data.Xml.Dom.XmlDocument CreateTiles(tallyitems item)
        {
            string dateText = item.date.ToString();
            XDocument xDoc = new XDocument(
                new XElement("tile", new XAttribute("version", 3),
                new XElement("visual",
                    new XElement("binding", new XAttribute("displayName", item.first_label), new XAttribute("template", "TileSmall"),
                    new XElement("image", new XAttribute("placement", "background"),
                                        new XAttribute("src", "Assets/rs.jpg")),
                    new XElement("group",
                        new XElement("subgroup",
                            new XElement("text", item.first_label, new XAttribute("hint-style", "caption")),
                            new XElement("text", item.money, new XAttribute("hint-style", "caption"), new XAttribute("hint-wrap", "true"))
                        )
                    )
                    ),

                    new XElement("binding", new XAttribute("displayName", item.first_label), new XAttribute("template", "TileMedium"),
                     new XElement("image", new XAttribute("placement", "background"),
                                        new XAttribute("src", "Assets/rs.jpg")),
                    new XElement("group",
                        new XElement("subgroup",
                            new XElement("text", item.first_label, new XAttribute("hint-style", "caption")),
                            new XElement("text", dateText, new XAttribute("hint-style", "caption")),
                            new XElement("text", item.money, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                        )

                    )
                    ),

                    new XElement("binding", new XAttribute("displayName", item.first_label), new XAttribute("template", "TileWide"),
                    new XElement("image", new XAttribute("placement", "background"),
                                        new XAttribute("src", "Assets/rs.jpg")),
                    new XElement("group",
                        new XElement("subgroup",
                            new XElement("text", item.first_label, new XAttribute("hint-style", "caption")),
                            new XElement("text", item.second_label, new XAttribute("hint-style", "caption")),
                            new XElement("text", dateText, new XAttribute("hint-style", "caption")),
                            new XElement("text", item.money + "元", new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                        )
                    )
                    )
                )
            ));

            Windows.Data.Xml.Dom.XmlDocument xmlDoc = new Windows.Data.Xml.Dom.XmlDocument();
            xmlDoc.LoadXml(xDoc.ToString());
            return xmlDoc;

        }
    }
}
