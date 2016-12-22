using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacter : MonoBehaviour {
    public Text CharacterName;
    public Text CharacterLevel;

    public void SetLevel(int characterLevel) {
        CharacterLevel.text = characterLevel.ToString();
    }

    public void SetName(string characterName) {
        CharacterName.text = characterName;
    }
}
