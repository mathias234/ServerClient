using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class NotifyOtherPlayerMapChange {
        public int SocketId { get; set; }
        public Character Character;
        public NotifyOtherPlayerMapChange(int socketId, Character character) {
            SocketId = socketId;
            Character = character;
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.NotifyOtherPlayerMapChange);

            var length = Marshal.SizeOf(SocketId); // not required

            bw.Write(length);

            bw.Write(SocketId);

            bw.Write(Character.ToByteArray());

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
