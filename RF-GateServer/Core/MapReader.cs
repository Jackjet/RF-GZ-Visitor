using Common;
using RF_GateServer.Gate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RF_GateServer.Core
{
    sealed class MapReader
    {
        private static readonly string filename = "comserver.xml";

        public static List<Channel> Read()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            var xPath = "/servers/server";
            var servers = doc.SelectNodes(xPath);
            if (servers.Count == 0)
                return new List<Core.Channel>();

            return ConvertToList(servers);
        }

        private static List<Channel> ConvertToList(XmlNodeList nodeList)
        {
            List<Channel> list = new List<Core.Channel>();
            foreach (XmlNode item in nodeList)
            {
                var channel = NodeToChannel(item);
                list.Add(channel);
            }
            return list;
        }

        private static Channel NodeToChannel(XmlNode item)
        {
            Channel channel = new Core.Channel();
            channel.Index = item.SelectSingleNode("index").InnerText;
            channel.ChannelName = item.SelectSingleNode("name").InnerText;
            channel.AreaName = item.SelectSingleNode("areaName").InnerText;
            channel.ItemId = item.SelectSingleNode("itemId").InnerText;

            var inIp = item.SelectSingleNode("in").InnerText;
            channel.InReader = !inIp.IsEmpty() ? new WeiGuangQRReader(inIp) : null;

            var outIp = item.SelectSingleNode("out").InnerText;
            channel.OutReader = !outIp.IsEmpty() ? new WeiGuangQRReader(outIp) : null;

            channel.Gate = new MegviiGate(new UdpSocket(item.SelectSingleNode("gate").InnerText));
            return channel;
        }

        public static void Save(Channel channel)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            var root = doc.SelectSingleNode("/servers");

            XmlNode server = doc.CreateNode(XmlNodeType.Element, "server", "");
            root.AppendChild(server);

            XmlElement nodeIndex = doc.CreateElement("index");
            nodeIndex.InnerText = channel.Index;
            server.AppendChild(nodeIndex);

            XmlElement nodeName = doc.CreateElement("name");
            nodeName.InnerText = channel.ChannelName;
            server.AppendChild(nodeName);

            XmlElement nodeAreaName = doc.CreateElement("areaName");
            nodeAreaName.InnerText = channel.AreaName;
            server.AppendChild(nodeAreaName);

            XmlElement nodeItemId = doc.CreateElement("itemId");
            nodeItemId.InnerText = channel.ItemId;
            server.AppendChild(nodeItemId);

            XmlElement nodecommunityId = doc.CreateElement("communityId");
            nodecommunityId.InnerText = channel.CommunityId;
            server.AppendChild(nodecommunityId);

            XmlElement nodeIn = doc.CreateElement("in");
            nodeIn.InnerText = channel.CommunityId;
            server.AppendChild(nodeIn);

            XmlElement nodeOut = doc.CreateElement("out");
            nodeOut.InnerText = channel.CommunityId;
            server.AppendChild(nodeOut);

            XmlElement nodeGate = doc.CreateElement("gate");
            nodeGate.InnerText = channel.CommunityId;
            server.AppendChild(nodeGate);

            doc.Save(filename);
        }

        //private static XmlElement CreateElement(string name, string value)
        //{
        //    XmlElement
        //}
    }
}
