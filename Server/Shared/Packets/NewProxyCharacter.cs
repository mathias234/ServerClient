using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class NewProxyCharacter : INetworkPacket {
        public int SocketId { get; set; }

        public NewProxyCharacter() {

        }

        public NewProxyCharacter(int socketId) {
            SocketId = socketId;
        }

        public string Serialize() {
            return SocketId.ToString();
        }

        public static NewProxyCharacter Deserialize(string newCharacter) {
            return new NewProxyCharacter(int.Parse(newCharacter));
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.NewProxyCharacter);

            var length = Marshal.SizeOf(SocketId);

            bw.Write(length);

            bw.Write(SocketId);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
