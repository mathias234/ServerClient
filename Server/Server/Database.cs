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
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;
                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
            }
        }

        public void Run(string sqlString) {
            MySqlCommand cmd = new MySqlCommand(sqlString, _connection);
            cmd.ExecuteNonQuery();
        }

        public bool CloseConnection() {
            try {
                _connection.Close();
                return true;
            } catch (MySqlException ex) {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}