namespace Server.Creature {
    public class InstancedCreature {
        public int InstanceId;
        public int TemplateId;
        public float X;
        public float Y;
        public float Z;
        public int MapId;

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
