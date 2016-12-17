using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class CharacterDisconnect : INetworkPacket {
        public CharacterDisconnect(int socketId) {
            SocketId = socketId;
        }

        public int SocketId { get; }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.CharacterDisconnect);

            var length = Marshal.SizeOf(SocketId); // not required

            bw.Write(length);

            bw.Write(SocketId);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
