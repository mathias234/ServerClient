using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Shared;
using Shared.Packets;

namespace Server {
    public class PacketHandler {
        public static int HandlePacket(int socketId, Packet packet) {
            if (packet?.Header == null)
                return 0;

            switch (packet.Header) {
                case PacketHeader.Movement:
                    var movement = (Movement)packet.Value;

                    // send this new position to all players
                    Server.SendMovement(movement.SocketId, movement.NewPosition, movement.YRotation, movement.Forward,
                        movement.Turn, movement.Crouch, movement.OnGround, movement.Jump, movement.JumpLeg);
                    break;
                case PacketHeader.Login:
                    var login = (Login)packet.Value;

                    Log.Debug(login.Username + ":" + login.Password);

                    MySqlDataReader reader;

                    var accounts = new List<Account>();

                    if (!Server.MainDb.Run("SELECT * FROM accounts", out reader)) {
                        Log.Error("Failed to find accounts");
                    } else {
                        while (reader.Read()) {
                            accounts.Add(new Account((int)reader["id"], (string)reader["username"],
                                (string)reader["password"] + "", null));
                        }
                        reader.Close();
                    }


                    accounts.Any(account => {
                        if (account.Username == login.Username && login.Password == account.Password) {
                            Server.UpdateAccountId(socketId, account);
                            Server.SendData(socketId,
                                new AuthenticationRespons(socketId,
                                    AuthenticationRespons.AuthenticationResponses.Success).ToByteArray());
                            return true;
                        } else {
                            Server.SendData(socketId,
                               new AuthenticationRespons(socketId, AuthenticationRespons.AuthenticationResponses.Failed)
                                   .ToByteArray());
                            return false;
                        }
                    });

                    break;
                case PacketHeader.Register:
                    var register = (AccountRegister)packet.Value;

                    accounts = new List<Account>();

                    if (!Server.MainDb.Run("SELECT * FROM accounts", out reader)) {
                        Log.Error("Failed to find accounts");
                    } else {
                        while (reader.Read()) {
                            accounts.Add(new Account((int)reader["id"], (string)reader["username"],
                                (string)reader["password"] + "", null));
                        }
                        reader.Close();
                    }

                    if (accounts.Any(account => account.Username == register.Username)) {
                        Log.Debug("Failed to register account, Username already in use");
                        return 0;
                    }

                    if (Server.MainDb.Run("INSERT INTO `accounts` (`username`, `password`) VALUES " +
                                          "('" + register.Username + "', '" + register.Password + "');")) {
                        Log.Debug("successfully registered account");
                    }
                    break;
                case PacketHeader.RequestCharacters:

                    var characters = new List<Character>();

                    if (!Server.MainDb.Run(string.Format("SELECT * FROM characters where accountId={0};",
                        Server.GetAccountFromSocketId(socketId).AccountId), out reader)) {
                        Log.Debug("Failed to find characters");
                    } else {
                        while (reader.Read()) {
                            var id = (int)reader["id"];
                            var name = (string)reader["characterName"];
                            var level = (int)reader["characterLevel"];
                            var charClass = (int)reader["characterClass"];
                            characters.Add(new Character(id, name, level, (CharacterClasses)charClass));
                        }
                        reader.Close();
                    }

                    Server.SendData(socketId, new RequestCharacters(socketId, characters).ToByteArray());
                    break;
                case PacketHeader.CreateCharacter:
                    var createCharacter = (CreateCharacter)packet.Value;
                    Log.Debug("Creating character with name: " + createCharacter.Name + " and class " + createCharacter.CharClass);

                    characters = new List<Character>();

                    // TODO: Cache this
                    if (!Server.MainDb.Run("SELECT * FROM characters", out reader)) {
                        Log.Debug("Failed to find characters");
                    } else {
                        while (reader.Read()) {
                            var id = (int)reader["id"];
                            var name = (string)reader["characterName"];
                            var level = (int)reader["characterLevel"];
                            var charClass = (int)reader["characterClass"];
                            characters.Add(new Character(id, name, level, (CharacterClasses)charClass));
                        }
                        reader.Close();
                    }

                    if (characters.Any(character => character.Name == createCharacter.Name)) {
                        Server.SendData(socketId, new CreateCharacterRespons(socketId, CreateCharacterRespons.CreateCharacterResponses.NameAlreadyUsed).ToByteArray());
                        return 0;
                    }

                    var createCharacterSql = string.Format(
                    "INSERT INTO characters (accountId, characterName, characterLevel, characterClass) VALUES ({0}, '{1}', 1, {2});",
                    Server.GetAccountFromSocketId(createCharacter.SocketId).AccountId,
                    createCharacter.Name,
                    (int)createCharacter.CharClass);

                    if (Server.MainDb.Run(createCharacterSql)) {
                        Log.Debug("successfully created character");
                        Server.SendData(socketId, new CreateCharacterRespons(socketId, CreateCharacterRespons.CreateCharacterResponses.Success).ToByteArray());
                    } else {
                        Server.SendData(socketId, new CreateCharacterRespons(socketId, CreateCharacterRespons.CreateCharacterResponses.Failed).ToByteArray());
                    }
                    break;
                case PacketHeader.FullCharacterUpdate:
                    var fullCharacterUpdate = (FullCharacterUpdate)packet.Value;

                    FullCharacterUpdate dataToSend = null;

                    if (!Server.MainDb.Run(string.Format("SELECT * FROM characters where id={0}", fullCharacterUpdate.CharacterId), out reader)) {
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

                            dataToSend = new FullCharacterUpdate(socketId, id, name, level, charClass, mapId, x, y, z);
                        }
                        reader.Close();

                        Server.SendData(socketId, dataToSend.ToByteArray());
                    }
                    break;
                default:
                    break;
            }

            return 1;
        }
    }
}
