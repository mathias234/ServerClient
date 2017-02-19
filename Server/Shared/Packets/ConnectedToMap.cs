using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class ConnectedToMap : BaseNetworkPacket {
        public int MapId { get; set; }

        public ConnectedToMap() {
            SocketId = -1;
            MapId = -1;
        }

        public ConnectedToMap(int socketId, int mapId) {
            SocketId = socketId;
            MapId = mapId;
        }

        public override BaseNetworkPacket FromByteArray(byte[] byteArray) {
            var br = new BinaryReader(new MemoryStream(byteArray));

            Header = (PacketHeader)br.ReadInt32();
            SocketId = br.ReadInt32();
            MapId = br.ReadInt32();

            return this;
        }

        public override byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.ConnectedToMap);

            bw.Write(SocketId);
            bw.Write(MapId);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
