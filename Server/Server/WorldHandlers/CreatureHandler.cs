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

            Callback.PlayerLogin += Callback_PlayerLogin;
            Callback.PlayerEnteredMap += Callback_PlayerEnteredMap;

            foreach (var creature in _spawnedCreatures) {
                Thread t = new Thread(new ParameterizedThreadStart(MoveCreature));
                t.Start(creature.Value);
            }
        }

        // send the creature templates to this player
        private void Callback_PlayerLogin(int socketId) {
            List<CreatureTemplate> creatureTemplates = new List<CreatureTemplate>();

            foreach (var creature in _creatureTemplates) {
                creatureTemplates.Add(creature.Value);
            }

            MainServer.SendData(socketId, new CreatureTemplates(socketId, creatureTemplates).ToByteArray());
        }

        DateTime lastCreatureMove = DateTime.Now;

        private void MoveCreature(object param) {
            while (true) {
                // make sure we get an updated version if its been modified elsewhere which is very likely
                var creature = _spawnedCreatures[((InstancedCreature)param).InstanceId];

                switch (creature.state) {
                    case CreatureState.Idle:
                        break;
                    case CreatureState.WalkingWaypoints:
                        WalkWaypoint(creature);
                        break;
                    case CreatureState.FollowingPlayer:
                        FollowPlayer(0, creature);
                        break;
                    default:
                        break;
                }
            }
        }

        public void FollowPlayer(int playerId, InstancedCreature creature) {
            Account player = MainServer.GetAccountFromSocketId(playerId);

            if (player == null || player.CharacterOnline == null) {
                Thread.Sleep(5000);
                return;
            }

            MoveCreature(creature.InstanceId, player.CharacterOnline.X, player.CharacterOnline.Y, player.CharacterOnline.Z, 2);
            Thread.Sleep(500);
        }

        public void WalkWaypoint(InstancedCreature creature) {
            if (creature.Waypoints == null || creature.Waypoints.Count <= 0) {
                creature.state = CreatureState.Idle;
                return;
            }

            if (creature.Waypoints.Count <= creature.LastWaypoint + 1) {
                creature.LastWaypoint = -1;
            } else {
                creature.LastWaypoint++;
                var currentWaypoint = creature.Waypoints[creature.LastWaypoint];

                var newX = currentWaypoint.X;
                var newZ = currentWaypoint.Z;

                MoveCreature(creature.InstanceId, newX, creature.Y, newZ, 2);

                Thread.Sleep(currentWaypoint.StayTime);
            }
        }

        public void MoveCreature(int InstanceId, float x, float y, float z, float speed) {
            var creature = _spawnedCreatures[InstanceId];

            foreach (var players in MainServer.GetAllAccounts()) {
                var socketId = MainServer.GetSocketIdFromAccountId(players.AccountId);

                MainServer.SendData(
                    socketId,
                    new MoveCreature(socketId, creature.InstanceId, x, y, z, speed).ToByteArray());
            }

            creature.X = x;
            creature.Y = y;
            creature.Z = z;
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
                    var creature = new InstancedCreature(
                        (int)reader["instanceId"],
                        (int)reader["templateId"],
                        (float)reader["x"],
                        (float)reader["y"],
                        (float)reader["z"],
                        (int)reader["mapId"]);


                    _spawnedCreatures.Add(creature.InstanceId, creature);
                }
                reader.Close();

                foreach (var creature in _spawnedCreatures) {
                    creature.Value.Waypoints = GetWaypointsForCreature(creature.Value.InstanceId);
                }
            }
        }

        public List<Waypoint> GetWaypointsForCreature(int instanceid) {
            var waypoints = new List<Waypoint>();

            if (!MainServer.MainDb.Run("SELECT * FROM creature_waypoints where instanceid=" + instanceid + "", out var reader)) {
                Log.Error("Failed to find waypoints");
            } else {
                while (reader.Read()) {
                    var waypoint = new Waypoint(
                        (int)reader["stayTime"],
                        (float)reader["x"],
                        (float)reader["y"],
                        (float)reader["z"]);

                    waypoints.Add(waypoint);
                }
                reader.Close();
            }

            return waypoints;
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
