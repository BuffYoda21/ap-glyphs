using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
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
                GameObject.Destroy(missedSwordTrigger);
            } catch (Exception ex) {
                MelonLogger.Error("Failed to destroy missedSwordTrigger: " + ex.Message);
            }

            try {
                GameObject.Find("World/Region3/Black/(R7D)>(R9F) The False Primary Glyph").AddComponent<WizardTriggerManager>();
            } catch (Exception ex) {
                MelonLogger.Error("Failed to add WizardTriggerManager: " + ex.Message);
            }
        }

        private static void EditWorldMemory() {

        }

        private static void EditWorldOuterVoid() {

        }

        private static int lastSceneHandle = -1;
    }
}