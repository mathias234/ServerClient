using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Shared;
using Shared.Packets;


namespace Server {
    public class Server {
        private static byte[] _buffer = new byte[1024];
        private static Dictionary<int, Socket> _clientSockets = new Dictionary<int, Socket>();
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static int lastKey;

        public static Database MainDB;


        public static void Main(string[] args) {
            Console.WriteLine("opening a DB connection");
            MainDB = new Database("localhost", "main", "root", "root");
            SetupServer(3003);


            while (true) {
                var consoleInput = Console.ReadLine();
                if (consoleInput == null) continue;


                switch (consoleInput.ToLower()) {
                    case "stop":
                        MainDB.CloseConnection();
                        return;
                }
            }
        }

        private static void SetupServer(int port) {
            Console.WriteLine("Setting up server...");
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            _serverSocket.Listen(5);
            _serverSocket.BeginAccept(AcceptCallback, null);
        }

        private static void AcceptCallback(IAsyncResult result) {
            var socket = _serverSocket.EndAccept(result);

            _clientSockets.Add(lastKey, socket);


            Console.WriteLine("Client Connected");

            SendNewCharacter(lastKey);

            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, RecievedCallback, lastKey);

            lastKey++;

            _serverSocket.BeginAccept(AcceptCallback, null);
        }

        private static void RecievedCallback(IAsyncResult result) {
            var socketId = (int)result.AsyncState;

            try {
                if (!_clientSockets.ContainsKey(socketId)) return;

                var received = _clientSockets[socketId].EndReceive(result);

                if (received == 0) {
                    CleanupDisconnectedPlayer(socketId);
                    return;
                }

                var dataBuffer = new byte[received];
                Array.Copy(_buffer, dataBuffer, received);


                var packet = PacketCreator.ReadPacket(dataBuffer);

                PacketHandler.HandlePacket(socketId, packet);

                _clientSockets[socketId].BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, RecievedCallback, socketId);
            } catch (SocketException) {
                CleanupDisconnectedPlayer(socketId);
            }
        }

        // This probably should inform the other players that someone disconnected
        public static void CleanupDisconnectedPlayer(int socketId) {
            if (_clientSockets.ContainsKey(socketId)) {
                SendCharacterDisconnect(socketId);
                _clientSockets[socketId].Shutdown(SocketShutdown.Both);
                _clientSockets.Remove(socketId);
            }
        }

        public static void SendData(int socketId, byte[] data) {
            try {
                _clientSockets[socketId].BeginSend(data, 0, data.Length, SocketFlags.None, SendCallbakck, socketId);
            } catch (Exception) {
                // ignored
            }
        }

        private static void SendNewCharacter(int socketId) {
            foreach (var clientSocket in _clientSockets) {
                if (clientSocket.Key == socketId) {
                    var data0 = new Connected(socketId).ToByteArray();
                    SendData(clientSocket.Key, data0);
                    continue;
                }

                var data1 = new NewProxyCharacter(socketId).ToByteArray();
                SendData(clientSocket.Key, data1);
            }
        }

        private static void SendCharacterDisconnect(int socketId) {
            foreach (var clientSocket in _clientSockets) {
                if (clientSocket.Key == socketId) {
                    // this is the socket that disconnected nothing should happend to it
                    continue;
                }

                var data = new CharacterDisconnect(socketId).ToByteArray();
                SendData(clientSocket.Key, data);
            }
        }

        // sends the xyz to all players
        public static void SendMovement(int socketId, NetworkVector3 vector3, float yRotation, float forward, float turn, bool crouch, bool onGround, float jump, float jumpLeg) {
            foreach (var clientSocket in _clientSockets) {
                if (clientSocket.Key == socketId) {
                    // this is the socket that sent the movement
                    continue;
                }

                var data = new Movement(socketId, vector3, yRotation, forward, turn, crouch, onGround, jump, jumpLeg).ToByteArray();
                SendData( clientSocket.Key, data);
            }
        }

        public static void SendPlayers(int socketId) {
            var players = new List<int>();

            foreach (var clientSocket in _clientSockets) {
                if (clientSocket.Key == socketId) continue;
                players.Add(clientSocket.Key);
            }

            var data = PacketCreator.CreatePacket(players);
            SendData(socketId, data);
        }

        private static void SendCallbakck(IAsyncResult result) {
            var socketId = (int)result.AsyncState;
            _clientSockets[socketId].EndSend(result);
        }
    }
}