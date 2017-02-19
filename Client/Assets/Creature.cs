using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {
    public int instanceId;

    public Vector3 destination;

    public void Start() {
        destination = transform.position;
    }

    public void MoveTo(float x, float y, float z) {
        destination = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z);
    }

    public void Update() {
        transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime / 5);
    }
}
