using Shared;

namespace Server.Packet {
    public class BasePacketHandler {
        public virtual void HandlePacket(int socketId, BaseNetworkPacket packet) {

        }
    }
}