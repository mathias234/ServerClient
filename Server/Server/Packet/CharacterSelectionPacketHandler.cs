using MySql.Data.MySqlClient;
using Shared;
using Shared.Packets;
using System.Collections.Generic;
using System.Linq;

namespace Server.Packet {
    /// <summary>
    /// Handles all character creation and selection
    /// </summary>
    [PacketHandlerAttribute(PacketHeader.RequestCharacters, PacketHeader.CreateCharacter)]
    public class CharacterEditPacketHandler : BasePacketHandler {
        public override void HandlePacket(int socketId, BaseNetworkPacket packet) {
            switch (packet.Header) {
                case PacketHeader.RequestCharacters:
                    var characters = new List<Character>();

                    MySqlDataReader reader;

                    if (!MainServer.MainDb.Run(string.Format("SELECT * FROM characters where accountId={0};",
                        MainServer.GetAccountFromSocketId(socketId).AccountId), out reader)) {
                        Log.Debug("Failed to find characters");
                    } else {
                        while (reader.Read()) {
                            var id = (int)reader["id"];
                            var name = (string)reader["characterName"];
                            var level = (int)reader["characterLevel"];
                            var charClass = (int)reader["characterClass"];
                            var mapId = (int)reader["mapId"];
                            var x = (float)reader["x"];
                            var y = (float)reader["y"];
                            var z = (float)reader["z"];

                            characters.Add(new Character(socketId, id, name, level, (CharacterClasses)charClass, mapId, x, y, z));
                        }
                        reader.Close();
                    }

                    MainServer.SendData(socketId, new RequestCharacters(socketId, characters).ToByteArray());
                    break;
                case PacketHeader.CreateCharacter:
                    var createCharacter = (CreateCharacter)packet;
                    Log.Debug("Creating character with name: " + createCharacter.Name + " and class " + createCharacter.CharClass);

                    characters = new List<Character>();

                    // TODO: Cache this
                    if (!MainServer.MainDb.Run("SELECT * FROM characters", out reader)) {
                        Log.Debug("Failed to find characters");
                    } else {
                        while (reader.Read()) {
                            var id = (int)reader["id"];
                            var name = (string)reader["characterName"];
                            var level = (int)reader["characterLevel"];
                            var charClass = (int)reader["characterClass"];
                            var mapId = (int)reader["mapId"];
                            var x = (float)reader["x"];
                            var y = (float)reader["y"];
                            var z = (float)reader["z"];

                            characters.Add(new Character(-1, id, name, level, (CharacterClasses)charClass, mapId, x, y, z));
                        }
                        reader.Close();
                    }

                    if (characters.Any(character => character.Name == createCharacter.Name)) {
                        MainServer.SendData(socketId, new CreateCharacterRespons(socketId, CreateCharacterRespons.CreateCharacterResponses.NameAlreadyUsed).ToByteArray());
                        return;
                    }

                    var createCharacterSql = string.Format(
                    "INSERT INTO characters (accountId, characterName, characterLevel, characterClass, mapId, x, y, z) VALUES ({0}, '{1}', 1, {2}, {3}, {4}, {5}, {6});",
                    MainServer.GetAccountFromSocketId(createCharacter.SocketId).AccountId,
                    createCharacter.Name,
                    (int)createCharacter.CharClass,
                    // This data is where the player will start. TODO: Remove hardcoding
                    0,
                    0,
                    23.82,
                    10.7);

                    if (MainServer.MainDb.Run(createCharacterSql)) {
                        Log.Debug("successfully created character");
                        MainServer.SendData(socketId, new CreateCharacterRespons(socketId, CreateCharacterRespons.CreateCharacterResponses.Success).ToByteArray());
                    } else {
                        MainServer.SendData(socketId, new CreateCharacterRespons(socketId, CreateCharacterRespons.CreateCharacterResponses.Failed).ToByteArray());
                    }
                    break;
            }
        }
    }
}
