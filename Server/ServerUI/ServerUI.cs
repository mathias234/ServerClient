using Server;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ServerUI {
    public partial class Form1 : Form {
        private MainServer _mainServer;
        private bool _serverStarted = false;

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            UpdateStartServerText();
            //ToggleServer();
        }

        private void StartServer_Click(object sender, EventArgs e) {
            ToggleServer();
        }

        private void ToggleServer() {
            if (_serverStarted == false) {
                Log.NewLogMessage += Log_NewLogMessage;
                _mainServer = new MainServer();
                _mainServer.Start();
                _serverStarted = true;
                Timer1.Enabled = true;
            } else {
                _mainServer.Stop();
                _mainServer = null;
                _serverStarted = false;
                Timer1.Enabled = false;
                OutputField.Clear();
                Log.NewLogMessage -= Log_NewLogMessage;
                GC.Collect();
            }


            UpdateStartServerText();
        }

        private void Log_NewLogMessage(LogType type, string message) {
            OutputField.Invoke(new MethodInvoker(delegate () {
                switch (type) {
                    case LogType.Debug:
                        OutputField.AppendText(message, Color.DimGray, true);
                        break;
                    case LogType.Warning:
                        OutputField.AppendText(message, Color.Yellow, true);
                        break;
                    case LogType.Error:
                        OutputField.AppendText(message, Color.Red, true);
                        break;
                    default:
                        break;
                }
            }));
        }

        public void UpdateStartServerText() {
            StartServer.Text = _serverStarted ? "Stop Server" : "Start Server";
        }

        private void Timer1_Tick(object sender, EventArgs e) {
            // first remove old
            for (int i = 0; i < PlayersOnline.Items.Count; i++) {
                var item = (string)PlayersOnline.Items[i];
                var accounts = MainServer.GetAllAccounts();

                var account = item.Split('(')[0];
                if (!accounts.Any(acc => acc.Username == account)) {
                    PlayersOnline.Items.Remove(item);
                }
            }

            foreach (Account account in MainServer.GetAllAccounts()) {
                if (!PlayersOnline.Items.Contains(account.Username))
                    PlayersOnline.Items.Add(account.Username);
            }
        }

        private void Button1_Click(object sender, EventArgs e) {
            var accountName = (string)PlayersOnline.SelectedItem;
            var account = MainServer.GetAllAccounts().First(acc => acc.Username == accountName);

            if (account == null)
                return;

            AccountEditor ae = new AccountEditor(account);
            ae.Show();
        }

        private void SaveTimer_Tick(object sender, EventArgs e) {
            if (_mainServer != null)
                _mainServer.Save();
        }
    }
}
