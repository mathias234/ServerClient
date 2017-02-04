using Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerUI {
    public partial class Form1 : Form {
        private bool _serverStarted = false;
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            UpdateStartServerText();
        }

        private void StartServer_Click(object sender, EventArgs e) {
            if (_serverStarted == false) {
                Server.Server.Start();
                _serverStarted = true;
                Timer1.Enabled = true;
            } else {
                // TODO: implement server shutdown
            }

            UpdateStartServerText();
        }

        public void UpdateStartServerText() {
            StartServer.Text = _serverStarted ? "Stop Server" : "Start Server";
        }

        private void Timer1_Tick(object sender, EventArgs e) {
            // first remove old
            for (int i = 0; i < PlayersOnline.Items.Count; i++) {
                var item = (string)PlayersOnline.Items[i];
                var accounts = Server.Server.GetAllAccounts();

                var account = item.Split('(')[0];
                if (!accounts.Any(acc => acc.Username == account)) {
                    PlayersOnline.Items.Remove(item);
                }
            }

            foreach (Account account in Server.Server.GetAllAccounts()) {
                if (!PlayersOnline.Items.Contains(account.Username))
                    PlayersOnline.Items.Add(account.Username);
            }
        }

        private void Button1_Click(object sender, EventArgs e) {
            var accountName = (string)PlayersOnline.SelectedItem;
            var account = Server.Server.GetAllAccounts().First(acc => acc.Username == accountName);
        }
    }
}
