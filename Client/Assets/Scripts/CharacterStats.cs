using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

public class CharacterStats : MonoBehaviour {
    public Dictionary<string, List<Stat>> stats;

    public GameObject UiStatTypesRoot;
    public GameObject UiStatsRoot;

    public GameObject UiStatTypesPrefab;
    public GameObject UiStatsPrefab;

    public void Start() {
        stats = new Dictionary<string, List<Stat>> {
            {"Base Stats", new List<Stat> {
                new Stat("Strength", "Strength is one of you primary stats. Your primary stats is mostly used for equiping gear, but also affects other skills", 4),
                new Stat("Stamina", "Stamina is one of you primary stats. Your primary stats is mostly used for equiping gear, but also affects other skills", 4)
            }},
            {"Weapon Stats", new List<Stat> {
                new Stat("1h Blunt", "1 Hand blunt is a skill required to wear 1 hand maces, and similar blunt weapons", 4),
                new Stat("2h Blunt", "2 Hand blunt is a skill required to wear 2 hand maces, and similar blunt weapons", 4),
                new Stat("Rifle", "Rifle is a skill required to wear rifles", 3)
            } }
        };

        foreach (var stat in stats) {
            var statType = Instantiate(UiStatTypesPrefab);

            statType.GetComponent<UIStatTypes>().SetType(stat.Key, () => {
                foreach (Transform child in UiStatsRoot.transform) {
                    Destroy(child.gameObject);
                }

                foreach (var stat1 in stat.Value) {
                    var statUi = Instantiate(UiStatsPrefab);
                    statUi.transform.SetParent(UiStatsRoot.transform, false);
                    statUi.GetComponent<UiStat>().SetType(stat1);
                }
            });

            statType.transform.SetParent(UiStatTypesRoot.transform, false);
        }

    }
}

public class Stat {
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int Value { get; private set; }
    public int ValuePerLevel { get; private set; }

    public Stat() {
        Name = "No Name";
        Description = "No Description";
        Value = 0;
        ValuePerLevel = 0;
    }

    public Stat(string name, string description, int valuePerLevel) {
        Name = name;
        Description = description;
        ValuePerLevel = valuePerLevel;
        Value = 0;
    }
}
