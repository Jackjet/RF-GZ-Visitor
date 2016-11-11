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

        private const string xPathchannel = "/channels/channel";
        private const string xPathchannels = "/channels";

        public static List<Channel> Read()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            var servers = doc.SelectNodes(xPathchannel);
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
            channel.InIp = inIp;

            var outIp = item.SelectSingleNode("out").InnerText;
            channel.OutIp = outIp;

            var gateIp = item.SelectSingleNode("gate").InnerText;
            channel.GateIp = gateIp;
            return channel;
        }

        public static void Save(Channel channel)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            var root = doc.SelectSingleNode(xPathchannels);

            XmlNode server = doc.CreateNode(XmlNodeType.Element, "channel", "");
            root.AppendChild(server);

            XmlElement nodeIndex = doc.CreateElement("index", channel.Index);
            server.AppendChild(nodeIndex);

            XmlElement nodeName = doc.CreateElement("name", channel.ChannelName);
            server.AppendChild(nodeName);

            XmlElement nodeAreaName = doc.CreateElement("areaName", channel.AreaName);
            server.AppendChild(nodeAreaName);

            XmlElement nodeItemId = doc.CreateElement("itemId", channel.ItemId);
            server.AppendChild(nodeItemId);

            XmlElement nodecommunityId = doc.CreateElement("communityId", channel.CommunityId);
            server.AppendChild(nodecommunityId);

            XmlElement nodeIn = doc.CreateElement("in", channel.InIp);
            server.AppendChild(nodeIn);

            XmlElement nodeOut = doc.CreateElement("out", channel.OutIp);
            server.AppendChild(nodeOut);

            XmlElement nodeGate = doc.CreateElement("gate", channel.GateIp);
            server.AppendChild(nodeGate);

            doc.Save(filename);
        }
    }
}
