using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Packets {
    public class AccountRegister : BaseNetworkPacket, INetworkPacket<AccountRegister> {
        public string Username { get; set; }
        public string Password { get; set; }

        public AccountRegister() {
            SocketId = -1;
            Username = "";
            Username = "";
        }

        public AccountRegister(int socketId, string username, string password) {
            SocketId = socketId;
            Username = username;
            Password = password;
        }

        public AccountRegister FromByteArray(byte[] byteArray) {
            var br = new BinaryReader(new MemoryStream(byteArray));

            Header = (PacketHeader)br.ReadInt32();
            SocketId = br.ReadInt32();
            Username = br.ReadString();
            Password = br.ReadString();

            return this;
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.AccountRegister);

            bw.Write(SocketId);
            bw.Write(Username);
            bw.Write(Password);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
