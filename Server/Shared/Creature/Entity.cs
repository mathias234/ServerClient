using System.Collections.Generic;
using System.Linq;

namespace Shared.Creature {
    public class Entity {
        public static List<Entity> Entities = new List<Entity>();

        public int EntityId { get; set; }

        public NetworkVector3 Position { get; set; }

        public bool IsPlayer;

        public void Init(bool isPlayer) {
            EntityId = Entities.Count + 1;
            IsPlayer = isPlayer;

            Entities.Add(this);
        }

        public static Entity GetEntity(int entityId) {
            return Entities.FirstOrDefault(e => e.EntityId == entityId);
        }

        public static List<Entity> GetPlayerEntities() {
            return Entities.FindAll(e => {
                if (e != null)
                    if (e.IsPlayer)
                        return true;
                return false;
            });
        }
    }
}
