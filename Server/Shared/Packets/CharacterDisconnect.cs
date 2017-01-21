using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class CharacterDisconnect : BaseNetworkPacket, INetworkPacket<CharacterDisconnect> {
        public CharacterDisconnect() {
            SocketId = -1;
        }

        public CharacterDisconnect(int socketId) {
            SocketId = socketId;
        }

        public CharacterDisconnect FromByteArray(byte[] byteArray) {
            var br = new BinaryReader(new MemoryStream(byteArray));

            Header = (PacketHeader)br.ReadInt32();
            SocketId = br.ReadInt32();

            return this;
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.CharacterDisconnect);

            bw.Write(SocketId);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
