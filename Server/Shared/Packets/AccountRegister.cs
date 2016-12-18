using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class AccountRegister : INetworkPacket {
        public int SocketId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public AccountRegister(int socketId, string username, string password) {
            SocketId = socketId;
            Username = username;
            Password = password;
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.Register);

            var length = Marshal.SizeOf(SocketId); // not required

            bw.Write(length);

            bw.Write(SocketId);
            bw.Write(Username);
            bw.Write(Password);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
