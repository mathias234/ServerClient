using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class CreateCharacter : BaseNetworkPacket {
        public string Name;
        public CharacterClasses CharClass;

        public CreateCharacter() {
            SocketId = -1;
            Name = "";
            CharClass = CharacterClasses.Agent;
        }

        public CreateCharacter(int socketId, string name, CharacterClasses charClass) {
            SocketId = socketId;
            Name = name;
            CharClass = charClass;
        }

        public override BaseNetworkPacket FromByteArray(byte[] byteArray) {
            var br = new BinaryReader(new MemoryStream(byteArray));

            Header = (PacketHeader)br.ReadInt32();
            SocketId = br.ReadInt32();
            Name = br.ReadString();
            CharClass = (CharacterClasses)br.ReadByte();

            return this;
        }

        public override byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.CreateCharacter);

            bw.Write(SocketId);
            bw.Write(Name);
            bw.Write((byte)CharClass);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
