using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    public class Account {
        public int AccountId;
        public Socket Socket;
        public string Username;
        public string Password;

        public Account() {
            
        }

        public Account(int accountId, string username, string password, Socket socket) {
            AccountId = accountId;
            Username = username;
            Password = password;
            Socket = socket;
        }
    }
}
