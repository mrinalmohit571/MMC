using System.Collections.Generic;
using System.Xml.Serialization;
namespace MMC.Entities
{
    [XmlRoot(ElementName = "Item")]
    public class Item
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "Title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "Section")]
        public string Section { get; set; }

        [XmlElement(ElementName = "Index")]
        public string Index { get; set; }

        [XmlElement(ElementName = "Scale")]
        public string Scale { get; set; }

        [XmlElement(ElementName = "PostCode")]
        public string PostCode { get; set; }

        [XmlElement(ElementName = "CountryCode")]
        public string CountryCode { get; set; }

        [XmlElement(ElementName = "CountryName")]
        public string CountryName { get; set; }

        [XmlElement(ElementName = "ResponseTime")]
        public int ResponseTime { get; set; }
    }

    [XmlRoot(ElementName = "SearchData")]
    public class SearchData
    {
        [XmlElement(ElementName = "Item")]
        public List<Item> Item { get; set; }
    }
}
