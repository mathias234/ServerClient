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

                    //Console.WriteLine("socketId" + socketId + "x: " + movement.NewPosition.X + " y: " + movement.NewPosition.Y + " z: " + movement.NewPosition.Z);

                    // send this new position to all players
                    Server.SendMovement(movement.SocketId, movement.NewPosition, movement.YRotation, movement.Forward, movement.Turn, movement.Crouch, movement.OnGround, movement.Jump, movement.JumpLeg);
                    break;
                case PacketHeader.GetOtherPlayers:
                    Server.SendPlayers(socketId);
                    break;
                case PacketHeader.Login:
                    var login = (Login)packet.Value;

                    Console.WriteLine(login.Username + ":" + login.Password);

                    MySqlDataReader reader;

                    var usernamesPasswords = new Dictionary<string, string>();

                    if (!Server.MainDB.Run("SELECT * FROM accounts", out reader)) {
                        Console.WriteLine("Failed to find accounts");
                    } else {
                        while (reader.Read()) {
                            usernamesPasswords.Add(reader["username"] + "", reader["password"] + "");
                        }
                        reader.Close();
                    }

                    foreach (var usernamesPassword in usernamesPasswords) {
                        if (login.Username == usernamesPassword.Key && login.Password == usernamesPassword.Value)
                            Server.SendData(socketId, new AuthenticationRespons(socketId, AuthenticationRespons.AuthenticationResponses.Success).ToByteArray());
                        else
                            Server.SendData(socketId, new AuthenticationRespons(socketId, AuthenticationRespons.AuthenticationResponses.Failed).ToByteArray());
                    }

                    break;
                case PacketHeader.Register:
                    var register = (AccountRegister)packet.Value;

                    if (Server.MainDB.Run("INSERT INTO `accounts` (`username`, `password`) VALUES ('" + register.Username + "', '" + register.Password + "');")) {
                        Console.WriteLine("successfully registered account");
                    }

                    break;
            }

            return 1;
        }
    }
}
