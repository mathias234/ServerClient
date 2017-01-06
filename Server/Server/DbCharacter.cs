using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    public class DbCharacter {
        public Character Character;

        public DbCharacter(Character character) {
            Character = character;
        }

        public void SaveToDb() {
            var query = string.Format("UPDATE characters SET characterName='{1}', characterLevel={2}, characterClass={3}, mapId={4}, x={5}, y={6}, z={7} WHERE id={0};",
                Character.CharacterId,
                Character.Name,
                Character.Level,
                (int)Character.Class,
                Character.MapId,
                Character.X,
                Character.Y,
                Character.Z);

            Server.MainDb.Run(query);
        }

        public static Character GetFromDb() {

            return new Character(0, 0, "", 0, CharacterClasses.Agent, 0, 0, 0, 0);
        }
    }
}
