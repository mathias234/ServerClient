using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creature : MonoBehaviour {
    public int instanceId;

    public Vector3 destination;

    public float Speed = 1.0F;
    private float startTime;
    private float journeyLength;

    public Vector3 startPosition;

    public GameObject NameObj;
    public GameObject NameMount;
    public GameObject CreatureNamePrefab;

    private Canvas _canvas;

    public void Start() {
        destination = transform.position;
        startTime = Time.time;
    }

    public void MoveTo(float x, float y, float z, float speed) {
        destination = new Vector3(x, CalculateY(x, z), z);
        startTime = Time.time;
        startPosition = transform.position;
        journeyLength = Vector3.Distance(transform.position, destination);
        Speed = speed;
    }

    public float CalculateY(float x, float z) {
        RaycastHit hit = new RaycastHit();

        Physics.Raycast(new Vector3(x, 200, z), Vector3.down, out hit, 5000);

        return hit.point.y;
    }

    public void Update() {
        float distCovered = (Time.time - startTime) * Speed;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(startPosition, destination, fracJourney);

        // TODO: dont do this.
        transform.position = new Vector3(transform.position.x, CalculateY(transform.position.x, transform.position.z), transform.position.z);

        if (NameObj != null) {
            var player = FindObjectOfType<NetworkManager>().Character;
            if (Vector3.Distance(player.transform.position, transform.position) <= 20) {
                NameObj.SetActive(true);
                var rectTransform = (RectTransform)NameObj.transform;

                rectTransform.anchoredPosition = _canvas.WorldToCanvas(NameMount.transform.position);
            } else {
                NameObj.SetActive(false);
            }
        }
    }

    public void SetName(string name) {
        if (NameObj == null) {
            _canvas = FindObjectOfType<Canvas>();

            NameObj = (GameObject)Instantiate(CreatureNamePrefab);
            NameObj.transform.SetParent(_canvas.transform, false);
            NameObj.name = "Creature Name tag " + name;
        }
        NameObj.GetComponent<Text>().text = name;
    }
}
