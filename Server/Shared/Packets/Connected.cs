using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class Connected : BaseNetworkPacket {
        public Connected() {
            SocketId = -1;
        }

        public Connected(int socketId) {
            SocketId = socketId;
        }

        public override BaseNetworkPacket FromByteArray(byte[] byteArray) {
            var br = new BinaryReader(new MemoryStream(byteArray));

            Header = (PacketHeader)br.ReadInt32();
            SocketId = br.ReadInt32();

            return this;
        }

        public override byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.Connected);

            bw.Write(SocketId);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
