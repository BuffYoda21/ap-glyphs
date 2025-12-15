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

            if (scene.name == "Game")
                PlaceItemsGame();
            else if (scene.name == "Memory")
                PlaceItemsMemory();
            else if (scene.name == "Outer Void")
                PlaceItemsOuterVoid();
        }
#pragma warning restore IDE0060 // Restore unused parameter warning

        private static void PlaceItemsGame() {
            Transform APItemParent = new GameObject("AP_Items").transform;

            GameObject loc1 = new GameObject("1_Starting Item");
            loc1.transform.SetParent(APItemParent);
            loc1.transform.position = new Vector3(0f, 0f, 0f);
            loc1.AddComponent<ArchipelagoItem>().locId = 1;

            GameObject sword = GameObject.Find("World/Region1/(R3D)(sword)/Sword");
            if (sword) {
                GameObject loc2 = new GameObject("2_Sword Pedestal");
                loc2.transform.SetParent(APItemParent);
                loc2.transform.position = sword.transform.position;
                sword.SetActive(false);
                loc2.AddComponent<ArchipelagoItem>().locId = 2;
            }

            GameObject dashOrb = GameObject.Find("World/Region1/Runic Construct(R3E)/Dash Orb");
            if (dashOrb) {
                GameObject loc3 = new GameObject("3_Runic Construct Reward");
                loc3.transform.SetParent(APItemParent);
                loc3.transform.position = dashOrb.transform.position;
                dashOrb.SetActive(false);
                loc3.AddComponent<ArchipelagoItem>().locId = 3;
            }

            GameObject map = GameObject.Find("World/Region1/(R2B)(Map)/Map");
            if (map) {
                GameObject loc4 = new GameObject("4_Map Pedestal");
                loc4.transform.SetParent(APItemParent);
                loc4.transform.position = map.transform.position;
                map.SetActive(false);
                loc4.AddComponent<ArchipelagoItem>().locId = 4;
            }

            GameObject shard1 = GameObject.Find("World/Region1/(R0B) (Fragment1)/Fragment 1");
            if (shard1) {
                GameObject loc5 = new GameObject("5_Silver Shard Puzzle 1");
                loc5.transform.SetParent(APItemParent);
                loc5.transform.position = shard1.transform.position;
                shard1.SetActive(false);
                loc5.AddComponent<ArchipelagoItem>().locId = 5;
            }

            GameObject shard2 = GameObject.Find("World/Region1/(R6-A) (Fragment2)/Fragment 2");
            if (shard2) {
                GameObject loc6 = new GameObject("6_Silver Shard Puzzle 2");
                loc6.transform.SetParent(APItemParent);
                loc6.transform.position = shard2.transform.position;
                shard2.SetActive(false);
                loc6.AddComponent<ArchipelagoItem>().locId = 6;
            }

            GameObject shard3 = GameObject.Find("World/Region1/(R8B) (Fragment3)/Fragment 3");
            if (shard3) {
                GameObject loc7 = new GameObject("7_Silver Shard Puzzle 3");
                loc7.transform.SetParent(APItemParent);
                loc7.transform.position = shard3.transform.position;
                shard3.SetActive(false);
                loc7.AddComponent<ArchipelagoItem>().locId = 7;
            }

            GameObject token1 = GameObject.Find("World/Region1/(R4C) (MegaPuzzle2)/SMILE I");
            if (token1) {
                GameObject loc8 = new GameObject("8_Smile Token Puzzle 1");
                loc8.transform.SetParent(APItemParent);
                loc8.transform.position = token1.transform.position;
                token1.SetActive(false);
                loc8.AddComponent<ArchipelagoItem>().locId = 8;
            }

            GameObject token9 = GameObject.Find("World/Region1/(SMILE IX)/SMILE I");    // dev probably forgot to change this to IX
            if (token9) {
                GameObject loc9 = new GameObject("9_Smile Token Puzzle 9");
                loc9.transform.SetParent(APItemParent);
                loc9.transform.position = token9.transform.position;
                token9.SetActive(false);
                loc9.AddComponent<ArchipelagoItem>().locId = 9;
            }

            GameObject seed1 = GameObject.Find("World/Region2/(R5-b)/Seeds/Seed");
            if (seed1) {
                GameObject loc10 = new GameObject("10_Color Cypher Room Pickup");
                loc10.transform.SetParent(APItemParent);
                loc10.transform.position = seed1.transform.position;
                seed1.SetActive(false);
                loc10.AddComponent<ArchipelagoItem>().locId = 10;
            }

            GameObject cube2 = GameObject.Find("World/Region1/(R4C) (MegaPuzzle2)/Cube II");
            if (cube2) {
                GameObject loc11 = new GameObject("11_Master Puzzle 2");
                loc11.transform.SetParent(APItemParent);
                loc11.transform.position = cube2.transform.position;
                cube2.SetActive(false);
                loc11.AddComponent<ArchipelagoItem>().locId = 11;
                loc11.SetActive(false); // this needs a seperate script to appear on puzzle completion
            }
        }

        private static void PlaceItemsMemory() {
            return; // memory doesn't actually have items
        }

        private static void PlaceItemsOuterVoid() {

        }

        private static int lastSceneHandle = -1;
        public List<ArchipelagoItem> items;
    }
}