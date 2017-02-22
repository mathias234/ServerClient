using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Shared.Packets;
using Shared;
using Server.Packet;
using System.Reflection;

namespace Server {
    [AttributeUsage(AttributeTargets.Class)]
    public class PacketHandlerAttribute : Attribute {
        public List<PacketHeader> PacketHeaders;

        public PacketHandlerAttribute(params PacketHeader[] packetHeaders) {
            PacketHeaders = new List<PacketHeader>();

            foreach (var packet in packetHeaders) {
                PacketHeaders.Add(packet);
            }
        }
    }

    public class PacketHandler { 
        public static List<BasePacketHandler> initializedPacketHandlers = new List<BasePacketHandler>();

        public static int HandlePacket(int socketId, BaseNetworkPacket packet) {
            if (packet?.Header == null)
                return 0;

            var baseType = typeof(BasePacketHandler);
            var handlers = Assembly.GetAssembly(baseType).GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t));

            foreach (var handler in handlers) {
                var attribute = (PacketHandlerAttribute)Attribute.GetCustomAttribute(handler, typeof(PacketHandlerAttribute));
                foreach (var packetHeader in attribute.PacketHeaders) {
                    if (packetHeader == packet.Header) {
                        foreach (var packetHandler in initializedPacketHandlers) {
                            if (packetHandler.GetType() == handler) {
                                packetHandler.HandlePacket(socketId, packet);
                                return 1;
                            }
                        }

                        BasePacketHandler initializedHandler = (BasePacketHandler)Activator.CreateInstance(handler);
                        initializedPacketHandlers.Add(initializedHandler);
                        initializedHandler.HandlePacket(socketId, packet);

                        return 1;
                    }
                }
            }

            return 1;
        }
    }
}
