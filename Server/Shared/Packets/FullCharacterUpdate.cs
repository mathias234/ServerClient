using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    // Get all data about a character
    public class FullCharacterUpdate : BaseNetworkPacket {
        public Character NewCharacter;

        public FullCharacterUpdate() {
            SocketId = -1;
            NewCharacter = null;
        }

        public FullCharacterUpdate(int socketId, Character character) {
            SocketId = socketId;
            NewCharacter = character;
        }

        public FullCharacterUpdate(int socketId, int characterId) {
            SocketId = socketId;
            NewCharacter = new Character(-1, characterId, "", 0, CharacterClasses.Agent, 0, 0, 0, 0);
        }

        public override BaseNetworkPacket FromByteArray(byte[] byteArray) {
            var br = new BinaryReader(new MemoryStream(byteArray));

            Header = (PacketHeader)br.ReadInt32();
            SocketId = br.ReadInt32();
            NewCharacter = new Character(br);

            return this;
        }

        public override byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.FullCharacterUpdate);

            bw.Write(SocketId);
            bw.Write(NewCharacter.ToByteArray());

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
