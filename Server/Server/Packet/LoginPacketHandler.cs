using MySql.Data.MySqlClient;
using Shared;
using Shared.Packets;
using System.Collections.Generic;
using System.Linq;

namespace Server.Packet {
    [PacketHandlerAttribute(PacketHeader.Login, PacketHeader.AccountRegister)]
    public class LoginPacketHandler : BasePacketHandler {
        public override void HandlePacket(int socketId, BaseNetworkPacket packet) {
            switch (packet.Header) {
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

                                return;
                            } else {
                                MainServer.SendData(socketId, new AuthenticationRespons(socketId, AuthenticationRespons.AuthenticationResponses.AlreadyLoggedIn).ToByteArray());
                                return;
                            }
                        } else {
                            MainServer.SendData(socketId, new AuthenticationRespons(socketId, AuthenticationRespons.AuthenticationResponses.WrongUsernameAndPassword).ToByteArray());
                            return;
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
                        return;
                    }

                    // fix for probleme where mysql breaks if a password contains '
                    register.Password = register.Password.Replace('\'', '3');

                    if (MainServer.MainDb.Run("INSERT INTO `accounts` (`username`, `password`) VALUES " +
                                          "('" + register.Username + "', '" + register.Password + "');")) {
                        MainServer.SendData(socketId, new RegisterRespons(socketId, RegisterRespons.RegisterResponses.Success).ToByteArray());
                    }
                    break;
            }
        }
    }
}
