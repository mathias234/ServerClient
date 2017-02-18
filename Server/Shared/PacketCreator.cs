using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Shared.Packets;

namespace Shared {
    // SHOULD BE THE SAME NAME AS THE CLASS!
    public enum PacketHeader {
        Connected, // send this to the client that connects
        CharacterDisconnect,
        Login,
        AuthenticationRespons,
        AccountRegister,
        RegisterRespons,
        RequestCharacters,
        CreateCharacter,
        CreateCharacterRespons,
        FullCharacterUpdate,
        Movement,
        ConnectedToMap,
        CharactersInMap,
        NotifyOtherPlayerMapChange, // Notify the other players that a character either DCed or changed map
        ChangeMap,
        CreaturesInMap
    }

    public class PacketCreator {
        public static BaseNetworkPacket ReadPacket(byte[] data) {
            var br = new BinaryReader(new MemoryStream(data));
            var header = (PacketHeader)br.ReadInt32();

            try {
                switch (header) {
                    // Make this smarter so it finds the correct class by the packet header
                    case PacketHeader.Movement:
                        return new Movement().FromByteArray(data);
                    case PacketHeader.Connected:
                        return new Connected().FromByteArray(data);
                    case PacketHeader.CharacterDisconnect:
                        return new CharacterDisconnect().FromByteArray(data);
                    case PacketHeader.Login:
                        return new Login().FromByteArray(data);
                    case PacketHeader.AuthenticationRespons:
                        return new AuthenticationRespons().FromByteArray(data);
                    case PacketHeader.AccountRegister:
                        return new AccountRegister().FromByteArray(data);
                    case PacketHeader.RegisterRespons:
                        return new RegisterRespons().FromByteArray(data);
                    case PacketHeader.RequestCharacters:
                        return new RequestCharacters().FromByteArray(data);
                    case PacketHeader.CreateCharacter:
                        return new CreateCharacter().FromByteArray(data);
                    case PacketHeader.CreateCharacterRespons:
                        return new CreateCharacterRespons().FromByteArray(data);
                    case PacketHeader.FullCharacterUpdate:
                        return new FullCharacterUpdate().FromByteArray(data);
                    case PacketHeader.ConnectedToMap:
                        return new ConnectedToMap().FromByteArray(data);
                    case PacketHeader.CharactersInMap:
                        return new CharactersInMap().FromByteArray(data);
                    case PacketHeader.NotifyOtherPlayerMapChange:
                        return new NotifyOtherPlayerMapChange().FromByteArray(data);
                    case PacketHeader.ChangeMap:
                        return new ChangeMap().FromByteArray(data);
                    case PacketHeader.CreaturesInMap:
                        return new CreaturesInMap().FromByteArray(data);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            // return the bytes
            catch (Exception ex) {
                Console.WriteLine("Failed to read packet: " + ex.Message + " : " + header.ToString());
                return null;
            }
        }
    }
}