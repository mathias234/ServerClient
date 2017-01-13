using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class NotifyOtherPlayerMapChange {
        public int SocketId { get; set; }
        public int OldMapId;
        public Character Character;

        public NotifyOtherPlayerMapChange(int socketId, int oldMapId, Character character) {
            SocketId = socketId;
            OldMapId = oldMapId;
            Character = character;
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.NotifyOtherPlayerMapChange);

            var length = Marshal.SizeOf(SocketId); // not required

            bw.Write(length);

            bw.Write(SocketId);

            bw.Write(OldMapId);

            bw.Write(Character.ToByteArray());

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
