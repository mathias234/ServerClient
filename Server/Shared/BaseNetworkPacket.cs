using System;
using System.Collections.Generic;
using System.Text;

namespace Shared {
    public class BaseNetworkPacket {
        public PacketHeader Header;
        public int SocketId;

        public BaseNetworkPacket() {
            SocketId = -1;
        }

        public BaseNetworkPacket(int socketId) {
            SocketId = socketId;
        }

        public virtual BaseNetworkPacket FromByteArray(byte[] byteArray) { return null; }
        public virtual byte[] ToByteArray() { return null; }
    }
}
