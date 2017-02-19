using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class NotifyOtherPlayerMapChange : BaseNetworkPacket {
        public int OldMapId;
        public Character Character;

        public NotifyOtherPlayerMapChange() {
            SocketId = -1;
            OldMapId = -1;
            Character = null;
        }

        public NotifyOtherPlayerMapChange(int socketId, int oldMapId, Character character) {
            SocketId = socketId;
            OldMapId = oldMapId;
            Character = character;
        }

        public override BaseNetworkPacket FromByteArray(byte[] byteArray) {
            var br = new BinaryReader(new MemoryStream(byteArray));

            Header = (PacketHeader)br.ReadInt32();
            SocketId = br.ReadInt32();
            OldMapId = br.ReadInt32();

            Character = new Character(br);

            return this;
        }

        public override byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.NotifyOtherPlayerMapChange);

            bw.Write(SocketId);
            bw.Write(OldMapId);
            bw.Write(Character.ToByteArray());

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
