﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class Movement : BaseNetworkPacket {
        public NetworkVector3 NewPosition;
        public float YRotation { get; set; }
        public float Forward { get; set; }
        public float Turn { get; set; }
        public bool Crouch { get; set; }
        public bool OnGround { get; set; }
        public float Jump { get; set; }
        public float JumpLeg { get; set; }

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

        public override byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.Movement);

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

        public override BaseNetworkPacket FromByteArray(byte[] byteArray) {
            var br = new BinaryReader(new MemoryStream(byteArray));

            Header = (PacketHeader)br.ReadInt32();
            SocketId = br.ReadInt32();

            NewPosition = new NetworkVector3() {
                X = br.ReadSingle(),
                Y = br.ReadSingle(),
                Z = br.ReadSingle()
            };

            YRotation = br.ReadSingle();
            Forward = br.ReadSingle();
            Turn = br.ReadSingle();
            Crouch = br.ReadBoolean();
            OnGround = br.ReadBoolean();
            Jump = br.ReadSingle();
            JumpLeg = br.ReadSingle();

            return this;
        }
    }
}
