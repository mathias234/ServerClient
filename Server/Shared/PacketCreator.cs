using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Shared.Packets;

namespace Shared {
    // Create or Read packets
    // A packet will be formatted this way
    // (short)Packet Type
    // (byte[])data

    public class Packet {
        public PacketHeader Header { get; set; }
        public object Value { get; set; }

        public Packet(PacketHeader header, object value) {
            Header = header;
            Value = value;
        }
    }

    public enum PacketHeader {
        GetTime,
        Ping, // a ping pong, the client / server sends a ping and should recieve a pong. Useful for checking if connection is still active
        NewProxyCharacter, // send this to all other clients when a player connects to the server
        Connected, // send this to the client that connects
        GetOtherPlayers, // client can use this to ask the server for a list of other players online
        Movement, // Should be recieved by the server then distributed to other online players
        CharacterDisconnect,
        Login,
        AuthenticationRespons
    }

    public class PacketCreator {
        public static Packet ReadPacket(byte[] data) {
            var br = new BinaryReader(new MemoryStream(data));

            var header = (PacketHeader)br.ReadInt32();
            var length = br.ReadInt32();

            try {
                switch (header) {
                    case PacketHeader.Movement:
                        var socketId = br.ReadInt32();
                        var x = br.ReadSingle();
                        var y = br.ReadSingle();
                        var z = br.ReadSingle();
                        var yRot = br.ReadSingle();
                        var forward = br.ReadSingle();
                        var turn = br.ReadSingle();
                        var crouch = br.ReadBoolean();
                        var onGround = br.ReadBoolean();
                        var jump = br.ReadSingle();
                        var jumpLeg = br.ReadSingle();

                        return new Packet(header, new Movement(socketId, new NetworkVector3(x, y, z), yRot, forward, turn, crouch, onGround, jump, jumpLeg));

                    case PacketHeader.Connected:
                        return new Packet(header, new Connected(br.ReadInt32()));
                    case PacketHeader.NewProxyCharacter:
                        return new Packet(header, new NewProxyCharacter(br.ReadInt32()));
                    case PacketHeader.GetOtherPlayers:
                        var players = new List<int>();

                        for (int i = 0; i < length; i++) {
                            players.Add(br.ReadInt32());
                        }

                        return new Packet(header, players);
                    case PacketHeader.CharacterDisconnect:
                        socketId = br.ReadInt32();
                        return new Packet(header, new CharacterDisconnect(socketId));
                    case PacketHeader.GetTime:
                        break;
                    case PacketHeader.Ping:
                        break;
                    case PacketHeader.Login:
                        socketId = br.ReadInt32();
                        var username = br.ReadString();
                        var password = br.ReadString();
                        return new Packet(header, new Login(socketId, username, password));
                    case PacketHeader.AuthenticationRespons:
                        socketId = br.ReadInt32();
                        var authenticationRespons = (AuthenticationRespons.AuthenticationResponses)br.ReadInt32();

                        return new Packet(header, new AuthenticationRespons(socketId, authenticationRespons));
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return new Packet(header, br.ReadBytes(length));
            }
            // return the bytes
            catch (Exception ex) {
                Console.WriteLine("Failed to read packet: " + ex.Message);
                return null;
            }
        }

        public static byte[] CreatePacket(List<int> players) {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.GetOtherPlayers);

            var length = players.Count;

            bw.Write(length);

            foreach (var player in players) {
                bw.Write(player);
            }

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}