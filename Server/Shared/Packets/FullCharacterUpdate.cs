using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    // Get all data about a character
    public class FullCharacterUpdate : INetworkPacket {
        public int SocketId;
        public int CharacterId;
        public string CharacterName;
        public int CharacterLevel;
        public int CharacterClass;
        public int MapId;
        public float X;
        public float Y;
        public float Z;

        public FullCharacterUpdate(int socketId, int characterId, string characterName, int characterLevel, int characterClass, int mapId, float x, float y, float z) {
            SocketId = socketId;
            CharacterId = characterId;
            CharacterName = characterName;
            CharacterLevel = characterLevel;
            CharacterClass = characterClass;
            MapId = mapId;
            X = x;
            Y = y;
            Z = z;
        }

        public FullCharacterUpdate(int socketId, int characterId) {
            SocketId = socketId;
            CharacterId = characterId;
            CharacterName = "";
            CharacterLevel = 0;
            CharacterClass = 0;
            MapId = 0;
            X = 0;
            Y = 0;
            Z = 0;
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.FullCharacterUpdate);

            var length = Marshal.SizeOf(SocketId);

            bw.Write(length);

            bw.Write(SocketId);
            bw.Write(CharacterId);
            bw.Write(CharacterName);
            bw.Write(CharacterLevel);
            bw.Write(CharacterClass);
            bw.Write(MapId);
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
