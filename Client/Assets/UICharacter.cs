using System.Collections;
using System.Collections.Generic;
using Shared;
using UnityEngine;
using UnityEngine.UI;

public class UICharacter : MonoBehaviour {
    public Text CharacterName;
    public Text CharacterLevel;
    public Text CharacterClass;

    public void SetLevel(int characterLevel) {
        CharacterLevel.text = characterLevel.ToString();
    }

    public void SetName(string characterName) {
        CharacterName.text = characterName;
    }

    public void SetClass(CharacterClasses charClass) {
        CharacterClass.text = charClass.ToString();
    }
}
