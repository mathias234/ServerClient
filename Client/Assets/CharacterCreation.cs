using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreation : MonoBehaviour {
    public GameObject LastSelectedButton;
    public int ClassId;
    public void SetSelectedClass(int classId, GameObject charClass) {
        LastSelectedButton = charClass;
        ClassId = classId;
    }
}
