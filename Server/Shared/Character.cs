using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shared {
    public class Character {
        public int SocketId { get; set; }
        public int CharacterId { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public CharacterClasses Class { get; set; }
        public int MapId;
        public float X;
        public float Y;
        public float Z;

        public Character(int socketId, int characterId, string name, int level, CharacterClasses charClass, int mapId, float x, float y, float z) {
            SocketId = socketId;
            CharacterId = characterId;
            Name = name;
            Level = level;
            Class = charClass;
            MapId = mapId;
            X = x;
            Y = y;
            Z = z;
        }

        public Character(BinaryReader br) {
            SocketId = br.ReadInt32();
            CharacterId = br.ReadInt32();
            Name = br.ReadString();
            Level = br.ReadInt32();
            Class = (CharacterClasses)br.ReadByte();
            MapId = br.ReadInt32();
            X = br.ReadSingle();
            Y = br.ReadSingle();
            Z = br.ReadSingle();
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());

            bw.Write(SocketId);
            bw.Write(CharacterId);
            bw.Write(Name);
            bw.Write(Level);
            bw.Write((byte)Class);
            bw.Write(MapId);
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
