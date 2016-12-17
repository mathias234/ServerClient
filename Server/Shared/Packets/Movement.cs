using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class Movement : INetworkPacket {
        public int SocketId;
        public NetworkVector3 NewPosition;
        public float YRotation { get; set;  }
        public float Forward { get; set; }
        public float Turn { get; set; }
        public bool Crouch { get; set; }
        public bool OnGround { get; set; }
        public float Jump { get; set; }
        public float JumpLeg { get; set; }


        public int Size => Marshal.SizeOf(SocketId) + Marshal.SizeOf(NewPosition.X) * 3 + Marshal.SizeOf(YRotation) * 5 + Marshal.SizeOf(Crouch) * 2;

        public Movement() {
            Forward = 0;
            Turn = 0;
            Crouch = false;
            OnGround = false;
            Jump = 0;
            SocketId = -1;
            NewPosition = new NetworkVector3();
        }

        public Movement(int socketId, NetworkVector3 newPosition, float yRotation, float forward, float turn, bool crouch, bool onGround, float jump, float jumpLeg) {
            SocketId = socketId;
            NewPosition = newPosition;
            YRotation = yRotation;
            Forward = forward;
            Turn = turn;
            Crouch = crouch;
            OnGround = onGround;
            Jump = jump;
            JumpLeg = jumpLeg;
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.Movement);

            var length = Size; // not required

            bw.Write(length);

            bw.Write(SocketId);
            bw.Write(NewPosition.X);
            bw.Write(NewPosition.Y);
            bw.Write(NewPosition.Z);
            bw.Write(YRotation);

            // a lazy way to make the animations look ok
            bw.Write(Forward);
            bw.Write(Turn);
            bw.Write(Crouch);
            bw.Write(OnGround);
            bw.Write(Jump);
            bw.Write(JumpLeg);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
