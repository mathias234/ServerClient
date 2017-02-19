using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shared.Packets {
    public class AuthenticationRespons : BaseNetworkPacket {
        public enum AuthenticationResponses {
            Success,
            AlreadyLoggedIn,
            WrongUsernameAndPassword,
            Failed
        }

        public AuthenticationResponses Respons;

        public AuthenticationRespons() {
            SocketId = -1;
            Respons = AuthenticationResponses.Failed;
        }

        public AuthenticationRespons(int socketId, AuthenticationResponses respons) {
            SocketId = socketId;
            Respons = respons;
        }

        public override BaseNetworkPacket FromByteArray(byte[] byteArray) {
            var br = new BinaryReader(new MemoryStream(byteArray));
            Header = (PacketHeader)br.ReadInt32();
            SocketId = br.ReadInt32();
            Respons = (AuthenticationResponses)br.ReadInt32();

            return this;
        }

        public override byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.AuthenticationRespons);

            bw.Write(SocketId);

            bw.Write((int)Respons);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
