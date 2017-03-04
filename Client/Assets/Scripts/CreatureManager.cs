using Shared.Creature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CreatureManager : MonoBehaviour {
    public static CreatureManager Instance;
    public List<CreatureTemplate> Templates = new List<CreatureTemplate>();

    public GameObject CreaturePrefab;

    public void Awake() {
        Instance = this;
    }

    public void SpawnCreature(InstancedCreature instancedCreature) {
        var creatureObj = Instantiate(CreaturePrefab);
        var creatureTemplate = Templates.First(t => t.TemplateId == instancedCreature.TemplateId);


        creatureObj.transform.position = new Vector3(instancedCreature.X, instancedCreature.Y, instancedCreature.Z);
        creatureObj.GetComponent<Creature>().instanceId = instancedCreature.InstanceId;
        creatureObj.GetComponent<Creature>().SetName(creatureTemplate.Name);
    }
}
