using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace Entities
{
    [XmlRoot(ElementName = "config")]
    public class Config
    {
        public Config LoadConfiguration()
        {
            try
            {
                string configXMLLocation = getRelativePath(@"XMLs\config.xml");
                XmlTextReader reader = new XmlTextReader(configXMLLocation);
                return (Config)(new XmlSerializer(typeof(Config))).Deserialize(reader);

            }
            catch (Exception)
            {
                return new Config();
            }
        }

        public string getRelativePath(string fileName)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string currentDir = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
            DirectoryInfo XMLLocation = new DirectoryInfo(
                Path.GetFullPath(Path.Combine(currentDir, fileName)));
            return XMLLocation.FullName;
        }

        [XmlElement(ElementName = "RemoteWebDriverHub")]
        public string RemoteWebDriverHub { get; set; }
        [XmlElement(ElementName = "DriverType")]
        public string DriverType { get; set; }
        [XmlElement(ElementName = "Browsers")]
        public Browsers Browsers { get; set; }
        [XmlElement(ElementName = "UseCurrentDirectory")]
        public string DownloadFolder { get; set; }
        [XmlElement(ElementName = "ScreenShotFolder")]
        public string ScreenShotFolder { get; set; }

        [XmlElement(ElementName = "FullDesktopScreenShotEnabled")]
        public string FullDesktopScreenShotEnabled { get; set; }

        [XmlElement(ElementName = "LogsLocation")]
        public string LogsLocation { get; set; }
        [XmlElement(ElementName = "ReportType")]
        public string ReportType { get; set; }

        [XmlElement(ElementName = "ReportLocation")]
        public string ReportLocation { get; set; }

        [XmlElement(ElementName = "ORLocation")]
        public string ORLocation { get; set; }

        [XmlElement(ElementName = "Applications")]
        public Applications Applications { get; set; }

    }

}