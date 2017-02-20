using MySql.Data.MySqlClient;
using Shared.Creature;
using Shared.Packets;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Server.WorldHandlers {
    public class CreatureHandler : WorldHandler {
        private Dictionary<int, CreatureTemplate> _creatureTemplates = new Dictionary<int, CreatureTemplate>();
        private Dictionary<int, InstancedCreature> _spawnedCreatures = new Dictionary<int, InstancedCreature>();

        public override void Start() {
            LoadCreatureTemplatesFromDB();
            GetSpawnedCreaturesFromDB();

            Callback.PlayerEnteredMap += Callback_PlayerEnteredMap;

            Thread moveCreaturesThread = new Thread(MoveCreatures);
            moveCreaturesThread.Start();
        }

        DateTime lastCreatureMove = DateTime.Now;

        private void MoveCreatures() {
            while (true) {
                lastCreatureMove = DateTime.Now;

                foreach (var creature in _spawnedCreatures) {
                    foreach (var players in MainServer.GetAllAccounts()) {
                        var socketId = MainServer.GetSocketIdFromAccountId(players.AccountId);

                        var newX = new Random(DateTime.Now.Millisecond + 303323 + +creature.Value.InstanceId).Next(-5, 5);
                        var newZ = new Random(DateTime.Now.Millisecond + 5423 + creature.Value.InstanceId).Next(-5, 5);

                        MainServer.SendData(
                            socketId,
                            new MoveCreature(socketId, creature.Value.InstanceId, newX, 0, newZ).ToByteArray());
                    }
                }

                Thread.Sleep(5000);
            }
        }

        private void Callback_PlayerEnteredMap(int socketId, int MapId) {
            List<InstancedCreature> creaturesInMap = GetCreaturesInMap(MapId);

            if (creaturesInMap.Count == 0) {
                Log.Warning("No Creatures in map: " + MapId);
                return;
            }

            MainServer.SendData(socketId, new CreaturesInMap(socketId, creaturesInMap).ToByteArray());
        }

        private void LoadCreatureTemplatesFromDB() {
            if (!MainServer.MainDb.Run("SELECT * FROM creaturetemplate", out var reader)) {
                Log.Error("Failed to find creature templates");
            } else {
                while (reader.Read()) {
                    _creatureTemplates.Add((int)reader["templateId"], new CreatureTemplate(
                                                                   (int)reader["templateId"],
                                                                   (string)reader["name"],
                                                                   (int)reader["health"],
                                                                   (int)reader["minLevel"],
                                                                   (int)reader["maxLevel"]));
                }
                reader.Close();
            }
        }

        private void GetSpawnedCreaturesFromDB() {
            if (!MainServer.MainDb.Run("SELECT * FROM spawnedcreatures", out var reader)) {
                Log.Error("Failed to find spawned creatures");
            } else {
                while (reader.Read()) {
                    _spawnedCreatures.Add((int)reader["instanceId"], new InstancedCreature(
                                                                (int)reader["instanceId"],
                                                                (int)reader["templateId"],
                                                                (float)reader["x"],
                                                                (float)reader["y"],
                                                                (float)reader["z"],
                                                                (int)reader["mapId"]));
                }
                reader.Close();
            }
        }

        public List<InstancedCreature> GetCreaturesInMap(int mapId) {
            var creatures = new List<InstancedCreature>();

            var spawnedCreatures = _spawnedCreatures;

            if (spawnedCreatures == null) {
                Log.Error("Failed to get creatures");
                return creatures;
            }

            foreach (var creature in spawnedCreatures) {
                if (creature.Value.MapId == mapId) {
                    creatures.Add(creature.Value);
                }
            }

            return creatures;
        }
    }
}
