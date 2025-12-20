using System;
using MelonLoader;
using UnityEngine;

namespace ApGlyphs {
    // to be put on "(R7D)>(R9F) The False Primary Glyph" GameObject
    public class WizardTriggerManager : MonoBehaviour {
        public void Start() {
            if (!inventory) inventory = GameObject.Find("Manager intro")?.GetComponent<InventoryManager>();
            if (!client) client = GameObject.Find("Manager intro")?.GetComponent<ClientWrapper>();
            trigger = transform.Find("Cutscene Conditional 1/Cutscene Conditional 2/Cutscene Conditional 3/CutsceneTrigger").gameObject;
            try {
                wizGlyphstones = Convert.ToInt32(client.client.slotData["WizardRequirements"]);
            } catch (Exception ex) {
                MelonLogger.Error("Failed to get WizardRequirements: " + ex.Message);
                wizGlyphstones = 3;
            }
            falsePrimaryGlyph = transform.Find("Tiles/False Primary Glyph")?.gameObject;
            if (falsePrimaryGlyph && falsePrimaryGlyph.activeSelf) return;
            Destroy(trigger);
            Destroy(this);
        }

        public void Update() {
            if (!inventory.items.ContainsKey("Glyphstone") || inventory.items["Glyphstone"] < wizGlyphstones) return;
            trigger.transform.SetParent(transform, true);
            Destroy(this);
        }

        private GameObject trigger;
        private GameObject falsePrimaryGlyph;
        private int wizGlyphstones;
        public InventoryManager inventory;
        public ClientWrapper client;
    }
}