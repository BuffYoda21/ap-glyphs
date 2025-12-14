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
            UpdatePlayer();
        }
#pragma warning restore IDE0060 // Restore unused parameter warning

        public static void UpdatePlayer() {
            if (!inventory) inventory = GameObject.Find("Manager intro")?.GetComponent<InventoryManager>();
            if ((scene.name != "Game" && scene.name != "Memory" && scene.name != "Outer Void") || !inventory || !inventory.inventoryLoaded) return;
            if (!player) player = GameObject.Find("Player")?.GetComponent<PlayerController>();
            if (!player) return;

            player.hasWeapon = false;
            player.attackBonus = 0f;
            player.midairJumpsMax = 0;
            player.dashAttack = false;
            player.hasGrapple = false;
            player.hasParry = false;
            player.parryCD = 2f;

            // other collectables will be handled by a scene setup script that is to be implemented
            foreach (KeyValuePair<string, int> kv in inventory.items) {
                switch (kv.Key) {
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
                        if (kv.Value >= 3) {
                            player.maxHp = 110;
                            player.hp = 110;
                        }
                        if (kv.Value >= 6) {
                            player.maxHp = 120;
                            player.hp = 120;
                        }
                        if (kv.Value >= 9) {
                            player.maxHp = 130;
                            player.hp = 130;
                        }
                        if (kv.Value >= 12) {
                            player.maxHp = 140;
                            player.hp = 140;
                        }
                        if (kv.Value >= 15) {
                            player.maxHp = 150;
                            player.hp = 150;
                        }
                        break;
                    case "Gold Shard":
                        if (kv.Value == 1)
                            player.goldfragments = 1;
                        else if (kv.Value == 2)
                            player.goldfragments = 2;
                        else if (kv.Value >= 3)
                            player.goldfragments = 3;
                        break;
                }
            }
        }

        private static InventoryManager inventory;
        private static PlayerController player;
        private static Scene scene;
        private static int lastSceneHandle = -1;
    }
}