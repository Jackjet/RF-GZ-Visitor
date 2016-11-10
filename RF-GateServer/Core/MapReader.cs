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
        public static List<Channel> Read()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("comserver.xml");

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
    }
}
