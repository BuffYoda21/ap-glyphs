using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ApGlyphs {
    [HarmonyPatch]
    public static class AbilityManager {
#pragma warning disable IDE0060 // Remove unused parameter warning
        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPostfix]
        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.handle == lastSceneHandle) return;
            lastSceneHandle = scene.handle;
            AbilityManager.scene = scene;
            if (scene.name != "Game" && scene.name != "Memory" && scene.name != "Outer Void") return;
            if (!inventory) inventory = SceneSearcher.Find("Manager intro")?.GetComponent<InventoryManager>();
            inventory.scene = scene;
            if (!client) client = SceneSearcher.Find("Manager intro")?.GetComponent<ClientWrapper>();
            MelonCoroutines.Start(DelayedCall());
        }
#pragma warning restore IDE0060 // Restore unused parameter warning

        // fixes the issue where the player always starts from world spawn
        private static IEnumerator DelayedCall() {
            yield return new WaitForSeconds(0.25f);
            UpdatePlayer();
        }

        public static void UpdatePlayer() {
            if (!sm) sm = SceneSearcher.Find("Manager intro")?.GetComponent<SaveManager>();
            if (!inventory) inventory = SceneSearcher.Find("Manager intro")?.GetComponent<InventoryManager>();
            if ((scene.name != "Game" && scene.name != "Memory" && scene.name != "Outer Void") || !inventory || !inventory.inventoryLoaded || !sm) return;
            if (!player) player = SceneSearcher.Find("Player")?.GetComponent<PlayerController>();
            if (!player) return;
            if (wraithRequirement == WraithRequirement.Undefined) GetWraithRequirement();

            //PlayerPrefs.SetString("Unlocked-map", "true");    // doesn't work for some reason. ItemChanger.cs moves map to player on load now as a workaround
            player.mapDisabled = true;
            player.hasWeapon = false;
            player.attackBonus = 0f;
            player.midairJumpsMax = 0;
            player.dashAttack = false;
            player.hasGrapple = false;
            player.hasParry = false;
            player.parryCD = 2f;
            player.hasShroud = false;
            player.hasGeorge = false;
            player.fragments = 0;
            player.maxHp = 100;
            player.goldfragments = 0;

            foreach (KeyValuePair<string, int> kv in inventory.items) {
                switch (kv.Key) {
                    case "Map":
                        player.mapDisabled = false;
                        break;
                    case "Progressive Sword":
                        if (kv.Value >= 1)
                            player.hasWeapon = true;
                        if (kv.Value >= 2)
                            player.attackBonus += 4f;
                        break;
                    case "Progressive Dash Orb":
                        if (kv.Value >= 1)
                            player.midairJumpsMax = 1;
                        if (kv.Value >= 2) {
                            player.dashAttack = true;
                            player.dashAttackChargeMax = 1f;
                            if (!shopItem3Square) shopItem3Square = SceneSearcher.Find("World/Smile Shop/Dissappear on Save")?.gameObject;
                            if (shopItem3Square) shopItem3Square.SetActive(false);
                        }
                        if (kv.Value >= 3) {
                            player.dashAttackChargeMax = 0.5f;
                        }
                        break;
                    case "Grapple":
                        player.hasGrapple = true;
                        break;
                    case "Progressive Parry":
                        if (kv.Value >= 1)
                            player.hasParry = true;
                        if (!shopItem4Square) shopItem4Square = SceneSearcher.Find("World/Smile Shop/Dissappear on Save (1)")?.gameObject;
                        if (shopItem4Square) shopItem4Square.SetActive(false);
                        if (kv.Value >= 2)
                            player.parryCD = 1f;
                        break;
                    case "Shroud":
                        player.hasShroud = true;
                        break;
                    case "Progressive Essence of George":
                        if (kv.Value >= 1)
                            player.hasGeorge = true;
                        if (kv.Value >= 2)
                            player.attackBonus += 2f; //extra healing will need to be handled separately
                        break;
                    case "Silver Shard":
                        player.maxHp = 100 + kv.Value / 3 * 10;
                        player.fragments = kv.Value % 3;
                        sm.playerHPBeforeCutscene = player.maxHp;
                        break;
                    case "Gold Shard":
                        player.goldfragments = kv.Value;
                        break;
                }
            }

            if (HasMetWraithRequirement()) {
                if (sm.playerHPBeforeCutscene < 150)
                    sm.playerHPBeforeCutscene = 150;
                if (!wraithTrigger) wraithTrigger = SceneSearcher.Find("World/Region2/Sector 3/Primary Glyph Room/CutsceneLoader")?.gameObject;
                if (wraithTrigger && !wraithTrigger.activeSelf) wraithTrigger.SetActive(true);
            } else {
                if (sm.playerHPBeforeCutscene >= 150)
                    sm.playerHPBeforeCutscene = 149;
                if (!wraithTrigger) wraithTrigger = SceneSearcher.Find("World/Region2/Sector 3/Primary Glyph Room/CutsceneLoader")?.gameObject;
                if (wraithTrigger && wraithTrigger.activeSelf) wraithTrigger.SetActive(false);
            }

            sm.Save();
        }

        private static void GetWraithRequirement() {
            wraithRequirement = (WraithRequirement)Convert.ToInt32(client.client.options["WraithRequirements"]);
            switch (wraithRequirement) {
                case WraithRequirement.Vanilla: wraithRequirement = WraithRequirement.SilverShard; wraithRequirementCount = 15; break;
                case WraithRequirement.SilverShard: wraithRequirementCount = Convert.ToInt32(client.client.options["WraithSilverCount"]); break;
                case WraithRequirement.GoldShard: wraithRequirementCount = Convert.ToInt32(client.client.options["WraithGoldCount"]); break;
                case WraithRequirement.SmileToken: wraithRequirementCount = Convert.ToInt32(client.client.options["WraithSmileCount"]); break;
                case WraithRequirement.RuneCube: wraithRequirementCount = Convert.ToInt32(client.client.options["WraithRuneCount"]); break;
                case WraithRequirement.GlyphStone: wraithRequirementCount = Convert.ToInt32(client.client.options["WraithGlyphstoneCount"]); break;
            }
        }

        private static bool HasMetWraithRequirement() {
            int itemCount = 0;
            switch (wraithRequirement) {
                case WraithRequirement.Undefined: return false;
                case WraithRequirement.None: return true;
                case WraithRequirement.Intended: return inventory.items.TryGetValue("Silver Shard", out itemCount) && itemCount >= 15 && inventory.items.TryGetValue("Glyphstone", out itemCount) && itemCount >= 3;
                case WraithRequirement.SilverShard: return inventory.items.TryGetValue("Silver Shard", out itemCount) && itemCount >= wraithRequirementCount;
                case WraithRequirement.GoldShard: return inventory.items.TryGetValue("Gold Shard", out itemCount) && itemCount >= wraithRequirementCount;
                case WraithRequirement.SmileToken: return inventory.items.TryGetValue("Smile Token", out itemCount) && itemCount >= wraithRequirementCount;
                case WraithRequirement.RuneCube: return inventory.items.TryGetValue("Rune Cube", out itemCount) && itemCount >= wraithRequirementCount;
                case WraithRequirement.GlyphStone: return inventory.items.TryGetValue("Glyphstone", out itemCount) && itemCount >= wraithRequirementCount;
                default: return false;
            }
        }

        private static SaveManager sm;
        private static InventoryManager inventory;
        private static ClientWrapper client;
        private static PlayerController player;
        private static Scene scene;
        private static int lastSceneHandle = -1;
        private static GameObject shopItem3Square;
        private static GameObject shopItem4Square;
        private static WraithRequirement wraithRequirement = WraithRequirement.Undefined;
        private static int wraithRequirementCount = 0;
        private static GameObject wraithTrigger;
        private enum WraithRequirement : int {
            Undefined = -1,
            None = 0,
            Vanilla = 1,
            Intended = 2,
            SilverShard = 3,
            GoldShard = 4,
            SmileToken = 5,
            RuneCube = 6,
            GlyphStone = 7,
        }
    }
}