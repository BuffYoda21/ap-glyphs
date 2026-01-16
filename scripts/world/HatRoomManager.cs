using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace ApGlyphs {
    // attach to "Pedestals" GameObject
    [HarmonyPatch]
    public class HatRoomManager : MonoBehaviour {
        public void Start() {
            inventory = SceneSearcher.Find("Manager intro")?.GetComponent<InventoryManager>();
            hats.Add("Pink Bow", transform.Find("pinkBow")?.gameObject);
            hats.Add("Propeller Hat", transform.Find("propeller hat")?.gameObject);
            hats.Add("Traffic Cone", transform.Find("cone hat")?.gameObject);
            hats.Add("John Hat", transform.Find("john hat")?.gameObject);
            hats.Add("Top Hat", transform.Find("topHat")?.gameObject);
            hats.Add("Fez", transform.Find("fez hat")?.gameObject);
            hats.Add("Party Hat", transform.Find("party hat")?.gameObject);
            hats.Add("Bomb Hat", transform.Find("bomb hat")?.gameObject);
            hats.Add("Crown", transform.Find("crown")?.gameObject);
            hats.Add("Progressive Chicken Hat", transform.Find("chicken")?.gameObject);
            scheduledSafetyUpdate = Time.time + 0.25f;
        }

        public void Update() {
            if (Time.time >= scheduledSafetyUpdate) preformSafetyUpdate = true;
            if (!inventory) return;
            if (inventory.items.Count != lastInventoryCount || preformSafetyUpdate) {
                preformSafetyUpdate = false;
                lastInventoryCount = inventory.items.Count;
                foreach (string hat in hats.Keys) {
                    if (inventory.items.ContainsKey(hat) && inventory.items[hat] > 0) {
                        ActivateHat(hats[hat]);
                        if (hat == "Progressive Chicken Hat") {
                            if (inventory.items[hat] == 1)
                                hats["Progressive Chicken Hat"].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/default/hats/chicken/chicken");
                            else
                                hats["Progressive Chicken Hat"].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/default/hats/chicken/chicken 1");
                        }
                    }
                }
            }
        }

        private void ActivateHat(GameObject hat) {
            hat.GetComponent<SpriteRenderer>().enabled = true;
            hat.GetComponent<BoxCollider2D>().enabled = true;
        }

        [HarmonyPatch(typeof(HatSwap), "OnTriggerEnter2D")]
        [HarmonyPostfix]
        public static void OnHatSwap(Collider2D col) {
            if (col.gameObject.name != "Player") return;
            AbilityManager.UpdatePlayer();
        }

        private Dictionary<string, GameObject> hats = new Dictionary<string, GameObject>();
        private InventoryManager inventory;
        private float scheduledSafetyUpdate = 0f;
        private bool preformSafetyUpdate = false;
        private int lastInventoryCount = -1;
    }
}