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
        Connected, // send this to the client that connects
        CharacterDisconnect,
        Login,
        AuthenticationRespons,
        Register,
        RegisterRespons,
        RequestCharacters,
        CreateCharacter,
        CreateCharacterRespons,
        FullCharacterUpdate,
        Movement,
        ConnectedToMap,
        CharactersInMap,
        NotifyOtherPlayerMapChange, // Notify the other players that a character either DCed or changed map
        ChangeMap
    }

    public class PacketCreator {
        // TODO: Make dynamic packet reader and writer
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
                    case PacketHeader.Register:
                        socketId = br.ReadInt32();

                        return new Packet(header, new AccountRegister(socketId, br.ReadString(), br.ReadString()));
                    case PacketHeader.RegisterRespons:
                        socketId = br.ReadInt32();
                        var RegRespons = (RegisterRespons.RegisterResponses)br.ReadInt32();

                        return new Packet(header, new RegisterRespons(socketId, RegRespons));
                    case PacketHeader.RequestCharacters:
                        socketId = br.ReadInt32();

                        var characters = new List<Character>();

                        for (int i = 0; i < length; i++) {
                            characters.Add(new Character(br));
                        }

                        return new Packet(header, new RequestCharacters(socketId, characters));
                    case PacketHeader.CreateCharacter:
                        socketId = br.ReadInt32();

                        return new Packet(header, new CreateCharacter(socketId, br.ReadString(), (CharacterClasses)br.ReadByte()));
                    case PacketHeader.CreateCharacterRespons:
                        socketId = br.ReadInt32();
                        var createCharacterRespons = (CreateCharacterRespons.CreateCharacterResponses)br.ReadInt32();

                        return new Packet(header, new CreateCharacterRespons(socketId, createCharacterRespons));
                    case PacketHeader.FullCharacterUpdate:
                        socketId = br.ReadInt32();
   

                        return new Packet(header, new FullCharacterUpdate(socketId, new Character(br)));
                    case PacketHeader.ConnectedToMap:
                        socketId = br.ReadInt32();
                        var mapId = br.ReadInt32();

                        return new Packet(header, new ConnectedToMap(socketId, mapId));
                    case PacketHeader.CharactersInMap:
                        socketId = br.ReadInt32();

                        characters = new List<Character>();

                        for (int i = 0; i < length; i++) {
                            characters.Add(new Character(br));
                        }

                        return new Packet(header, new CharactersInMap(socketId, characters));
                    case PacketHeader.NotifyOtherPlayerMapChange:
                        socketId = br.ReadInt32();

                        return new Packet(header, new NotifyOtherPlayerMapChange(socketId, new Character(br)));
                    case PacketHeader.ChangeMap:
                        socketId = br.ReadInt32();
                        var oldMapId = br.ReadInt32();
                        var newMapId = br.ReadInt32();

                        return new Packet(header, new ChangeMap(socketId, oldMapId, newMapId));
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return null;
            }
            // return the bytes
            catch (Exception ex) {
                Console.WriteLine("Failed to read packet: " + ex.Message);
                return null;
            }
        }
    }
}