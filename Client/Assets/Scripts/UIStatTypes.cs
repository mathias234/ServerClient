using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIStatTypes : MonoBehaviour {
    public GameObject Text;

    public void SetType(string text, UnityAction action) {
        Text.GetComponent<Text>().text = text;

        GetComponent<Button>().onClick.AddListener(action);
    }
}
