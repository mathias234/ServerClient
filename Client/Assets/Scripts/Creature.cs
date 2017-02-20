using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {
    public int instanceId;

    public Vector3 destination;

    public float speed = 1.0F;
    private float startTime;
    private float journeyLength;

    public Vector3 startPosition;

    public void Start() {
        destination = transform.position;
        startTime = Time.time;
    }

    public void MoveTo(float x, float y, float z) {
        destination = new Vector3(x, CalculateY(x,z), z);
        startTime = Time.time;
        startPosition = transform.position;
        journeyLength = Vector3.Distance(transform.position, destination);
    }

    public float CalculateY(float x, float z) {
        RaycastHit hit = new RaycastHit();

        Physics.Raycast(new Vector3(x, 200, z), Vector3.down, out hit, 5000);

        return hit.point.y;
    }

    public void Update() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(startPosition, destination, fracJourney);
    }
}
