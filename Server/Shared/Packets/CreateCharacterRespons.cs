using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shared.Packets {
    public class CreateCharacterRespons : INetworkPacket {
        public enum CreateCharacterResponses {
            Success,
            NameAlreadyUsed,
            Failed
        }

        public int SocketId;
        public CreateCharacterResponses Respons;
        public int Size => 0;

        public CreateCharacterRespons() {
            SocketId = -1;
            Respons = CreateCharacterResponses.Failed;
        }

        public CreateCharacterRespons(int socketId, CreateCharacterResponses respons) {
            SocketId = socketId;
            Respons = respons;
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.CreateCharacterRespons);

            var length = Size; // not required

            bw.Write(length);

            bw.Write(SocketId);

            bw.Write((int)Respons);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
