using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ApGlyphs {
    public static class VoidGateManager {
        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPostfix]
        public static void OnSceneLoaded(Scene scene) {
            gateObjectsToDisable.Clear();
            gateObjectsToEnable.Clear();
            voidGate = null;
            johnRoom = null;
            if (scene.name != "Outer Void") return;

            voidGate = SceneSearcher.Find(VOID_GATE_TRANSFORM_PATH);
            johnRoom = SceneSearcher.Find(JOHN_ROOM_TRANSFORM_PATH);
            gateObjectsToDisable = new List<GameObject>() {
                voidGate.Find("R5E")?.gameObject,
                voidGate.Find("Tiles/Gate")?.gameObject,
                johnRoom.Find("R-2L")?.gameObject,
                johnRoom.Find("Gate")?.gameObject,
            };
            gateObjectsToEnable = new List<GameObject>() {
                voidGate.Find("Light Ring")?.gameObject,
                voidGate.Find("Tiles/Portal")?.gameObject,
                johnRoom.Find("CutsceneTrigger")?.gameObject,
            };
            for (int i = 0; i < voidGate.childCount; i++) {
                Transform child = voidGate.GetChild(i);
                if (child.name.StartsWith("GateIndicator")) gateIndicators.Add(child.gameObject);
            }
            if (!gateIndicatorOnSprite)
                gateIndicatorOnSprite = Resources.Load<Sprite>("sprites/depictions/gatefragment/GateFragmentON");
            for (int i = 0; i < johnRoom.childCount; i++) {
                Transform child = johnRoom.GetChild(i);
                if (child.name.StartsWith("GateIndicator")) johnRoomGateIndicators.Add(child.gameObject);
            }
            UpdateGate();
        }

        public static void UpdateGate() {
            if (InventoryManager.items.TryGetValue("Void Gate Shard", out int shardCount)) {
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
                    foreach (GameObject gateObject in gateObjectsToEnable) {
                        gateObject.SetActive(true);
                    }
                    foreach (GameObject gateObject in gateObjectsToDisable) {
                        gateObject.SetActive(false);
                    }
                }
            }
        }

        private const string VOID_GATE_TRANSFORM_PATH = "WORLD/The Chasm/(Hub) (R5E)";
        private const string JOHN_ROOM_TRANSFORM_PATH = "WORLD/The Chasm/(R-2L) (John Room)";
        private static Transform voidGate;
        private static Transform johnRoom;
        private static List<GameObject> gateObjectsToDisable = new List<GameObject>();
        private static List<GameObject> gateObjectsToEnable = new List<GameObject>();
        private static List<GameObject> gateIndicators = new List<GameObject>();
        private static List<GameObject> johnRoomGateIndicators = new List<GameObject>();
        private static Sprite gateIndicatorOnSprite;
    }
}