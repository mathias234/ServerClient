﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Shared.Packets;

namespace Shared.Packets {
    public class RequestCharacters : BaseNetworkPacket {
        public List<Character> Characters;

        public RequestCharacters() {
            SocketId = -1;
            Characters = null;
        }

        public RequestCharacters(int socketId, List<Character> characters) {
            SocketId = socketId;
            Characters = characters;
        }

        public override BaseNetworkPacket FromByteArray(byte[] byteArray) {
            var br = new BinaryReader(new MemoryStream(byteArray));

            Header = (PacketHeader)br.ReadInt32();
            SocketId = br.ReadInt32();

            var charactersCount = br.ReadInt32();

            Characters = new List<Character>();

            for (int i = 0; i < charactersCount; i++) {
                Characters.Add(new Character(br));
            }

            return this;
        }

        public override byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.RequestCharacters);

            bw.Write(SocketId);

            bw.Write(Characters.Count);

            foreach (var character in Characters) {
                bw.Write(character.ToByteArray());
            }

            var data = ((MemoryStream)bw.BaseStream).ToArray();
            return data;
        }
    }
}
