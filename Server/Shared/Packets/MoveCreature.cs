using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shared.Packets {
    public class MoveCreature : BaseNetworkPacket {
        public int InstanceId;
        public float X;
        public float Y;
        public float Z;

        public MoveCreature() {
            SocketId = -1;
        }

        public MoveCreature(int socketId, int instanceId, float x, float y, float z) {
            SocketId = socketId;
            InstanceId = instanceId;
            X = x;
            Y = y;
            Z = z;
        }

        public override BaseNetworkPacket FromByteArray(byte[] byteArray) {
            var br = new BinaryReader(new MemoryStream(byteArray));

            Header = (PacketHeader)br.ReadInt32();
            SocketId = br.ReadInt32();

            InstanceId = br.ReadInt32();
            X = br.ReadSingle();
            Y = br.ReadSingle();
            Z = br.ReadSingle();

            return this;
        }

        public override byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.MoveCreature);

            bw.Write(SocketId);
            bw.Write(InstanceId);
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
