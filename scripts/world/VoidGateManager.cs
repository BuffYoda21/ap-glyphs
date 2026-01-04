using System.Collections.Generic;
using UnityEngine;

namespace ApGlyphs {
    // to be placed on WORLD/The Chasm/(Hub) (R5E)
    public class VoidGateManager : MonoBehaviour {
        public void Start() {
            if (!inventory) inventory = SceneSearcher.Find("Manager intro")?.GetComponent<InventoryManager>();
            if (!inventory) Destroy(this);
            closedBg = transform.Find("R5E")?.gameObject;
            gate = transform.Find("Tiles/Gate")?.gameObject;
            openEffect = transform.Find("Light Ring")?.gameObject;
            portal = transform.Find("Tiles/Portal")?.gameObject;
            for (int i = 0; i < transform.childCount; i++) {
                Transform child = transform.GetChild(i);
                if (child.name.StartsWith("GateIndicator")) gateIndicators.Add(child.gameObject);
            }
            johnRoomParent = SceneSearcher.Find("WORLD/The Chasm/(R-2L) (John Room)");
            johnRoomClosedBg = johnRoomParent.Find("R-2L")?.gameObject;
            johnRoomGate = johnRoomParent.Find("Gate")?.gameObject;
            johnRoomCustsceneTrigger = johnRoomParent.Find("CutsceneTrigger")?.gameObject;
            gateIndicatorOnSprite = Resources.Load<Sprite>("sprites/depictions/gatefragment/GateFragmentON");
            for (int i = 0; i < johnRoomParent.childCount; i++) {
                Transform child = johnRoomParent.GetChild(i);
                if (child.name.StartsWith("GateIndicator")) johnRoomGateIndicators.Add(child.gameObject);
            }
            //if (gateIndicators.Count != 7 || johnRoomGateIndicators.Count != 7 || gateIndicatorOnSprite == null || portal == null || gate == null || openEffect == null || closedBg == null || johnRoomClosedBg == null || johnRoomGate == null || johnRoomCustsceneTrigger == null)
            //    Destroy(this);
        }

        public void Update() {
            if (!inventory) return;
            if (inventory.items.ContainsKey("Void Gate Shard")) {
                int shardCount = inventory.items["Void Gate Shard"];
                if (shardCount >= 1) {
                    gateIndicators[0].GetComponent<SpriteRenderer>().sprite = gateIndicatorOnSprite;
                    johnRoomGateIndicators[0].GetComponent<SpriteRenderer>().sprite = gateIndicatorOnSprite;
                }
                if (shardCount >= 2) {
                    gateIndicators[1].GetComponent<SpriteRenderer>().sprite = gateIndicatorOnSprite;
                    johnRoomGateIndicators[1].GetComponent<SpriteRenderer>().sprite = gateIndicatorOnSprite;
                }
                if (shardCount >= 3) {
                    gateIndicators[2].GetComponent<SpriteRenderer>().sprite = gateIndicatorOnSprite;
                    johnRoomGateIndicators[2].GetComponent<SpriteRenderer>().sprite = gateIndicatorOnSprite;
                }
                if (shardCount >= 4) {
                    gateIndicators[3].GetComponent<SpriteRenderer>().sprite = gateIndicatorOnSprite;
                    johnRoomGateIndicators[3].GetComponent<SpriteRenderer>().sprite = gateIndicatorOnSprite;
                }
                if (shardCount >= 5) {
                    gateIndicators[4].GetComponent<SpriteRenderer>().sprite = gateIndicatorOnSprite;
                    johnRoomGateIndicators[4].GetComponent<SpriteRenderer>().sprite = gateIndicatorOnSprite;
                }
                if (shardCount >= 6) {
                    gateIndicators[5].GetComponent<SpriteRenderer>().sprite = gateIndicatorOnSprite;
                    johnRoomGateIndicators[5].GetComponent<SpriteRenderer>().sprite = gateIndicatorOnSprite;
                }
                if (shardCount >= 7) {
                    gateIndicators[6].GetComponent<SpriteRenderer>().sprite = gateIndicatorOnSprite;
                    johnRoomGateIndicators[6].GetComponent<SpriteRenderer>().sprite = gateIndicatorOnSprite;
                    portal.SetActive(true);
                    openEffect.SetActive(true);
                    closedBg.SetActive(false);
                    gate.SetActive(false);
                    johnRoomClosedBg.SetActive(false);
                    johnRoomGate.SetActive(false);
                    johnRoomCustsceneTrigger.SetActive(true);
                    Destroy(this);
                }
            }
        }

        private InventoryManager inventory;
        private GameObject closedBg;
        private GameObject gate;
        private GameObject openEffect;
        private GameObject portal;
        private List<GameObject> gateIndicators = new List<GameObject>();
        private Transform johnRoomParent;
        private GameObject johnRoomClosedBg;
        private GameObject johnRoomGate;
        private GameObject johnRoomCustsceneTrigger;
        private List<GameObject> johnRoomGateIndicators = new List<GameObject>();
        private Sprite gateIndicatorOnSprite;
    }
}