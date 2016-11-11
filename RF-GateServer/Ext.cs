using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RF_GateServer
{
    static class Ext
    {
        public static XmlElement CreateElement(this XmlDocument doc, string name, string text)
        {
            XmlElement element = doc.CreateElement(name);
            element.InnerText = text;
            return element;
        }
    }
}
