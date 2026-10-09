using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ApGlyphs {
    [HarmonyPatch]
    public static class ShopCounter {
        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPostfix]
        public static void OnSceneLoaded(Scene scene) {
            shopParent = null;
            counters.Clear();
            if (scene.name != "Game") return;
            shopParent = SceneSearcher.Find(SHOP_TRANSFORM_PATH);
            counters.Add(shopParent.Find("Counter")?.GetComponent<BuildText>());
            counters.Add(shopParent.Find("Refund Room!/Counter")?.GetComponent<BuildText>());
            counters.Add(shopParent.Find("Hat room/Counter")?.GetComponent<BuildText>());
            counters.Add(shopParent.Find("Smilemask Room/Counter")?.GetComponent<BuildText>());
        }

        public static void UpdateCounters() {
            if (GamestateManager.spentTokens == -1) return;
            unspentTokens = InventoryManager.items.TryGetValue("Smile Token", out int tokens) ? tokens : 0;
            foreach (BuildText counter in counters) {
                if (!counter) continue;
                if (counter.text == "" + unspentTokens || counter.text == "0" + unspentTokens) continue;
                for (int i = counter.transform.childCount - 1; i >= 0; i--) {
                    Object.Destroy(counter.transform.GetChild(i)?.gameObject);
                }
                counter.text = "" + unspentTokens;
                if (counter.text.Length == 1) counter.text = "0" + counter.text;
                counter.x = 0f;
                counter.placed = false;
            }
        }

        private const string SHOP_TRANSFORM_PATH = "World/Smile Shop";
        private static Transform shopParent;
        private static List<BuildText> counters = new List<BuildText>();
        private static int unspentTokens;
    }
}