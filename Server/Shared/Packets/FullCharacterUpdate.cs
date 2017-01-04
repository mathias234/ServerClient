using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    // Get all data about a character
    public class FullCharacterUpdate : INetworkPacket {
        public int SocketId;
        public Character NewCharacter;


        public FullCharacterUpdate(int socketId, Character character) {
            SocketId = socketId;
            NewCharacter = character;
        }

        public FullCharacterUpdate(int socketId, int characterId) {
            SocketId = socketId;
            NewCharacter = new Character(-1, characterId, "", 0, CharacterClasses.Agent, 0, 0, 0, 0);
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.FullCharacterUpdate);

            var length = Marshal.SizeOf(SocketId);

            bw.Write(length);

            bw.Write(SocketId);
            bw.Write(NewCharacter.ToByteArray());

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
