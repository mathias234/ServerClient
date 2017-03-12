using System;
using System.Collections.Generic;

namespace Shared.Creature {
    public enum CreatureState {
        Idle,
        WalkingWaypoints,
        FollowingPlayer
    }

    [Serializable]
    public class InstancedCreature : Entity {
        public int InstanceId { get; set; }
        public int TemplateId { get; set; }
        public int MapId { get; set; }

        public List<Waypoint> Waypoints { get; set; }
        public int LastWaypoint;

        public CreatureState state;

        public int TargetEntityId; // the entity id of the target

        public InstancedCreature() {
            Init(false);

            state = CreatureState.Idle;
        }

        public InstancedCreature(int instanceId, int templateId, NetworkVector3 position, int mapId) {
            Init(false);

            InstanceId = instanceId;
            TemplateId = templateId;
            Position = position;
            MapId = mapId;
            state = CreatureState.Idle;
        }
    }
}
