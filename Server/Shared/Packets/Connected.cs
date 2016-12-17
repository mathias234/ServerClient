using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class Connected : INetworkPacket {
        public int SocketId { get; }

        public Connected() {
            
        }

        public Connected(int socketId) {
            SocketId = socketId;
        }

        public string Serialize() {
            return SocketId.ToString();
        }

        public static Connected Deserialize(string newCharacter) {
            return new Connected(int.Parse(newCharacter));
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.Connected);

            var length = Marshal.SizeOf(SocketId);

            bw.Write(length);

            bw.Write(SocketId);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
