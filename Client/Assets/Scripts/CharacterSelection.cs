using System.Collections.Generic;
using Shared;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Shared.Packets;

namespace Assets {
    public class CharacterSelection : MonoBehaviour {
        public GameObject CharacterTemplate;
        public GameObject CharacterSelectionUi;
        public GameObject CreateNewButton;

        public int MaxCharacters;
        private int _characters;

        private int _selectedCharacterId;
        private GameObject _selectedCharacter;

        public void Update() {
            if (_characters >= 9) {
                CreateNewButton.SetActive(false);
            } else {
                CreateNewButton.SetActive(true);
            }
        }

        public void Populate(List<Character> characters) {
            foreach (Transform child in CharacterSelectionUi.transform) {
                Destroy(child.gameObject);
                _characters = 0;
            }

            foreach (var character in characters) {
                var characterObj = Instantiate(CharacterTemplate);
                characterObj.transform.SetParent(CharacterSelectionUi.transform);
                characterObj.transform.localScale = Vector3.one;

                var uiCharacter = characterObj.GetComponent<UICharacter>();
                uiCharacter.SetName(character.Name);
                uiCharacter.SetLevel(character.Level);
                uiCharacter.SetClass(character.Class);

                var button = uiCharacter.GetComponent<Button>();
                var tempId = character.CharacterId;

                button.onClick.AddListener(() => {
                    if (_selectedCharacter != null)
                        _selectedCharacter.GetComponent<Image>().color = new Color(0, 0.5843f, 0.6117f, 0.7411f);

                    _selectedCharacter = characterObj;
                    _selectedCharacter.GetComponent<Image>().color = new Color(0, 0.4196f, 0.4431f, 0.7411f);

                    _selectedCharacterId = character.CharacterId;
                });

                _characters++;
            }
        }

        public void Play() {
            if (_selectedCharacter == null)
                return;

            NetworkManager nm = FindObjectOfType<NetworkManager>();
            nm.SendData(new FullCharacterUpdate(nm.SocketId, _selectedCharacterId).ToByteArray());
            gameObject.SetActive(false);
        }
    }
}
