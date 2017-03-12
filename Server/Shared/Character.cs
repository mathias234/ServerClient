using Shared.Creature;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shared {
    public class Character : Entity {
        public int SocketId { get; set; }
        public int CharacterId { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public CharacterClasses Class { get; set; }
        public int MapId;

        public Character(int socketId, int characterId, string name, int level, CharacterClasses charClass, int mapId, NetworkVector3 position) {
            Init(true);

            SocketId = socketId;
            CharacterId = characterId;
            Name = name;
            Level = level;
            Class = charClass;
            MapId = mapId;
            Position = position;
        }

        public Character(BinaryReader br) {
            Init(true);

            SocketId = br.ReadInt32();
            CharacterId = br.ReadInt32();
            Name = br.ReadString();
            Level = br.ReadInt32();
            Class = (CharacterClasses)br.ReadByte();
            MapId = br.ReadInt32();

            Position = new NetworkVector3();

            Position.X = br.ReadSingle();
            Position.Y = br.ReadSingle();
            Position.Z = br.ReadSingle();
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());

            bw.Write(SocketId);
            bw.Write(CharacterId);
            bw.Write(Name);
            bw.Write(Level);
            bw.Write((byte)Class);
            bw.Write(MapId);
            bw.Write(Position.X);
            bw.Write(Position.Y);
            bw.Write(Position.Z);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
