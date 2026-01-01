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
            if (!inventory) inventory = GameObject.Find("Manager intro")?.GetComponent<InventoryManager>();
            inventory.scene = scene;
            UpdatePlayer();
        }
#pragma warning restore IDE0060 // Restore unused parameter warning

        public static void UpdatePlayer() {
            if (!sm) sm = GameObject.Find("Manager intro")?.GetComponent<SaveManager>();
            if (!inventory) inventory = GameObject.Find("Manager intro")?.GetComponent<InventoryManager>();
            if ((scene.name != "Game" && scene.name != "Memory" && scene.name != "Outer Void") || !inventory || !inventory.inventoryLoaded || !sm) return;
            if (!player) player = GameObject.Find("Player")?.GetComponent<PlayerController>();
            if (!player) return;

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

            // other collectables will be handled by a scene setup script that is to be implemented
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
                            if (!shopItem3Square) shopItem3Square = GameObject.Find("World/Smile Shop/Dissappear on Save");
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
                        if (!shopItem4Square) shopItem4Square = GameObject.Find("World/Smile Shop/Dissappear on Save (1)");
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
            sm.Save();
        }

        private static SaveManager sm;
        private static InventoryManager inventory;
        private static PlayerController player;
        private static Scene scene;
        private static int lastSceneHandle = -1;
        private static GameObject shopItem3Square;
        private static GameObject shopItem4Square;
    }
}