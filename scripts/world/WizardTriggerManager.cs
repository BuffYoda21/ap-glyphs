using System;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ApGlyphs {
    [HarmonyPatch]
    public static class WizardTriggerManager {
        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPostfix]
        public static void OnSceneLoaded(Scene scene) {
            if (scene.name != "Game") return;
            triggerEnabled = false;
            trigger = SceneSearcher.Find(TRIGGER_ENABLED_TRANSFORM_PATH + "/" + TRIGGER_DISABLED_TRANSFORM_RELATIVE_PATH)?.gameObject;
            try {
                wizGlyphstones = Convert.ToInt32(NetworkClient.options["WizardRequirements"]);
            } catch (Exception ex) {
                MelonLogger.Error("Failed to get WizardRequirements: " + ex.Message);
                wizGlyphstones = 3;
            }
            falsePrimaryGlyph = trigger.transform.Find("Tiles/False Primary Glyph")?.gameObject;
            UpdateTrigger();
        }

        public static void UpdateTrigger() {
            if (!falsePrimaryGlyph || !falsePrimaryGlyph.activeSelf) { UnityEngine.Object.Destroy(trigger); return; }
            if (triggerEnabled || !InventoryManager.items.TryGetValue("Glyphstone", out int glyphstones) || glyphstones < wizGlyphstones) return;
            trigger.transform.SetParent(SceneSearcher.Find(TRIGGER_ENABLED_TRANSFORM_PATH)?.transform, true);
            triggerEnabled = true;
        }

        private const string TRIGGER_ENABLED_TRANSFORM_PATH = "World/Region3/Black/(R7D)>(R9F) The False Primary Glyph";
        private const string TRIGGER_DISABLED_TRANSFORM_RELATIVE_PATH = "Cutscene Conditional 1/Cutscene Conditional 2/Cutscene Conditional 3/CutsceneTrigger";
        private static GameObject trigger;
        private static GameObject falsePrimaryGlyph;
        private static int wizGlyphstones;
        private static bool triggerEnabled;
    }
}