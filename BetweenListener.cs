using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ApGlyphs {
    public class BetweenListener : MonoBehaviour {
        public void Start() {
            if (!bbm) bbm = GameObject.Find("Canvas/Bossbars")?.GetComponent<BossBarManager>();
        }

        public void Update() {
            if (!bbm || apItems.Count == 0 || !bbm.boss) return;
            GameObject bossRoom = bbm.boss.transform.parent.gameObject;
            GameObject targetApItem;
            switch (bossRoom.name) {
                case "Construct(Clone)":
                    if (!apItems.ContainsKey("construct")) return;
                    targetApItem = apItems["construct"];
                    if (!targetApItem) return;
                    targetApItem.transform.parent = bossRoom.transform;
                    targetApItem.transform.localPosition = new Vector3(0f, -1f, 0f);
                    targetApItem.SetActive(false);
                    bbm.boss.gameObject.AddComponent<ReplaceOnDestroy>().replacement = targetApItem;
                    apItems.Remove("construct");
                    break;
                case "2 Serpent(Clone)":
                    if (!apItems.ContainsKey("serpent")) return;
                    targetApItem = apItems["serpent"];
                    if (!targetApItem) return;
                    targetApItem.transform.parent = bossRoom.transform;
                    targetApItem.transform.localPosition = new Vector3(15f, -1f, 0f);
                    targetApItem.SetActive(false);
                    bbm.boss.gameObject.AddComponent<ReplaceOnDestroy>().replacement = targetApItem;
                    apItems.Remove("serpent");
                    break;
                case "BossGroup":
                    if (!apItems.ContainsKey("wizard")) return;
                    targetApItem = apItems["wizard"];
                    if (!targetApItem) return;
                    bossRoom = bossRoom.transform.parent.gameObject;
                    targetApItem.transform.parent = bossRoom.transform;
                    targetApItem.transform.localPosition = new Vector3(15f, -1f, 0f);
                    targetApItem.SetActive(false);
                    bbm.boss.gameObject.AddComponent<ReplaceOnDestroy>().replacement = targetApItem;
                    apItems.Remove("wizard");
                    break;
            }
        }

        public void PlaceFountainRoomItem(GameObject fountainRoom) {
            if (!fountainRoom || !apItems.ContainsKey("fountain") || apItems["fountain"] == null) return;
            GameObject targetApItem = apItems["fountain"];
            targetApItem.transform.parent = fountainRoom.transform;
            targetApItem.transform.localPosition = new Vector3(0f, -3f, 0f);
            targetApItem.SetActive(true);
        }


        public Dictionary<string, GameObject> apItems = new Dictionary<string, GameObject>();
        public BossBarManager bbm;
    }

    [HarmonyPatch]
    public static class BetweenFountainRoomPatch {
        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPostfix]
        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.handle == lastSceneHandle) return;
            lastSceneHandle = scene.handle;
            if (!betweenListener) betweenListener = GameObject.Find("Manager intro")?.GetComponent<BetweenListener>();
            if (betweenListener) betweenListener.Start();
        }

        [HarmonyPatch(typeof(BetweenManager), "GenerateFountainRoom")]
        [HarmonyPostfix]
        public static void GenerateFountainRoom(BetweenManager __instance) {
            if (!betweenListener) betweenListener = GameObject.Find("Manager intro")?.GetComponent<BetweenListener>();
            if (!betweenListener) return;
            betweenListener.PlaceFountainRoomItem(GameObject.Find("Fountain(Clone)"));
        }

        private static BetweenListener betweenListener;
        private static int lastSceneHandle = -1;
    }
}
