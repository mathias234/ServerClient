using System;
using System.Collections;
using System.Collections.Generic;
using Shared;
using Shared.Packets;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreation : MonoBehaviour {
    public GameObject LastSelectedButton;
    public GameObject ClassTemplate;
    public GameObject Classes;
    public InputField CharacterName;
    public GameObject CharacterSelection;
    public int ClassId;
    private NetworkManager networkManager;

    public void SetSelectedClass(int classId, GameObject charClass) {
        LastSelectedButton = charClass;

        ClassId = classId;
    }

    // Use this for initialization
    void Start() {
        networkManager = FindObjectOfType<NetworkManager>();
        PopulateClasses();
    }

    public void PopulateClasses() {
        foreach (var child in Classes.transform) {
            Destroy((GameObject)child);
        }

        var enums = Enum.GetValues(typeof(Shared.CharacterClasses));

        for (int i = 0; i < enums.Length; i++) {
            // TODO: Lookup the actual name somewhere
            var enumName = enums.GetValue(i);

            var charClass = Instantiate(ClassTemplate, Classes.transform);
            charClass.transform.localScale = Vector3.one;
            charClass.transform.FindChild("ClassName").GetComponent<Text>().text = enumName.ToString();

            var classButton = charClass.GetComponent<Button>();

            var i1 = i;
            classButton.onClick.AddListener(() => {
                charClass.transform.FindChild("BasicBackground").gameObject.SetActive(false);
                charClass.transform.FindChild("SelectedBackground").gameObject.SetActive(true);

                if (LastSelectedButton != null && LastSelectedButton != charClass) {
                    LastSelectedButton.transform.FindChild("BasicBackground").gameObject.SetActive(true);
                    LastSelectedButton.transform.FindChild("SelectedBackground").gameObject.SetActive(false);
                }

                SetSelectedClass(i1, charClass);
            });
        }
    }

    public void Finish() {
        var message = new CreateCharacter(networkManager.SocketId, CharacterName.text, (CharacterClasses)ClassId).ToByteArray();
        networkManager.SendData(message);
    }

    public void Reset() {
        LastSelectedButton.transform.FindChild("BasicBackground").gameObject.SetActive(true);
        LastSelectedButton.transform.FindChild("SelectedBackground").gameObject.SetActive(false);
        CharacterName.text = "";
    }

    public void HandleCreationRespons(CreateCharacterRespons characterCreationRespons) {
        switch (characterCreationRespons.Respons) {
            case CreateCharacterRespons.CreateCharacterResponses.Success:
                CharacterSelection.SetActive(true);
                gameObject.SetActive(false);
                networkManager.RequestCharacters();
                Reset();
                break;
            case CreateCharacterRespons.CreateCharacterResponses.NameAlreadyUsed:
                Debug.Log("Character name is already in use, try again");
                break;
            case CreateCharacterRespons.CreateCharacterResponses.Failed:
                Debug.Log("Failed to create character reason unknown. Please restart the game and try again");
                break;
            default:
                break;  
        }
    }
}
