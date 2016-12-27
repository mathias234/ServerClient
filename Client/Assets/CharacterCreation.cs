using System;
using System.Collections;
using System.Collections.Generic;
using Shared;
using Shared.Packets;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreation : MonoBehaviour
{
    public GameObject LastSelectedButton;
    public GameObject ClassTemplate;
    public GameObject Classes;
    public InputField CharacterName;
    public int ClassId;

    public void SetSelectedClass(int classId, GameObject charClass)
    {
        LastSelectedButton = charClass;

        ClassId = classId;
    }

    // Use this for initialization
    void Start() {
        PopulateClasses();
    }

    public void PopulateClasses()
    {
        foreach (var child in Classes.transform) {
            Destroy((GameObject)child);
        }

        var enums = Enum.GetValues(typeof(Shared.CharacterClasses));

        for (int i = 0; i < enums.Length; i++)
        {
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

                var characterCreation = FindObjectOfType<CharacterCreation>();

                if (characterCreation.LastSelectedButton != null && characterCreation.LastSelectedButton != charClass)
                {
                    characterCreation.LastSelectedButton.transform.FindChild("BasicBackground").gameObject.SetActive(true);
                    characterCreation.LastSelectedButton.transform.FindChild("SelectedBackground").gameObject.SetActive(false);
                }

                characterCreation.SetSelectedClass(i1, charClass);
            });
        }
    }

    public void Finish() {
        var nm = FindObjectOfType<NetworkManager>();
        var message = new CreateCharacter(nm.SocketId, CharacterName.text, (CharacterClasses)ClassId).ToByteArray();
        nm.SendData(message);
        nm.RequestCharacters();
    }
}
