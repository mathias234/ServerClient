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
                case PacketHeader.GetOtherPlayers:
                    Server.SendPlayers(socketId);
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

                    foreach (var account in accounts) {
                        if (login.Username == account.Username && login.Password == account.Password) {
                            Server.UpdateAccountId(socketId, account);
                            Server.SendData(socketId,
                                new AuthenticationRespons(socketId,
                                    AuthenticationRespons.AuthenticationResponses.Success).ToByteArray());
                        } else
                            Server.SendData(socketId,
                                new AuthenticationRespons(socketId, AuthenticationRespons.AuthenticationResponses.Failed)
                                    .ToByteArray());
                    }

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

                    if (Server.MainDb.Run("INSERT INTO `accounts` (`username`, `password`) VALUES ('" + register.Username + "', '" + register.Password + "');")) {
                        Log.Debug("successfully registered account");
                    }
                    break;
                case PacketHeader.RequestCharacters:
                    Server.SendData(socketId, new RequestCharacters(socketId, new List<Character>() {
                        new Character("First", 50),
                        new Character("Secound", 20)
                    }).ToByteArray());
                    break;
            }

            return 1;
        }
    }
}
