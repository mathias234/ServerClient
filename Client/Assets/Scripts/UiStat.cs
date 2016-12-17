using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts {
    public class UiStat : MonoBehaviour {
        public GameObject StatName;
        public GameObject StatValue;

        public void SetType(Stat stat) {
            StatName.GetComponent<Text>().text = stat.Name;
            StatValue.GetComponent<Text>().text = stat.Value.ToString();
        }
    }
}