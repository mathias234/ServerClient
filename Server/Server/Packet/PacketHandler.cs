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
        public static int HandlePacket(int socketId, BaseNetworkPacket packet) {
            if (packet?.Header == null)
                return 0;

            switch (packet.Header) {
                case PacketHeader.Movement:
                    var movement = (Movement)packet;

                    var tempCharacter = MainServer.GetAccountFromSocketId(movement.SocketId).CharacterOnline;

                    tempCharacter.X = movement.NewPosition.X;
                    tempCharacter.Y = movement.NewPosition.Y;
                    tempCharacter.Z = movement.NewPosition.Z;

                    MainServer.GetAccountFromSocketId(movement.SocketId).CharacterOnline = tempCharacter;

                    // send this new position to all players
                    MainServer.SendMovement(movement.SocketId, movement.NewPosition, movement.YRotation, movement.Forward,
                        movement.Turn, movement.Crouch, movement.OnGround, movement.Jump, movement.JumpLeg);
                    break;
                case PacketHeader.Login:
                    var login = (Login)packet;

                    MySqlDataReader reader;

                    var accounts = new List<Account>();

                    if (!MainServer.MainDb.Run("SELECT * FROM accounts", out reader)) {
                        Log.Error("Failed to find accounts");
                    } else {
                        while (reader.Read()) {
                            accounts.Add(new Account((int)reader["id"], (string)reader["username"],
                                (string)reader["password"] + "", null, ((int)reader["isOnline"]) == 1 ? true : false));
                        }
                        reader.Close();
                    }

                    // fix for probleme where mysql breaks if a password contains '
                    login.Password = login.Password.Replace('\'', '3');

                    foreach (var account in accounts) {
                        if (account.Username == login.Username && login.Password == account.Password) {
                            if (!account.IsOnline) {
                                account.IsOnline = true;
                                MainServer.UpdateAccountId(socketId, account);
                                MainServer.SendData(socketId,
                                    new AuthenticationRespons(socketId,
                                        AuthenticationRespons.AuthenticationResponses.Success).ToByteArray());

                                DbAccount dbAccount = new DbAccount(account);
                                dbAccount.SaveToDb();

                                return 1;
                            } else {
                                MainServer.SendData(socketId, new AuthenticationRespons(socketId, AuthenticationRespons.AuthenticationResponses.AlreadyLoggedIn).ToByteArray());
                                return 1;
                            }
                        } else {
                            MainServer.SendData(socketId, new AuthenticationRespons(socketId, AuthenticationRespons.AuthenticationResponses.WrongUsernameAndPassword).ToByteArray());
                            return 1;
                        }
                    }

                    MainServer.SendData(socketId, new AuthenticationRespons(socketId, AuthenticationRespons.AuthenticationResponses.Failed).ToByteArray());

                    break;
                case PacketHeader.AccountRegister:
                    var register = (AccountRegister)packet;

                    accounts = new List<Account>();

                    if (!MainServer.MainDb.Run("SELECT * FROM accounts", out reader)) {
                        Log.Error("Failed to find accounts");
                    } else {
                        while (reader.Read()) {
                            accounts.Add(new Account((int)reader["id"], (string)reader["username"],
                                (string)reader["password"] + "", null, ((int)reader["isOnline"]) == 1 ? true : false));
                        }
                        reader.Close();
                    }

                    if (accounts.Any(account => account.Username == register.Username)) {
                        MainServer.SendData(socketId,
                            new RegisterRespons(socketId, RegisterRespons.RegisterResponses.UsernameAlreadyInUse).ToByteArray());
                        return 0;
                    }

                    // fix for probleme where mysql breaks if a password contains '
                    register.Password = register.Password.Replace('\'', '3');

                    if (MainServer.MainDb.Run("INSERT INTO `accounts` (`username`, `password`) VALUES " +
                                          "('" + register.Username + "', '" + register.Password + "');")) {
                        MainServer.SendData(socketId, new RegisterRespons(socketId, RegisterRespons.RegisterResponses.Success).ToByteArray());
                    }
                    break;
                case PacketHeader.RequestCharacters:
                    var characters = new List<Character>();

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
                        return 0;
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
                case PacketHeader.FullCharacterUpdate:
                    var fullCharacterUpdate = (FullCharacterUpdate)packet;

                    FullCharacterUpdate dataToSend = null;

                    var dbCharacter = DbCharacter.GetFromDb(fullCharacterUpdate.NewCharacter.CharacterId);
                    dbCharacter.SocketId = socketId;

                    MainServer.GetAccountFromSocketId(socketId).CharacterOnline = dbCharacter;

                    dataToSend = new FullCharacterUpdate(socketId, dbCharacter);
                    MainServer.SendData(socketId, dataToSend.ToByteArray());

                    break;
                case PacketHeader.ConnectedToMap:
                    var connectedToMap = (ConnectedToMap)packet;

                    // Send all players online
                    var accountsInMap = MainServer.GetAllAccounts().FindAll(account => {
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

                    MainServer.SendData(socketId, new CharactersInMap(socketId, charactersInMap).ToByteArray());

                    foreach (var account in MainServer.GetAllAccounts()) {
                        MainServer.SendData(MainServer.GetSocketIdFromAccountId(account.AccountId), new NotifyOtherPlayerMapChange(socketId, -1, MainServer.GetAccountFromSocketId(socketId).CharacterOnline).ToByteArray());
                    }

                    Callback.CallPlayerEnteredMap(socketId, connectedToMap.MapId);

                    break;
                case PacketHeader.ChangeMap:
                    var changeMap = (ChangeMap)packet;

                    MainServer.GetAccountFromSocketId(socketId).CharacterOnline.MapId = changeMap.NewMapId;
                    MainServer.GetAccountFromSocketId(socketId).CharacterOnline.X = changeMap.NewX;
                    MainServer.GetAccountFromSocketId(socketId).CharacterOnline.Y = changeMap.NewY;
                    MainServer.GetAccountFromSocketId(socketId).CharacterOnline.Z = changeMap.NewZ;

                    DbCharacter dbChar = new DbCharacter(MainServer.GetAccountFromSocketId(socketId).CharacterOnline);
                    dbChar.SaveToDb();

                    MainServer.SendData(socketId, new FullCharacterUpdate(socketId, MainServer.GetAccountFromSocketId(socketId).CharacterOnline).ToByteArray());

                    foreach (var account in MainServer.GetAllAccounts()) {
                        MainServer.SendData(MainServer.GetSocketIdFromAccountId(account.AccountId), new NotifyOtherPlayerMapChange(socketId, changeMap.OldMapId, MainServer.GetAccountFromSocketId(socketId).CharacterOnline).ToByteArray());
                    }

                    break;
                default:
                    break;
            }

            return 1;
        }
    }
}
