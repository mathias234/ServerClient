using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class ConnectedToMap : INetworkPacket {
        public int SocketId { get; set; }
        public int MapId { get; set; }

        public ConnectedToMap(int socketId, int mapId) {
            SocketId = socketId;
            MapId = mapId;
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.ConnectedToMap);

            var length = Marshal.SizeOf(SocketId) * 2;

            bw.Write(length);

            bw.Write(SocketId);
            bw.Write(MapId);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
