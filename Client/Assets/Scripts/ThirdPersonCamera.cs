using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {
    public GameObject Character;
    public Vector3 BasePositionOffset;
    public Vector3 BaseRotationOffset;
    public float ScrollSpeed = 3.5f;
    // Use this for initialization
    public void Start () {
		
	}
 
    // Update is called once per frame
	public void Update () {
        gameObject.transform.position = Character.transform.position + BasePositionOffset;
        gameObject.transform.rotation = Quaternion.Euler(BaseRotationOffset);

        var scrollWheel = Input.GetAxis("Mouse ScrollWheel");

        if (scrollWheel > 0f) {
            // scroll up
            BasePositionOffset += gameObject.transform.forward / ScrollSpeed;
        } else if (scrollWheel < 0f) {
            // scroll down
           BasePositionOffset -= gameObject.transform.forward / ScrollSpeed;
        }
    }
}
