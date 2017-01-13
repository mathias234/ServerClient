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
        private static Character tempCharacter; // useful for storing temporary copy of a character without creating a new instance(saving a few bytes of memory :P)

        public static int HandlePacket(int socketId, Packet packet) {
            if (packet?.Header == null)
                return 0;

            switch (packet.Header) {
                case PacketHeader.Movement:
                    var movement = (Movement)packet.Value;

                    tempCharacter = Server.GetAccountFromSocketId(movement.SocketId).CharacterOnline;

                    tempCharacter.X = movement.NewPosition.X;
                    tempCharacter.Y = movement.NewPosition.Y;
                    tempCharacter.Z = movement.NewPosition.Z;

                    Server.GetAccountFromSocketId(movement.SocketId).CharacterOnline = tempCharacter;

                    // send this new position to all players
                    Server.SendMovement(movement.SocketId, movement.NewPosition, movement.YRotation, movement.Forward,
                        movement.Turn, movement.Crouch, movement.OnGround, movement.Jump, movement.JumpLeg);
                    break;
                case PacketHeader.Login:
                    var login = (Login)packet.Value;

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

                    foreach (var account in accounts) {
                        if (account.Username == login.Username && login.Password == account.Password) {
                            Server.UpdateAccountId(socketId, account);
                            Server.SendData(socketId,
                                new AuthenticationRespons(socketId,
                                    AuthenticationRespons.AuthenticationResponses.Success).ToByteArray());
                            return 1;
                        }
                    }

                    Server.SendData(socketId, new AuthenticationRespons(socketId, AuthenticationRespons.AuthenticationResponses.Failed).ToByteArray());

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
                        Server.SendData(socketId,
                            new RegisterRespons(socketId, RegisterRespons.RegisterResponses.UsernameAlreadyInUse).ToByteArray());
                        return 0;
                    }

                    // fix for probleme where mysql breaks if a password contains '
                    register.Password = register.Password.Replace('\'', '3');

                    if (Server.MainDb.Run("INSERT INTO `accounts` (`username`, `password`) VALUES " +
                                          "('" + register.Username + "', '" + register.Password + "');")) {
                        Server.SendData(socketId, new RegisterRespons(socketId, RegisterRespons.RegisterResponses.Success).ToByteArray());
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
                            var mapId = (int)reader["mapId"];
                            var x = (float)reader["x"];
                            var y = (float)reader["y"];
                            var z = (float)reader["z"];

                            characters.Add(new Character(socketId, id, name, level, (CharacterClasses)charClass, mapId, x, y, z));
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
                            var mapId = (int)reader["mapId"];
                            var x = (float)reader["x"];
                            var y = (float)reader["y"];
                            var z = (float)reader["z"];

                            characters.Add(new Character(-1, id, name, level, (CharacterClasses)charClass, mapId, x, y, z));
                        }
                        reader.Close();
                    }

                    if (characters.Any(character => character.Name == createCharacter.Name)) {
                        Server.SendData(socketId, new CreateCharacterRespons(socketId, CreateCharacterRespons.CreateCharacterResponses.NameAlreadyUsed).ToByteArray());
                        return 0;
                    }

                    var createCharacterSql = string.Format(
                    "INSERT INTO characters (accountId, characterName, characterLevel, characterClass, mapId, x, y, z) VALUES ({0}, '{1}', 1, {2}, {3}, {4}, {5}, {6});",
                    Server.GetAccountFromSocketId(createCharacter.SocketId).AccountId,
                    createCharacter.Name,
                    (int)createCharacter.CharClass,
                    // This data is where the player will start. TODO: Remove hardcoding
                    0,
                    0,
                    23.82,
                    10.7);

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

                    var dbCharacter = DbCharacter.GetFromDb(fullCharacterUpdate.NewCharacter.CharacterId);
                    dbCharacter.SocketId = socketId;

                    Server.GetAccountFromSocketId(socketId).CharacterOnline = dbCharacter;

                    dataToSend = new FullCharacterUpdate(socketId, dbCharacter);
                    Server.SendData(socketId, dataToSend.ToByteArray());

                    break;
                case PacketHeader.ConnectedToMap:
                    var connectedToMap = (ConnectedToMap)packet.Value;

                    // Send all players online
                    var accountsInMap = Server.GetAllAccounts().FindAll(account => {
                        if (account.CharacterOnline != null)
                            if (account.CharacterOnline.MapId == connectedToMap.MapId)
                                return true;
                            else
                                return false;
                        else
                            return false;
                    });
                    var charactersInMap = new List<Character>();

                    foreach (var account in accountsInMap) {
                        if (account.CharacterOnline != null)
                            charactersInMap.Add(account.CharacterOnline);
                    }

                    Server.SendData(socketId, new CharactersInMap(socketId, charactersInMap).ToByteArray());
                    Server.GetAccountFromSocketId(socketId).CharacterOnline.MapId = connectedToMap.MapId;

                    foreach (var account in Server.GetAllAccounts()) {
                        Server.SendData(Server.GetSocketIdFromAccountId(account.AccountId), new NotifyOtherPlayerMapChange(socketId, -1, Server.GetAccountFromSocketId(socketId).CharacterOnline).ToByteArray());
                    }

                    break;
                case PacketHeader.ChangeMap:
                    var changeMap = (ChangeMap)packet.Value;

                    Server.GetAccountFromSocketId(socketId).CharacterOnline.MapId = changeMap.NewMapId;
                    Server.GetAccountFromSocketId(socketId).CharacterOnline.X = changeMap.NewX;
                    Server.GetAccountFromSocketId(socketId).CharacterOnline.Y = changeMap.NewY;
                    Server.GetAccountFromSocketId(socketId).CharacterOnline.Z = changeMap.NewZ;

                    DbCharacter dbChar = new DbCharacter(Server.GetAccountFromSocketId(socketId).CharacterOnline);
                    dbChar.SaveToDb();

                    Server.SendData(socketId, new FullCharacterUpdate(socketId, Server.GetAccountFromSocketId(socketId).CharacterOnline).ToByteArray());

                    foreach (var account in Server.GetAllAccounts()) {
                        Server.SendData(Server.GetSocketIdFromAccountId(account.AccountId), new NotifyOtherPlayerMapChange(socketId, changeMap.OldMapId, Server.GetAccountFromSocketId(socketId).CharacterOnline).ToByteArray());
                    }

                    break;
                default:
                    break;
            }

            return 1;
        }
    }
}
