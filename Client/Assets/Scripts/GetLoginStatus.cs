using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetLoginStatus : MonoBehaviour {
    private NetworkManager _networkManager;
    private Text _text;

    // Use this for initialization
	void Start () {
	    _networkManager = GameObject.FindObjectOfType<NetworkManager>();
	    _text = gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	    _text.text = _networkManager.CurrentLoginText;
	}
}
