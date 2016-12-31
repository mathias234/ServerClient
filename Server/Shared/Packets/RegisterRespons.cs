using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shared.Packets {
    public class RegisterRespons : INetworkPacket {
        public enum RegisterResponses {
            Success,
            UsernameAlreadyInUse,
            Failed // Unknown reason
        }

        public int SocketId;
        public RegisterResponses Respons;
        public int Size => 0;

        public RegisterRespons() {
            SocketId = -1;
            Respons = RegisterResponses.Failed;
        }

        public RegisterRespons(int socketId, RegisterResponses respons) {
            SocketId = socketId;
            Respons = respons;
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.RegisterRespons);

            var length = Size; // not required

            bw.Write(length);

            bw.Write(SocketId);

            bw.Write((int)Respons);


            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
