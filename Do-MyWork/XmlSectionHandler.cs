using System.Configuration;

namespace Do_MyWork
{
    public class XmlSectionHandler : IConfigurationSectionHandler
    {
        public object Create(
             object parent,
             object configContext,
             System.Xml.XmlNode section)
        {
            return section;
        }
    }
}
