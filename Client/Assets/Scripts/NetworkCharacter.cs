using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacter : MonoBehaviour {
    public int socketId;
    public bool isLocal;
	// Update is called once per frame
	void Update () {
	    if (isLocal == true) {
	        // send movement

	    }
	}
}
