using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class ChangeMap : BaseNetworkPacket {
        public int OldMapId;
        public int NewMapId;
        public float NewX;
        public float NewY;
        public float NewZ;

        public ChangeMap() {
            SocketId = -1;
            OldMapId = -1;
            NewMapId = -1;
            NewX = 0;
            NewY = 0;
            NewZ = 0;
        }

        public ChangeMap(int socketId, int oldMapId, int newMapId, float newX, float newY, float newZ) {
            SocketId = socketId;
            OldMapId = oldMapId;
            NewMapId = newMapId;
            NewX = newX;
            NewY = newY;
            NewZ = newZ;
        }

        public override BaseNetworkPacket FromByteArray(byte[] byteArray) {
            var br = new BinaryReader(new MemoryStream(byteArray));

            Header = (PacketHeader)br.ReadInt32();
            SocketId = br.ReadInt32();
            OldMapId = br.ReadInt32();
            NewMapId = br.ReadInt32();
            NewX = br.ReadSingle();
            NewY = br.ReadSingle();
            NewZ = br.ReadSingle();

            return this;
        }

        public override byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.ChangeMap);

            bw.Write(SocketId);
            bw.Write(OldMapId);
            bw.Write(NewMapId);
            bw.Write(NewX);
            bw.Write(NewY);
            bw.Write(NewZ);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
