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
                GameObject missedSwordTrigger = GameObject.Find("World/Region1/(R3D)(sword)/SaveConditional");
                UnityEngine.Object.Destroy(missedSwordTrigger);
            } catch (Exception ex) {
                MelonLogger.Error("Failed to destroy missedSwordTrigger: " + ex.Message);
            }

            try {
                GameObject chickenRoomParent = GameObject.Find("World/Region2/(R5-b)");
                chickenRoomParent.transform.Find("Tiles/Square (23)").gameObject.SetActive(false);
                chickenRoomParent.transform.Find("Tiles/Square (24)").gameObject.SetActive(false);
                chickenRoomParent.transform.Find("R5-B").gameObject.SetActive(false);
                chickenRoomParent.transform.Find("door").gameObject.SetActive(true);
            } catch (Exception ex) {
                MelonLogger.Error("Failed to open between passage " + ex.Message);
            }

            try {
                GameObject secretWallTileParent = GameObject.Find("World/Region2/Sector 1/(R4-D)/Tiles");
                UnityEngine.Object.Destroy(secretWallTileParent.transform.Find("dissapear on save falseending")?.gameObject);
                UnityEngine.Object.Destroy(secretWallTileParent.transform.Find("dissapear on save smile1")?.gameObject);
                UnityEngine.Object.Destroy(secretWallTileParent.transform.Find("dissapear on save smile2")?.gameObject);
                UnityEngine.Object.Destroy(secretWallTileParent.transform.Find("dissapear on save smile3")?.gameObject);
            } catch (Exception ex) {
                MelonLogger.Error("Failed to destroy Cameo Room Entrance Tiles: " + ex.Message);
            }

            try {
                UnityEngine.Object.Destroy(GameObject.Find("World/Region3/Red/(R9J) (hidden)/Tiles/Fragment Door"));
            } catch (Exception ex) {
                MelonLogger.Error("Failed to destroy Master Puzzle 3 Door: " + ex.Message);
            }

            try {
                GameObject.Find("World/Region3/Black/(R7D)>(R9F) The False Primary Glyph").AddComponent<WizardTriggerManager>();
            } catch (Exception ex) {
                MelonLogger.Error("Failed to add WizardTriggerManager: " + ex.Message);
            }

            try {
                GameObject.Find("World/Smile Shop").AddComponent<ShopCounter>();
            } catch (Exception ex) {
                MelonLogger.Error("Failed to add ShopCounter: " + ex.Message);
            }

            try {
                GameObject.Find("World/Smile Shop/Hat room/Pedestals").AddComponent<HatRoomManager>();
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