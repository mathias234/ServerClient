using MySql.Data.MySqlClient;
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
            if (Character == null) { return; }

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

        public static Character GetFromDb(int charId) {
            if (!Server.MainDb.Run(string.Format("SELECT * FROM characters where id={0}", charId), out var reader)) {
                Log.Debug("Failed to find characters");
            } else {
                while (reader.Read()) {
                    var id = (int)reader["id"];
                    var name = (string)reader["characterName"];
                    var level = (int)reader["characterLevel"];
                    var charClass = (int)reader["characterClass"];
                    var mapId = (int)reader["mapId"];
                    var x = (float)reader["x"];
                    var y = (float)reader["y"];
                    var z = (float)reader["z"];

                    reader.Close();

                    return new Character(-1, id, name, level, (CharacterClasses)charClass, mapId, x, y, z);
                }
            }

            return null;
        }
    }
}
