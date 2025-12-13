using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ApGlyphs {
    [HarmonyPatch]
    public class ItemChanger {
#pragma warning disable IDE0060 // Remove unused parameter warning
        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPostfix]
        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.handle == lastSceneHandle) return;
            lastSceneHandle = scene.handle;
            if (scene.name != "Game") return;

            GameObject sword = GameObject.Find("World/Region1/(R3D)(sword)/Sword");
            GameObject ap_obj_test = new GameObject("AP Test Object");
            ap_obj_test.transform.position = sword.transform.position;
            sword.SetActive(false);
            ap_obj_test.AddComponent<ArchipelagoItem>().locId = 2;
        }
#pragma warning restore IDE0060 // Restore unused parameter warning

        private static int lastSceneHandle = -1;
        public List<ArchipelagoItem> items;
    }
}