using Il2Cpp;
using UnityEngine;

namespace ApGlyphs {
    // to be placed on World/Smile Shop/Smilemask Room/Smile Mask
    public class ApSmileMask : MonoBehaviour {
        public void Start() {
            inventory = SceneSearcher.Find("Manager intro")?.GetComponent<InventoryManager>();
            smileMask = SceneSearcher.Find("World/Smile Shop/smilemask")?.gameObject;
            cutsceneTrigger = SceneSearcher.Find("World/Smile Shop/CutsceneTrigger")?.gameObject;
            startPosition = transform.position;
        }

        public void Update() {
            if (!inventory) return;
            if (startPosition != transform.position && inventory.items.ContainsKey("Smile Token") && inventory.items["Smile Token"] >= 10) {
                smileMask.SetActive(false);
                cutsceneTrigger.SetActive(true);
            }
        }

        private InventoryManager inventory;
        private GameObject smileMask;
        private GameObject cutsceneTrigger;
        private Vector3 startPosition;
    }
}