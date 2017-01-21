using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class Login : BaseNetworkPacket, INetworkPacket<Login> {
        public string Username;
        public string Password;

        public Login() {
            SocketId = -1;
            Username = "";
            Password = "";
        }

        public Login(int socketId, string username, string password) {
            SocketId = socketId;
            Username = username;
            Password = password;
        }

        public Login FromByteArray(byte[] byteArray) {
            var br = new BinaryReader(new MemoryStream(byteArray));

            Header = (PacketHeader)br.ReadInt32();
            SocketId = br.ReadInt32();
            Username = br.ReadString();
            Password = br.ReadString();
            return this;
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.Login);

            bw.Write(SocketId);

            bw.Write(Username);
            bw.Write(Password);


            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
