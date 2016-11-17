using Common;
using RF_GateServer.Gate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public static ObservableCollection<Channel> Read()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            var servers = doc.SelectNodes(xPathchannel);
            if (servers.Count == 0)
                return new ObservableCollection<Core.Channel>();

            return ConvertToList(servers);
        }

        private static ObservableCollection<Channel> ConvertToList(XmlNodeList nodeList)
        {
            ObservableCollection<Channel> list = new ObservableCollection<Channel>();
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
            channel.Name = item.SelectSingleNode("name").InnerText;
            channel.Area = item.SelectSingleNode("areaName").InnerText;
            channel.ItemId = item.SelectSingleNode("itemId").InnerText;
            channel.CommunityId = item.SelectSingleNode("communityId").InnerText;

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

            doc.Save(filename);
        }

        public static void Delete(string index)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            var parent = doc.SelectSingleNode("/channels");
            var path = "/channels/channel[index='" + index + "']";
            var node = doc.SelectSingleNode(path);
            if (node != null)
            {
                parent.RemoveChild(node);
            }
            doc.Save(filename);
        }
    }
}
