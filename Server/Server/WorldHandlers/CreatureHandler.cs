using MySql.Data.MySqlClient;
using Server.Creature;
using System;
using System.Collections.Generic;

namespace Server.WorldHandlers {
    public class CreatureHandler : WorldHandler {
        private Dictionary<int, CreatureTemplate> _creatureTemplates = new Dictionary<int, CreatureTemplate>();
        private Dictionary<int, InstancedCreature> _spawnedCreatures = new Dictionary<int, InstancedCreature>();

        public override void Start() {
            LoadCreatureTemplatesFromDB();
            GetSpawnedCreaturesFromDB();

            Callback.PlayerEnteredMap += Callback_PlayerEnteredMap;
        }

        private void Callback_PlayerEnteredMap(int playerId, int MapId) {
            Log.Debug("player with id: " + playerId + " entered map" + MapId);
        }

        private void LoadCreatureTemplatesFromDB() {
            MySqlDataReader reader;

            if (!MainServer.MainDb.Run("SELECT * FROM creaturetemplate", out reader)) {
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
            MySqlDataReader reader;

            if (!MainServer.MainDb.Run("SELECT * FROM spawnedcreatures", out reader)) {
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

        public static List<InstancedCreature> GetCreaturesInMap(int mapId) {
            var creatures = new List<InstancedCreature>();

            foreach (var creature in ((CreatureHandler)instance)._spawnedCreatures) {
                if (creature.Value.MapId == mapId) {
                    creatures.Add(creature.Value);
                }
            }

            return creatures;
        }
    }
}
