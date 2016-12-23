using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateClasses : MonoBehaviour {
    public GameObject ClassTemplate;

    // Use this for initialization
    void Start() {
        var enums = Enum.GetValues(typeof(Shared.CharacterClasses));

        for (int i = 0; i < enums.Length; i++) {
            // TODO: Lookup the actual name somewhere
            var enumName = enums.GetValue(i);

            var charClass = Instantiate(ClassTemplate, transform);
            charClass.transform.localScale = Vector3.one;
            charClass.transform.FindChild("ClassName").GetComponent<Text>().text = enumName.ToString();

            var classButton = charClass.GetComponent<Button>();

            var i1 = i;
            classButton.onClick.AddListener(() => {
                charClass.transform.FindChild("BasicBackground").gameObject.SetActive(false);
                charClass.transform.FindChild("SelectedBackground").gameObject.SetActive(true);

                var characterCreation = FindObjectOfType<CharacterCreation>();

                if (characterCreation.LastSelectedButton != null && characterCreation.LastSelectedButton != charClass) {
                    characterCreation.LastSelectedButton.transform.FindChild("BasicBackground").gameObject.SetActive(true);
                    characterCreation.LastSelectedButton.transform.FindChild("SelectedBackground").gameObject.SetActive(false);
                }

                characterCreation.SetSelectedClass(i1, charClass);
            });
        }
    }
}
