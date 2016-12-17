using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class Login : INetworkPacket {
        public int SocketId;
        public string Username;
        public string Password;

        public int Size => 0;

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

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.Login);

            var length = Size; // not required

            bw.Write(length);

            bw.Write(SocketId);

            bw.Write(Username);
            bw.Write(Password);


            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
