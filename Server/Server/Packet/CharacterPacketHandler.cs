using MySql.Data.MySqlClient;
using Shared;
using Shared.Packets;
using System.Collections.Generic;
using System.Linq;

namespace Server.Packet {
    /// <summary>
    /// Handles all character creation and selection
    /// </summary>
    [PacketHandlerAttribute(PacketHeader.Movement, PacketHeader.FullCharacterUpdate, PacketHeader.ConnectedToMap, PacketHeader.ChangeMap)]
    public class CharacterPacketHandler : BasePacketHandler {
        public override void HandlePacket(int socketId, BaseNetworkPacket packet) {
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
            }
        }
    }
}
