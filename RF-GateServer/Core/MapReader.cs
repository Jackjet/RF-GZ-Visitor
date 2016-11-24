using Common;
using RF_GateServer.Gate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace RF_GateServer.Core
{
    sealed class MapReader
    {
        private static readonly string filePath = "comserver.xml";

        private const string xPathchannel = "/channels/channel";
        private const string xPathchannels = "/channels";

        public static ObservableCollection<Channel> Read()
        {
            XElement doc = XElement.Load(filePath);
            var channelList = (from n in doc.Elements("channel") select n).ToList();
            return ConvertToList(channelList);
        }

        private static ObservableCollection<Channel> ConvertToList(List<XElement> nodeList)
        {
            ObservableCollection<Channel> list = new ObservableCollection<Channel>();
            foreach (XElement element in nodeList)
            {
                var channel = ElementToChannel(element);
                list.Add(channel);
            }
            return list;
        }

        private static Channel ElementToChannel(XElement element)
        {
            Channel channel = new Core.Channel();
            channel.Index = element.Element("index").Value;
            channel.Name = element.Element("name").Value;
            channel.Area = element.Element("areaName").Value;
            channel.ItemId = element.Element("itemId").Value;
            channel.CommunityId = element.Element("communityId").Value;

            var inIp = element.Element("in").Value;
            channel.InIp = inIp;

            var outIp = element.Element("out").Value;
            channel.OutIp = outIp;

            var gateIp = element.Element("gate").Value;
            channel.GateIp = gateIp;
            return channel;
        }

        public static void Save(Channel channel)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            var root = doc.SelectSingleNode(xPathchannels);

            XmlNode server = doc.CreateNode(XmlNodeType.Element, "channel", "");
            root.AppendChild(server);

            XmlElement nodeIndex = doc.CreateElement("index");
            nodeIndex.InnerText = channel.Index;
            server.AppendChild(nodeIndex);

            XmlElement nodeName = doc.CreateElement("name");
            nodeName.InnerText = channel.Name;
            server.AppendChild(nodeName);

            XmlElement nodeAreaName = doc.CreateElement("areaName");
            nodeAreaName.InnerText = channel.Area;
            server.AppendChild(nodeAreaName);

            XmlElement nodeItemId = doc.CreateElement("itemId");
            nodeItemId.InnerText = channel.ItemId;
            server.AppendChild(nodeItemId);

            XmlElement nodecommunityId = doc.CreateElement("communityId");
            nodecommunityId.InnerText = channel.CommunityId;
            server.AppendChild(nodecommunityId);

            XmlElement nodeIn = doc.CreateElement("in");
            nodeIn.InnerText = channel.InIp;
            server.AppendChild(nodeIn);

            XmlElement nodeOut = doc.CreateElement("out");
            nodeOut.InnerText = channel.OutIp;
            server.AppendChild(nodeOut);

            XmlElement nodeGate = doc.CreateElement("gate");
            nodeGate.InnerText = channel.GateIp;
            server.AppendChild(nodeGate);

            doc.Save(filePath);
        }

        public static void Delete(string index)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            var parent = doc.SelectSingleNode("/channels");
            var path = "/channels/channel[index='" + index + "']";
            var node = doc.SelectSingleNode(path);
            if (node != null)
            {
                parent.RemoveChild(node);
            }
            doc.Save(filePath);
        }
    }
}
