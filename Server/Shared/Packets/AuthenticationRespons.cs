using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shared.Packets {
    public class AuthenticationRespons : INetworkPacket {
        public enum AuthenticationResponses {
            Success,
            Failed
        }

        public int SocketId;
        public AuthenticationResponses Respons;
        public int Size => 0;

        public AuthenticationRespons() {
            SocketId = -1;
            Respons = AuthenticationResponses.Failed;
        }

        public AuthenticationRespons(int socketId, AuthenticationResponses respons) {
            SocketId = socketId;
            Respons = respons;
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.AuthenticationRespons);

            var length = Size; // not required

            bw.Write(length);

            bw.Write(SocketId);

            bw.Write((int)Respons);


            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
