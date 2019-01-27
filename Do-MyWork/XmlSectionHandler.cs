using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

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
