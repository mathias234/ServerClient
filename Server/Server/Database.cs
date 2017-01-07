using System;
using MySql.Data.MySqlClient;

namespace Server {
    public class Database {
        private MySqlConnection _connection;
        private readonly string _server;
        private readonly string _database;
        private readonly string _uid;
        private readonly string _password;

        public Database(string server, string database, string uid, string password) {
            _server = server;
            _database = database;
            _uid = uid;
            _password = password;

            Connect();
        }

        private void Connect() {
            _connection =
                new MySqlConnection("SERVER=" + _server + "; DATABASE=" + _database + "; UID=" + _uid + "; PASSWORD=" +
                                    _password + ";");
            try {
                _connection.Open();
            } catch (MySqlException ex) {
                switch (ex.Number) {
                    case 0:
                        Log.Error("Cannot connect to server.  Contact administrator");
                        break;
                    case 1045:
                        Log.Error("Invalid username/password, please try again");
                        break;
                }
            }
        }

        public bool Run(string sqlString, out MySqlDataReader reader) {
            Log.Debug("running query: " + sqlString);

            try {
                var cmd = new MySqlCommand(sqlString, _connection);
                cmd.ExecuteNonQuery();

                if (sqlString.ToLower().Contains("select")) {
                    reader = cmd.ExecuteReader();
                    return true;
                }

                reader = null;
                return true;
            } catch (Exception ex) {
                Log.Error("Failed to run SQL: " + sqlString + "\n Error: " + ex.Message);
                reader = null;
                return false;
            }
        }


        public bool Run(string sqlString) {
            var result = Run(sqlString, out var reader);
            if (reader != null)
                reader.Close();
            return result;
        }

        public bool CloseConnection() {
            try {
                _connection.Close();
                return true;
            } catch (MySqlException ex) {
                Log.Error(ex.Message);
                return false;
            }
        }
    }
}