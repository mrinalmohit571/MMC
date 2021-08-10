using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Entities
{
    [XmlRoot(ElementName = "Application")]
    public class Application
    {
        [XmlElement(ElementName = "Name")]
        public String Name { get; set; }

        [XmlElement(ElementName = "UI")]
        public String UIURL { get; set; }

        [XmlElement(ElementName = "API")]
        public String APIURL { get; set; }
    }

    [XmlRoot(ElementName = "Applications")]
    public class Applications
    {
        [XmlElement(ElementName = "Application")]
        public List<Application> Application { get; set; }
    }

}
