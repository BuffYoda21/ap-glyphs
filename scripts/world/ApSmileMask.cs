using UnityEngine;

namespace ApGlyphs {
    // to be placed on World/Smile Shop/Smilemask Room/Smile Mask
    public class ApSmileMask : MonoBehaviour {
        public void Start() {
            smileMask = SceneSearcher.Find("World/Smile Shop/smilemask")?.gameObject;
            cutsceneTrigger = SceneSearcher.Find("World/Smile Shop/CutsceneTrigger")?.gameObject;
            startPosition = transform.position;
        }

        public void Update() {
            if (startPosition != transform.position && InventoryManager.items.ContainsKey("Smile Token") && InventoryManager.items["Smile Token"] >= 10) {
                smileMask.SetActive(false);
                cutsceneTrigger.SetActive(true);
            }
        }

        private GameObject smileMask;
        private GameObject cutsceneTrigger;
        private Vector3 startPosition;
    }
}