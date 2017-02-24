using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Shared;
using Shared.Packets;
using System.Linq;
using System.Reflection;
using Server.WorldHandlers;
using System.Threading;

namespace Server {
    public class MainServer {
        private static int maxBufferSize = 1024;
        private static byte[] _buffer = new byte[maxBufferSize];

        private static Dictionary<int, Account> _clientSockets = new Dictionary<int, Account>();

        private static Socket _serverSocket;

        public static int LastKey;

        public static Database MainDb;

        public static int timeSinceLastSave;

        public static void Start() {
            Log.Debug("Opening a DB connection");
            MainDb = new Database("localhost", "main", "root", "root");

            InitializeHandlers();

            _clientSockets = new Dictionary<int, Account>();
            SetupServer(3003);

            CleanUpCrash();

        }

        private static void InitializeHandlers() {
            var baseType = typeof(WorldHandler);
            var handlers = Assembly.GetAssembly(baseType).GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t));

            foreach (var handler in handlers) {
                Log.Debug("Starting " + handler.Name.ToString());

                WorldHandler initializedHandler = (WorldHandler)Activator.CreateInstance(handler);
                initializedHandler.Start();
                Log.Debug(handler.Name.ToString() + " Started");
            }
        }

        // run this on start in case the server crashed last run
        public static void CleanUpCrash() {
            MainDb.Run("UPDATE accounts SET isOnline='0'");
        }

        public static void Save() {
            foreach (var accounts in _clientSockets) {
                Log.Debug("Saving");
                var DbChar = new DbCharacter(accounts.Value.CharacterOnline);
                DbChar.SaveToDb();
            }
        }

        private static void SetupServer(int port) {
            Log.Debug("Setting up server...");
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));

            _serverSocket.Listen(5);
            _serverSocket.BeginAccept(AcceptCallback, null);
            Log.Debug("Server Started");
        }

        private static void AcceptCallback(IAsyncResult result) {
            var socket = _serverSocket.EndAccept(result);

            _clientSockets.Add(LastKey, /* a temporary account */ new Account(-1, "", "", socket, true));

            Log.Debug("Client Connected");

            SendData(LastKey, new Connected(LastKey).ToByteArray());

            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, RecievedCallback, LastKey);

            LastKey++;

            _serverSocket.BeginAccept(AcceptCallback, null);
        }

        private static void RecievedCallback(IAsyncResult result) {
            var socketId = (int)result.AsyncState;

            try {
                if (!_clientSockets.ContainsKey(socketId)) return;

                var received = _clientSockets[socketId].Socket.EndReceive(result);

                if (received == 0) {
                    CleanupDisconnectedPlayer(socketId);
                    return;
                }

                var dataBuffer = new byte[received];
                Array.Copy(_buffer, dataBuffer, received);


                var packet = PacketCreator.ReadPacket(dataBuffer);

                PacketHandler.HandlePacket(socketId, packet);

                _clientSockets[socketId].Socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, RecievedCallback, socketId);
            } catch (SocketException) {
                CleanupDisconnectedPlayer(socketId);
            }
        }

        // This probably should inform the other players that someone disconnected
        public static void CleanupDisconnectedPlayer(int socketId) {
            if (_clientSockets.ContainsKey(socketId)) {
                new DbCharacter(_clientSockets[socketId].CharacterOnline).SaveToDb();
                SendCharacterDisconnect(socketId);
                _clientSockets[socketId].Socket.Shutdown(SocketShutdown.Both);

                _clientSockets[socketId].IsOnline = false;

                DbAccount dbAccount = new DbAccount(_clientSockets[socketId]);
                dbAccount.SaveToDb();

                _clientSockets.Remove(socketId);
            }
        }

        public static void SendData(int socketId, byte[] data) {
            try {
                _clientSockets[socketId].Socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallbakck, socketId);
            } catch (Exception) {
                // ignored
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
            try {
                foreach (var clientSocket in _clientSockets) {
                    if (clientSocket.Key == socketId) {
                        // this is the socket that sent the movement
                        continue;
                    }

                    var data = new Movement(socketId, vector3, yRotation, forward, turn, crouch, onGround, jump, jumpLeg).ToByteArray();
                    SendData(clientSocket.Key, data);
                }
            } catch {
            }
        }

        /// <summary>
        /// Will keep the socket
        /// Make sure you update this when editing Account.cs
        /// </summary>
        /// <param name="socketId"></param>
        /// <param name="newAccount"></param>
        public static void UpdateAccountId(int socketId, Account newAccount) {
            _clientSockets[socketId].AccountId = newAccount.AccountId;
            _clientSockets[socketId].Username = newAccount.Username;
            _clientSockets[socketId].Password = newAccount.Password;
            _clientSockets[socketId].CharacterOnline = newAccount.CharacterOnline;
            _clientSockets[socketId].IsOnline = newAccount.IsOnline;
        }

        public static Account GetAccountFromSocketId(int socketId) {
            if (_clientSockets.ContainsKey(socketId))
                return _clientSockets[socketId];
            else
                return null;
        }

        public static int GetSocketIdFromAccountId(int accountId) {
            var account = _clientSockets.First(keyValue => {
                if (keyValue.Value.AccountId == accountId)
                    return true;
                return false;
            });

            return account.Key;
        }

        public static List<Account> GetAllAccounts() {
            List<Account> accounts = new List<Account>();
            foreach (var client in _clientSockets) {
                accounts.Add(client.Value);
            }

            return accounts;
        }

        private static void SendCallbakck(IAsyncResult result) {
            var socketId = (int)result.AsyncState;
            _clientSockets[socketId].Socket.EndSend(result);
        }
    }
}