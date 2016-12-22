using System.Collections.Generic;
using Shared;
using UnityEngine;

namespace Assets {
    public class PopulateCharacterSelection : MonoBehaviour {
        public GameObject CharacterTemplate;
        public GameObject CharacterSelection;

        public void Populate(List<Character> charactersCharacters) {
            foreach (var charactersCharacter in charactersCharacters) {
                var character = Instantiate(CharacterTemplate);
                character.transform.SetParent(CharacterSelection.transform);

                var uiCharacter = character.GetComponent<UICharacter>();
                uiCharacter.SetName(charactersCharacter.Name);
                uiCharacter.SetLevel(charactersCharacter.Level);
            }
        }
    }
}
