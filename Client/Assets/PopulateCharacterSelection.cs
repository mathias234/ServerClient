using System.Collections.Generic;
using Shared;
using UnityEngine;

namespace Assets {
    public class PopulateCharacterSelection : MonoBehaviour {
        public GameObject CharacterTemplate;
        public GameObject CharacterSelection;

        public void Populate(List<Character> characters) {
            foreach (var character in characters) {
                var characterObj = Instantiate(CharacterTemplate);
                characterObj.transform.SetParent(CharacterSelection.transform);
                characterObj.transform.localScale = Vector3.one;

                var uiCharacter = characterObj.GetComponent<UICharacter>();
                uiCharacter.SetName(character.Name);
                uiCharacter.SetLevel(character.Level);
                uiCharacter.SetClass(character.Class);
            }
        }
    }
}
