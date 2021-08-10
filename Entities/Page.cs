using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Entities
{
    [XmlRoot(ElementName = "Page")]
    public class Page : IXmlSerializable
    {
        [XmlAttribute(AttributeName = "name")]
        public string PageName { get; set; }

        [XmlElement(ElementName = "Locator")]
        public Dictionary<string, Locator> LocatorList { get; set; }

        XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}