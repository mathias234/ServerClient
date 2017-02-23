using System.Collections.Generic;

namespace Shared.Creature {
    public class InstancedCreature {
        public int InstanceId { get; set; }
        public int TemplateId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public int MapId { get; set; }

        public List<Waypoint> Waypoints { get; set; } 

        public InstancedCreature() {

        }

        public InstancedCreature(int instanceId, int templateId, float x, float y, float z, int mapId) {
            InstanceId = instanceId;
            TemplateId = templateId;
            X = x;
            Y = y;
            Z = z;
            MapId = mapId;
        }
    }
}
