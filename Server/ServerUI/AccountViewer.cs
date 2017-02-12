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
    public partial class AccountEditor : Form {
        private Account _account;

        public AccountEditor(Account account) {
            InitializeComponent();
            _account = account;
        }

        private void AccountEditor_Load(object sender, EventArgs e) {
            if (_account != null) {
                AccountIDTextField.Text = _account.AccountId.ToString();
                UsernameTextField.Text = _account.Username;
                EncryptedPasswordTextField.Text = _account.Password;

                if(_account.CharacterOnline != null) {
                    CharacterIdTextField.Text = _account.CharacterOnline.CharacterId.ToString();
                    CharacterNameTextField.Text = _account.CharacterOnline.Name;
                    CharacterClassTextField.Text = _account.CharacterOnline.Class.ToString();
                    CharacterLevelTextField.Text = _account.CharacterOnline.Level.ToString();
                    MapIdTextField.Text = _account.CharacterOnline.MapId.ToString();
                }
            }
        }
    }
}
