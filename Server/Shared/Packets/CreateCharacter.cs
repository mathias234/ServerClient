using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class CreateCharacter : INetworkPacket {
        public int SocketId;
        public string Name;
        public CharacterClasses CharClass;

        public CreateCharacter(int socketId, string name, CharacterClasses charClass) {
            SocketId = socketId;
            Name = name;
            CharClass = charClass;
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.CreateCharacter);

            var length = Marshal.SizeOf(SocketId);

            bw.Write(length);

            bw.Write(SocketId);
            bw.Write(Name);
            bw.Write((byte)CharClass);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
