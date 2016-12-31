using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Shared.Packets;

namespace Shared {
    public class RequestCharacters : INetworkPacket {
        public int SocketId { get; }
        public List<Character> Characters;

        public RequestCharacters(int socketId, List<Character> characters) {
            SocketId = socketId;
            Characters = characters;
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.RequestCharacters);

            var length = Characters.Count;

            bw.Write(length);

            bw.Write(SocketId);

            foreach (var character in Characters) {
                bw.Write(character.ToByteArray());
            }


            var data = ((MemoryStream)bw.BaseStream).ToArray();
            return data;
        }
    }
}
