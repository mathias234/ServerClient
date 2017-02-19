using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shared.Packets {
    public class RegisterRespons : BaseNetworkPacket {
        public enum RegisterResponses {
            Success,
            UsernameAlreadyInUse,
            Failed // Unknown reason
        }

        public RegisterResponses Respons;

        public RegisterRespons() {
            SocketId = -1;
            Respons = RegisterResponses.Failed;
        }

        public RegisterRespons(int socketId, RegisterResponses respons) {
            SocketId = socketId;
            Respons = respons;
        }

        public override BaseNetworkPacket FromByteArray(byte[] byteArray) {
            var br = new BinaryReader(new MemoryStream(byteArray));

            Header = (PacketHeader)br.ReadInt32();
            SocketId = br.ReadInt32();
            Respons = (RegisterResponses)br.ReadInt32();

            return this;
        }

        public override byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.RegisterRespons);

            bw.Write(SocketId);

            bw.Write((int)Respons);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
