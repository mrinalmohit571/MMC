using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace MMC.Utils
{
    public class XMLUtils
    {

        public static Object LoadXMLObject(Object CastObject, string XMLName)
        {
            object EntityVal;
            try
            {
                string XMLLocation = GetRelativePath(@"XMLs\" + XMLName);
                XmlSerializer serializer = new XmlSerializer(CastObject.GetType());

                using (FileStream fs = new FileStream(XMLLocation, FileMode.Open, FileAccess.Read))
                {
                    EntityVal = serializer.Deserialize(fs) as Object;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Issue in loading xml:" + XMLName + " Exception:" + ex);
                throw new Exception("Issue in loading xml:" + XMLName);
            }
            return EntityVal;
        }

        public static string GetRelativePath(string fileName)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string currentDir = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
            DirectoryInfo XMLLocation = new DirectoryInfo(
                Path.GetFullPath(Path.Combine(currentDir, fileName)));
            return XMLLocation.FullName;
        }


    }

}
