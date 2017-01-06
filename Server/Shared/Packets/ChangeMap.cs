using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class ChangeMap : INetworkPacket {
        public int SocketId;
        public int OldMapId;
        public int NewMapId;

        public ChangeMap() {
            SocketId = -1;
            OldMapId = -1;
            NewMapId = -1;
        }

        public ChangeMap(int socketId, int oldMapId, int newMapId) {
            SocketId = socketId;
            OldMapId = oldMapId;
            NewMapId = newMapId;
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.ChangeMap);

            var length = Marshal.SizeOf(SocketId) * 3; // not required

            bw.Write(length);

            bw.Write(SocketId);
            bw.Write(OldMapId);
            bw.Write(NewMapId);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
