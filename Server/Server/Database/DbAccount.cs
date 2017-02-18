using MySql.Data.MySqlClient;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    public class DbAccount {
        public Account Account;

        public DbAccount(Account account) {
            Account = account;
        }

        public void SaveToDb() {
            if (Account == null) { return; }

            var query = string.Format("UPDATE accounts SET username='{1}', password='{2}', isOnline='{3}'  WHERE id={0};",
                Account.AccountId,
                Account.Username,
                Account.Password,
                Account.IsOnline == true ? 1 : 0);

            MainServer.MainDb.Run(query);
        }

        public static Account GetFromDb(int accountId) {
            if (!MainServer.MainDb.Run(string.Format("SELECT * FROM account where id={0}", accountId), out var reader)) {
                Log.Debug("Failed to find characters");
            } else {
                while (reader.Read()) {
                    var id = (int)reader["id"];
                    var username = (string)reader["characterName"];
                    var password = (string)reader["characterLevel"];
                    var isOnline = (int)reader["characterClass"];

                    reader.Close();

                    return new Account(id, username, password, null, isOnline == 1 ? true : false);
                }
            }

            return null;
        }
    }
}
