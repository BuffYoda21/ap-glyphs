using System;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ApGlyphs {
    [HarmonyPatch]
    public class WorldChanger {
#pragma warning disable IDE0060 // Remove unused parameter warning
        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPostfix]
        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.handle == lastSceneHandle) return;
            lastSceneHandle = scene.handle;

            if (scene.name == "Game")
                EditWorldGame();
            else if (scene.name == "Memory")
                EditWorldMemory();
            else if (scene.name == "Outer Void")
                EditWorldOuterVoid();
        }
#pragma warning restore IDE0060 // Restore unused parameter warning

        private static void EditWorldGame() {
            try {
                GameObject missedSwordTrigger = SceneSearcher.Find("World/Region1/(R3D)(sword)/SaveConditional")?.gameObject;
                UnityEngine.Object.Destroy(missedSwordTrigger);
            } catch (Exception ex) {
                MelonLogger.Error("Failed to destroy missedSwordTrigger: " + ex.Message);
            }

            try {
                GameObject chickenRoomParent = SceneSearcher.Find("World/Region2/(R5-b)")?.gameObject;
                chickenRoomParent.transform.Find("Tiles/Square (23)")?.gameObject.SetActive(false);
                chickenRoomParent.transform.Find("Tiles/Square (24)")?.gameObject.SetActive(false);
                chickenRoomParent.transform.Find("R5-B")?.gameObject.SetActive(false);
                chickenRoomParent.transform.Find("door")?.gameObject.SetActive(true);
            } catch (Exception ex) {
                MelonLogger.Error("Failed to open between passage " + ex.Message);
            }

            try {
                GameObject secretWallTileParent = SceneSearcher.Find("World/Region2/Sector 1/(R4-D)/Tiles")?.gameObject;
                UnityEngine.Object.Destroy(secretWallTileParent.transform.Find("dissapear on save falseending")?.gameObject);
                UnityEngine.Object.Destroy(secretWallTileParent.transform.Find("dissapear on save smile1")?.gameObject);
                UnityEngine.Object.Destroy(secretWallTileParent.transform.Find("dissapear on save smile2")?.gameObject);
                UnityEngine.Object.Destroy(secretWallTileParent.transform.Find("dissapear on save smile3")?.gameObject);
            } catch (Exception ex) {
                MelonLogger.Error("Failed to destroy Cameo Room Entrance Tiles: " + ex.Message);
            }

            try {
                UnityEngine.Object.Destroy(SceneSearcher.Find("World/Region3/Red/(R9J) (hidden)/Tiles/Fragment Door")?.gameObject);
            } catch (Exception ex) {
                MelonLogger.Error("Failed to destroy Master Puzzle 3 Door: " + ex.Message);
            }

            try {
                SceneSearcher.Find("World/Region3/Black/(R7D)>(R9F) The False Primary Glyph")?.gameObject.AddComponent<WizardTriggerManager>();
            } catch (Exception ex) {
                MelonLogger.Error("Failed to add WizardTriggerManager: " + ex.Message);
            }

            try {
                SceneSearcher.Find("World/Smile Shop")?.gameObject.AddComponent<ShopCounter>();
            } catch (Exception ex) {
                MelonLogger.Error("Failed to add ShopCounter: " + ex.Message);
            }

            try {
                SceneSearcher.Find("World/Smile Shop/Hat room/Pedestals")?.gameObject.AddComponent<HatRoomManager>();
            } catch (Exception ex) {
                MelonLogger.Error("Failed to add HatRoomManager: " + ex.Message);
            }
        }

        private static void EditWorldMemory() {

        }

        private static void EditWorldOuterVoid() {

        }

        private static int lastSceneHandle = -1;
    }
}