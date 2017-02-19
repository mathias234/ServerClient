using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shared.Packets {
    public class CreateCharacterRespons : BaseNetworkPacket {
        public enum CreateCharacterResponses {
            Success,
            NameAlreadyUsed,
            Failed
        }
        
        public CreateCharacterResponses Respons;

        public CreateCharacterRespons() {
            SocketId = -1;
            Respons = CreateCharacterResponses.Failed;
        }

        public CreateCharacterRespons(int socketId, CreateCharacterResponses respons) {
            SocketId = socketId;
            Respons = respons;
        }

        public override BaseNetworkPacket FromByteArray(byte[] byteArray) {
            var br = new BinaryReader(new MemoryStream(byteArray));

            Header = (PacketHeader)br.ReadInt32();
            SocketId = br.ReadInt32();
            Respons = (CreateCharacterResponses)br.ReadInt32();

            return this;
        }

        public override byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.CreateCharacterRespons);

            bw.Write(SocketId);

            bw.Write((int)Respons);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
