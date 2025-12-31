using System;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
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

            if (!gamestate) gamestate = GameObject.Find("Manager intro")?.GetComponent<GamestateManager>();
            if (!client) client = GameObject.Find("Manager intro")?.GetComponent<ClientWrapper>();

            if (scene.name == "Game")
                PlaceItemsGame();
            else if (scene.name == "Memory")
                PlaceItemsMemory();
            else if (scene.name == "Outer Void")
                PlaceItemsOuterVoid();
            else
                CheckForCutsceneReward(scene.name);
        }
#pragma warning restore IDE0060 // Restore unused parameter warning

        private static void PlaceItemsGame() {
            Transform APItemParent = new GameObject("AP_Items").transform;

            // Region 1
            try {
                GameObject loc1 = new GameObject("1_Starting Item");
                loc1.transform.SetParent(APItemParent);
                loc1.transform.position = new Vector3(0f, 0f, 0f);
                loc1.AddComponent<ArchipelagoItem>().locId = 1;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 1_Starting Item: " + ex.Message);
            }

            try {
                GameObject sword = GameObject.Find("World/Region1/(R3D)(sword)/Sword");
                GameObject loc2 = new GameObject("2_Sword Pedestal");
                loc2.transform.SetParent(APItemParent);
                loc2.transform.position = sword.transform.position;
                sword.SetActive(false);
                loc2.AddComponent<ArchipelagoItem>().locId = 2;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 2_Sword Pedestal: " + ex.Message);
            }

            try {
                GameObject dashOrb = GameObject.Find("World/Region1/Runic Construct(R3E)/Dash Orb");
                GameObject loc3 = new GameObject("3_Runic Construct Reward");
                loc3.transform.SetParent(APItemParent);
                loc3.transform.position = dashOrb.transform.position;
                dashOrb.SetActive(false);
                loc3.AddComponent<ArchipelagoItem>().locId = 3;
                loc3.SetActive(false);
                dashOrb.AddComponent<ReplaceOnEnable>().replacement = loc3;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 3_Runic Construct Reward: " + ex.Message);
            }

            try {
                GameObject map = GameObject.Find("World/Region1/(R2B)(Map)/Map");
                GameObject loc4 = new GameObject("4_Map Pedestal");
                loc4.transform.SetParent(APItemParent);
                loc4.transform.position = map.transform.position;
                map.transform.position = GameObject.Find("Player").transform.position; // for some reason setting playerprefs doesn't give map so just moving it to the player on load
                loc4.AddComponent<ArchipelagoItem>().locId = 4;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 4_Map Pedestal: " + ex.Message);
            }

            try {
                GameObject shard1 = GameObject.Find("World/Region1/(R0B) (Fragment1)/Fragment 1");
                GameObject loc5 = new GameObject("5_Silver Shard Puzzle 1");
                loc5.transform.SetParent(APItemParent);
                loc5.transform.position = shard1.transform.position;
                shard1.SetActive(false);
                loc5.AddComponent<ArchipelagoItem>().locId = 5;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 5_Silver Shard Puzzle 1: " + ex.Message);
            }

            try {
                GameObject shard2 = GameObject.Find("World/Region1/(R6-A) (Fragment2)/Fragment 2");
                GameObject loc6 = new GameObject("6_Silver Shard Puzzle 2");
                loc6.transform.SetParent(APItemParent);
                loc6.transform.position = shard2.transform.position;
                shard2.SetActive(false);
                loc6.AddComponent<ArchipelagoItem>().locId = 6;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 6_Silver Shard Puzzle 2: " + ex.Message);
            }

            try {
                GameObject shard3 = GameObject.Find("World/Region1/(R8B) (Fragment3)/Fragment 3");
                GameObject loc7 = new GameObject("7_Silver Shard Puzzle 3");
                loc7.transform.SetParent(APItemParent);
                loc7.transform.position = shard3.transform.position;
                shard3.SetActive(false);
                loc7.AddComponent<ArchipelagoItem>().locId = 7;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 7_Silver Shard Puzzle 3: " + ex.Message);
            }

            try {
                GameObject token1 = GameObject.Find("World/Region1/(R4C) (MegaPuzzle2)/SMILE I");
                GameObject loc8 = new GameObject("8_Smile Token Puzzle 1");
                loc8.transform.SetParent(APItemParent);
                loc8.transform.position = token1.transform.position;
                token1.SetActive(false);
                loc8.AddComponent<ArchipelagoItem>().locId = 8;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 8_Smile Token Puzzle 1: " + ex.Message);
            }

            try {
                GameObject token9 = GameObject.Find("World/Region1/(SMILE IX)/SMILE I");    // dev probably forgot to change this to IX
                GameObject loc9 = new GameObject("9_Smile Token Puzzle 9");
                loc9.transform.SetParent(APItemParent);
                loc9.transform.position = token9.transform.position;
                token9.SetActive(false);
                loc9.AddComponent<ArchipelagoItem>().locId = 9;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 9_Smile Token Puzzle 9: " + ex.Message);
            }

            try {
                GameObject seed1 = GameObject.Find("World/Region2/(R5-b)/Seeds/Seed");
                GameObject loc10 = new GameObject("10_Color Cypher Room Pickup");
                loc10.transform.SetParent(APItemParent);
                loc10.transform.position = seed1.transform.position;
                seed1.SetActive(false);
                loc10.AddComponent<ArchipelagoItem>().locId = 10;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 10_Color Cypher Room Pickup: " + ex.Message);
            }

            try {
                GameObject cube2 = GameObject.Find("World/Region1/(R4C) (MegaPuzzle2)/Cube II");
                GameObject loc11 = new GameObject("11_Master Puzzle 2");
                loc11.transform.SetParent(APItemParent);
                loc11.transform.position = cube2.transform.position;
                cube2.SetActive(false);
                loc11.AddComponent<ArchipelagoItem>().locId = 11;
                loc11.SetActive(false);
                cube2.AddComponent<ReplaceOnEnable>().replacement = loc11;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 11_Master Puzzle 2: " + ex.Message);
            }

            // Region 2
            try {
                GameObject shard4 = GameObject.Find("World/Region2/Sector 1/(R1-E) (Fragment4)/Fragment 4");
                GameObject loc12 = new GameObject("12_Silver Shard Puzzle 4");
                loc12.transform.SetParent(APItemParent);
                loc12.transform.position = shard4.transform.position;
                shard4.SetActive(false);
                loc12.AddComponent<ArchipelagoItem>().locId = 12;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 12_Silver Shard Puzzle 4: " + ex.Message);
            }

            try {
                GameObject shard5 = GameObject.Find("World/Region2/(R7-A) (Fragment 5)/Fragment 5");
                GameObject loc13 = new GameObject("13_Silver Shard Puzzle 5");
                loc13.transform.SetParent(APItemParent);
                loc13.transform.position = shard5.transform.position;
                shard5.SetActive(false);
                loc13.AddComponent<ArchipelagoItem>().locId = 13;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 13_Silver Shard Puzzle 5: " + ex.Message);
            }

            try {
                GameObject shard6 = GameObject.Find("World/Region2/(R5B) (Fragment 6)/Fragment 6");
                GameObject loc14 = new GameObject("14_Silver Shard Puzzle 6");
                loc14.transform.SetParent(APItemParent);
                loc14.transform.position = shard6.transform.position;
                shard6.SetActive(false);
                loc14.AddComponent<ArchipelagoItem>().locId = 14;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 14_Silver Shard Puzzle 6: " + ex.Message);
            }

            try {
                GameObject shard7 = GameObject.Find("World/Region2/Sector 2/(R10-D)>(R11-D) (Fragment 7)/Fragment 7");
                GameObject loc15 = new GameObject("15_Silver Shard Puzzle 7");
                loc15.transform.SetParent(APItemParent);
                loc15.transform.position = shard7.transform.position;
                shard7.SetActive(false);
                loc15.AddComponent<ArchipelagoItem>().locId = 15;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 15_Silver Shard Puzzle 7: " + ex.Message);
            }

            try {
                GameObject shard8 = GameObject.Find("World/Region2/(R7E) (Fragment8)/Fragment 8");
                GameObject loc16 = new GameObject("16_Silver Shard Puzzle 8");
                loc16.transform.SetParent(APItemParent);
                loc16.transform.position = shard8.transform.position;
                shard8.SetActive(false);
                loc16.AddComponent<ArchipelagoItem>().locId = 16;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 16_Silver Shard Puzzle 8: " + ex.Message);
            }

            try {
                GameObject shard9 = GameObject.Find("World/Region2/(R7F) (Fragment9)/Fragment  9");
                GameObject loc17 = new GameObject("17_Silver Shard Puzzle 9");
                loc17.transform.SetParent(APItemParent);
                loc17.transform.position = shard9.transform.position;
                shard9.SetActive(false);
                loc17.AddComponent<ArchipelagoItem>().locId = 17;
                loc17.SetActive(false);
                shard9.AddComponent<ReplaceOnEnable>().replacement = loc17;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 17_Silver Shard Puzzle 9: " + ex.Message);
            }

            try {
                GameObject shard15 = GameObject.Find("World/Region2/(R12B) (Fragment 15)/Fragment 15");
                GameObject loc18 = new GameObject("18_Silver Shard Puzzle 15");
                loc18.transform.SetParent(APItemParent);
                loc18.transform.position = shard15.transform.position;
                shard15.SetActive(false);
                loc18.AddComponent<ArchipelagoItem>().locId = 18;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 18_Silver Shard Puzzle 15: " + ex.Message);
            }

            try {
                GameObject token3 = GameObject.Find("World/Region2/Sector 2/(R7-F)(Secret)/SMIlE III");
                GameObject loc19 = new GameObject("19_Smile Token Puzzle 3");
                loc19.transform.SetParent(APItemParent);
                loc19.transform.position = token3.transform.position;
                token3.SetActive(false);
                loc19.AddComponent<ArchipelagoItem>().locId = 19;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 19_Smile Token Puzzle 3: " + ex.Message);
            }

            try {
                GameObject token6 = GameObject.Find("World/Region2/(R10-A)(SMILE VI)/SMIlE VI");
                GameObject loc20 = new GameObject("20_Smile Token Puzzle 6");
                loc20.transform.SetParent(APItemParent);
                loc20.transform.position = token6.transform.position;
                token6.SetActive(false);
                loc20.AddComponent<ArchipelagoItem>().locId = 20;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 20_Smile Token Puzzle 6: " + ex.Message);
            }

            try {
                GameObject token8 = GameObject.Find("World/Region2/Sector 1/(SMILE VIII)/SMIlE VIII");
                GameObject loc21 = new GameObject("21_Smile Token Puzzle 8");
                loc21.transform.SetParent(APItemParent);
                loc21.transform.position = token8.transform.position;
                token8.SetActive(false);
                loc21.AddComponent<ArchipelagoItem>().locId = 21;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 21_Smile Token Puzzle 8: " + ex.Message);
            }

            try {
                GameObject token10 = GameObject.Find("World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/SMIlE X");
                GameObject loc22 = new GameObject("22_Smile Token Puzzle 10");
                loc22.transform.SetParent(APItemParent);
                loc22.transform.position = token10.transform.position;
                token10.SetActive(false);
                loc22.AddComponent<ArchipelagoItem>().locId = 22;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 22_Smile Token Puzzle 10: " + ex.Message);
            }

            try {
                GameObject grapple = GameObject.Find("World/Region2/(R10A) (Boss2)/Grapple Worm");
                GameObject loc23 = new GameObject("23_Gilded Serpent Reward");
                loc23.transform.SetParent(APItemParent);
                loc23.transform.position = grapple.transform.position;
                grapple.SetActive(false);
                loc23.AddComponent<ArchipelagoItem>().locId = 23;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 23_Gilded Serpent Reward: " + ex.Message);
            }

            try {
                GameObject propHat = GameObject.Find("World/Region2/Sector 1/(R5-D) (Cameo Room)/propellerHat");
                GameObject loc24 = new GameObject("24_Cameo Room Pickup");
                loc24.transform.SetParent(APItemParent);
                loc24.transform.position = propHat.transform.position;
                propHat.SetActive(false);
                loc24.AddComponent<ArchipelagoItem>().locId = 24;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 24_Cameo Room Pickup: " + ex.Message);
            }

            try {
                GameObject coneHat = GameObject.Find("World/Region2/Sector 2/(R7-F)(Secret)/Car grouping/coneHat");
                GameObject loc25 = new GameObject("25_Car Hall Pickup");
                loc25.transform.SetParent(APItemParent);
                loc25.transform.position = coneHat.transform.position;
                coneHat.SetActive(false);
                loc25.AddComponent<ArchipelagoItem>().locId = 25;
                loc25.SetActive(false);
                GameObject carGrouping = coneHat.transform.parent.gameObject;
                carGrouping.AddComponent<ReplaceOnEnable>().replacement = loc25;
                carGrouping.GetComponent<ReplaceOnEnable>().destroyTarget = coneHat;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 25_Car Hall Pickup: " + ex.Message);
            }

            try {
                GameObject seed6 = GameObject.Find("World/Region2/(R5-b)/Seeds/Seed (6)");
                GameObject loc26 = new GameObject("26_Near Shooters Pickup");
                loc26.transform.SetParent(APItemParent);
                loc26.transform.position = new Vector3(seed6.transform.position.x - 2f, seed6.transform.position.y, seed6.transform.position.z);
                seed6.SetActive(false);
                loc26.AddComponent<ArchipelagoItem>().locId = 26;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 26_Near Shooters Pickup: " + ex.Message);
            }

            try {
                GameObject seed7 = GameObject.Find("World/Region2/(R5-b)/Seeds/Seed (7)");
                GameObject loc27 = new GameObject("27_Collapsed Tunnel Pickup");
                loc27.transform.SetParent(APItemParent);
                loc27.transform.position = seed7.transform.position;
                seed7.SetActive(false);
                loc27.AddComponent<ArchipelagoItem>().locId = 27;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 27_Collapsed Tunnel Pickup: " + ex.Message);
            }

            try {
                GameObject seed9 = GameObject.Find("World/Region2/(R5-b)/Seeds/Seed (9)");
                GameObject loc28 = new GameObject("28_Nest Room Pickup");
                loc28.transform.SetParent(APItemParent);
                loc28.transform.position = seed9.transform.position;
                seed9.SetActive(false);
                loc28.AddComponent<ArchipelagoItem>().locId = 28;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 28_Nest Room Pickup: " + ex.Message);
            }

            try {
                GameObject seed10 = GameObject.Find("World/Region2/(R5-b)/Seeds/Seed (10)");
                GameObject loc29 = new GameObject("29_Serpent Boss Room Pickup");
                loc29.transform.SetParent(APItemParent);
                loc29.transform.position = seed10.transform.position;
                seed10.SetActive(false);
                loc29.AddComponent<ArchipelagoItem>().locId = 29;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 29_Serpent Boss Room Pickup: " + ex.Message);
            }

            try {
                GameObject dashAttackOrb = GameObject.Find("World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/Dash Attack Orb");
                GameObject loc30 = new GameObject("30_Shadow Chase Reward");
                loc30.transform.SetParent(APItemParent);
                loc30.transform.position = dashAttackOrb.transform.position;
                dashAttackOrb.SetActive(false);
                loc30.AddComponent<ArchipelagoItem>().locId = 30;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 30_Shadow Chase Reward: " + ex.Message);
            }

            try {
                GameObject topHat = GameObject.Find("World/Region2/Sector 4/(WATER ROOM)/topHat");
                GameObject loc31 = new GameObject("31_Water Room Pickup");
                loc31.transform.SetParent(APItemParent);
                loc31.transform.position = topHat.transform.position;
                topHat.SetActive(false);
                loc31.AddComponent<ArchipelagoItem>().locId = 31;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 31_Water Room Pickup: " + ex.Message);
            }

            try {
                GameObject loc32 = new GameObject("32_George Reward 1");
                loc32.transform.SetParent(APItemParent);
                loc32.transform.position = new Vector3(395f, 15f, 0f);
                loc32.AddComponent<ArchipelagoItem>().locId = 32;
                loc32.AddComponent<SeedCounter>().hiddenPosition = new Vector3(395f, 35f, 0f);
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 32_George Reward 1: " + ex.Message);
            }

            try {
                GameObject loc33 = new GameObject("33_George Reward 2");
                loc33.transform.SetParent(APItemParent);
                loc33.transform.position = new Vector3(402f, 15f, 0f);
                loc33.AddComponent<ArchipelagoItem>().locId = 33;
                loc33.AddComponent<SeedCounter>().hiddenPosition = new Vector3(402f, 35f, 0f);
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 33_George Reward 2: " + ex.Message);
            }

            try {
                GameObject seed8 = GameObject.Find("World/Region2/(R5-b)/Seeds/Seed (8)");
                GameObject loc34 = new GameObject("34_Shadow Chase Pickup");
                loc34.transform.SetParent(APItemParent);
                loc34.transform.position = seed8.transform.position;
                seed8.SetActive(false);
                loc34.AddComponent<ArchipelagoItem>().locId = 34;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 34_Shadow Chase Pickup: " + ex.Message);
            }

            try {
                GameObject cube1 = GameObject.Find("World/Region2/Sector 2/R10-C (Map Room)/Cube");
                GameObject loc35 = new GameObject("35_Master Puzzle 1");
                loc35.transform.SetParent(APItemParent);
                loc35.transform.position = cube1.transform.position;
                cube1.SetActive(false);
                loc35.AddComponent<ArchipelagoItem>().locId = 35;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 35_Master Puzzle 1: " + ex.Message);
            }

            // Region 3
            try {
                GameObject greenStone = GameObject.Find("World/Region3/Green/(R4G)>(R3F) (GLYPHSTONE)/GlyphStone");
                GameObject loc36 = new GameObject("36_Green Stone Trial");
                loc36.transform.SetParent(APItemParent);
                loc36.transform.position = greenStone.transform.position;
                greenStone.SetActive(false);
                loc36.AddComponent<ArchipelagoItem>().locId = 36;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 36_Green Stone Trial: " + ex.Message);
            }

            try {
                GameObject blueStone = GameObject.Find("World/Region3/Blue/(R12G)>(R13F) (GLYPHSTONE)/GlyphStone");
                GameObject loc37 = new GameObject("37_Blue Stone Trial");
                loc37.transform.SetParent(APItemParent);
                loc37.transform.position = blueStone.transform.position;
                loc37.AddComponent<ArchipelagoItem>().locId = 37;
                loc37.SetActive(false);
                blueStone.AddComponent<ReplaceOnDestroy>().replacement = loc37;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 37_Blue Stone Trial: " + ex.Message);
            }

            try {
                GameObject redStone = GameObject.Find("World/Region3/Red/(R7K)>(R9K) (GLYPHSTONE)/GlyphStone");
                GameObject loc38 = new GameObject("38_Red Stone Trial");
                loc38.transform.SetParent(APItemParent);
                loc38.transform.position = redStone.transform.position;
                loc38.AddComponent<ArchipelagoItem>().locId = 38;
                loc38.SetActive(false);
                redStone.AddComponent<ReplaceOnDestroy>().replacement = loc38;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 38_Red Stone Trial: " + ex.Message);
            }

            try {
                GameObject shard10 = GameObject.Find("World/Region3/Blue/(R14A) (Fragment 10)/Fragment 10");
                GameObject loc39 = new GameObject("39_Silver Shard Puzzle 10");
                loc39.transform.SetParent(APItemParent);
                loc39.transform.position = shard10.transform.position;
                shard10.SetActive(false);
                loc39.AddComponent<ArchipelagoItem>().locId = 39;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 39_Silver Shard Puzzle 10: " + ex.Message);
            }

            try {
                GameObject shard11 = GameObject.Find("World/Region3/Green/(R2C) (Fragment 11)/Fragment 11");
                GameObject loc40 = new GameObject("40_Silver Shard Puzzle 11");
                loc40.transform.SetParent(APItemParent);
                loc40.transform.position = shard11.transform.position;
                shard11.SetActive(false);
                loc40.AddComponent<ArchipelagoItem>().locId = 40;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 40_Silver Shard Puzzle 11: " + ex.Message);
            }

            try {
                GameObject shard12 = GameObject.Find("World/Region3/Black/(R9C) (Fragment 12)/Fragment 12");
                GameObject loc41 = new GameObject("41_Silver Shard Puzzle 12");
                loc41.transform.SetParent(APItemParent);
                loc41.transform.position = shard12.transform.position;
                shard12.SetActive(false);
                loc41.AddComponent<ArchipelagoItem>().locId = 41;
                loc41.SetActive(false);
                shard12.AddComponent<ReplaceOnEnable>().replacement = loc41;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 41_Silver Shard Puzzle 12: " + ex.Message);
            }

            try {
                GameObject shard13 = GameObject.Find("World/Region3/Red/(R9G) (Fragment 13)/Fragment 13");
                GameObject loc42 = new GameObject("42_Silver Shard Puzzle 13");
                loc42.transform.SetParent(APItemParent);
                loc42.transform.position = shard13.transform.position;
                shard13.SetActive(false);
                loc42.AddComponent<ArchipelagoItem>().locId = 42;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 42_Silver Shard Puzzle 13: " + ex.Message);
            }

            try {
                GameObject shard14 = GameObject.Find("World/Region3/Blue/(R14E) (Fragment 14)/Fragment 14");
                GameObject loc43 = new GameObject("43_Silver Shard Puzzle 14");
                loc43.transform.SetParent(APItemParent);
                loc43.transform.position = shard14.transform.position;
                shard14.SetActive(false);
                loc43.AddComponent<ArchipelagoItem>().locId = 43;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 43_Silver Shard Puzzle 14: " + ex.Message);
            }

            try {
                GameObject token2 = GameObject.Find("World/Region3/Black/(R6D) (SMILE II)/SMILE II");
                GameObject loc44 = new GameObject("44_Smile Token Puzzle 2");
                loc44.transform.SetParent(APItemParent);
                loc44.transform.position = token2.transform.position;
                token2.SetActive(false);
                loc44.AddComponent<ArchipelagoItem>().locId = 44;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 44_Smile Token Puzzle 2: " + ex.Message);
            }

            try {
                GameObject token7 = GameObject.Find("World/Region3/Blue/(SMILE VII)/SMILE VII");
                GameObject loc45 = new GameObject("45_Smile Token Puzzle 7");
                loc45.transform.SetParent(token7.transform);    // used to follow token positon but at the cost of not appearing with other AP items in heiarchy tree
                loc45.transform.position = token7.transform.position;
                token7.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0f);
                UnityEngine.Object.Destroy(token7.GetComponent<Pickup>());
                loc45.AddComponent<ArchipelagoItem>().locId = 45;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 45_Smile Token Puzzle 7: " + ex.Message);
            }

            GameObject collapseController = GameObject.Find("World/Region3/Black/(R7D)>(R9F) The False Primary Glyph/Collapse Sequence Controller");
            if (!collapseController) MelonLogger.Msg("Collapse controller is null");

            try {
                GameObject loc46 = new GameObject("46_Wizard Reward");
                loc46.transform.SetParent(APItemParent);
                loc46.transform.position = new Vector3(475f, -225f, 0f);
                loc46.AddComponent<ArchipelagoItem>().locId = 46;
                loc46.SetActive(false);
                collapseController.AddComponent<ReplaceOnEnable>().replacement = loc46;
                collapseController.GetComponent<ReplaceOnEnable>().doNotDestroy = true;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 46_Wizard Reward: " + ex.Message);
            }

            try {
                GameObject seed2 = GameObject.Find("World/Region2/(R5-b)/Seeds/Seed (2)");
                GameObject loc47 = new GameObject("47_Room Below Wizard Pickup");
                loc47.transform.SetParent(APItemParent);
                loc47.transform.position = new Vector3(seed2.transform.position.x - 2f, seed2.transform.position.y + 2f, seed2.transform.position.z);
                seed2.SetActive(false);
                loc47.AddComponent<ArchipelagoItem>().locId = 47;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 47_Room Below Wizard Pickup: " + ex.Message);
            }

            try {
                GameObject cube3 = GameObject.Find("World/Region3/Red/(R11J) (CUBE III)/Cube III");
                GameObject loc48 = new GameObject("48_Master Puzzle 3");
                loc48.transform.SetParent(APItemParent);
                loc48.transform.position = cube3.transform.position;
                cube3.SetActive(false);
                loc48.AddComponent<ArchipelagoItem>().locId = 48;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 48_Master Puzzle 3: " + ex.Message);
            }

            // Region 4
            try {
                GameObject parry = GameObject.Find("World/Region2/Sector 3/(R1E) (Parry)/Parry");
                GameObject loc49 = new GameObject("49_Spearman Reward");
                loc49.transform.SetParent(APItemParent);
                loc49.transform.position = parry.transform.position;
                parry.SetActive(false);
                loc49.AddComponent<ArchipelagoItem>().locId = 49;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 49_Spearman Reward: " + ex.Message);
            }

            try {
                GameObject gold2 = GameObject.Find("World/Region2/Sector 3/(R-1H) (GOLDEN FRAGMENT II)/Gold Fragment II");
                GameObject loc50 = new GameObject("50_Multiparry Gold Shard Puzzle");
                loc50.transform.SetParent(APItemParent);
                loc50.transform.position = gold2.transform.position;
                gold2.SetActive(false);
                loc50.AddComponent<ArchipelagoItem>().locId = 50;
                loc50.SetActive(false);
                gold2.AddComponent<ReplaceOnEnable>().replacement = loc50;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 50_Multiparry Gold Shard Puzzle: " + ex.Message);
            }

            try {
                GameObject gold1 = GameObject.Find("World/Region2/Sector 3/(R-4F) (GOLD FRAGMENT I)/Gold Fragment");
                GameObject loc51 = new GameObject("51_Platforming Gold Shard Room");
                loc51.transform.SetParent(APItemParent);
                loc51.transform.position = gold1.transform.position;
                gold1.SetActive(false);
                loc51.AddComponent<ArchipelagoItem>().locId = 51;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 51_Platforming Gold Shard Room: " + ex.Message);
            }

            try {
                GameObject gold3 = GameObject.Find("World/Region2/Sector 3/(R-4H)(Flower)/Gold Fragment III");
                GameObject loc52 = new GameObject("52_Flower Puzzle Reward");
                loc52.transform.SetParent(APItemParent);
                loc52.transform.position = gold3.transform.position;
                gold3.SetActive(false);
                loc52.AddComponent<ArchipelagoItem>().locId = 52;
                loc52.SetActive(false);
                gold3.AddComponent<ReplaceOnEnable>().replacement = loc52;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 52_Flower Puzzle Reward: " + ex.Message);
            }

            try {
                GameObject token4 = GameObject.Find("World/Region2/Sector 3/(R-4C) (SMILE IV)/SMIlE IV");
                GameObject loc53 = new GameObject("53_Smile Token Puzzle 4");
                loc53.transform.SetParent(APItemParent);
                loc53.transform.position = token4.transform.position;
                token4.SetActive(false);
                loc53.AddComponent<ArchipelagoItem>().locId = 53;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 53_Smile Token Puzzle 4: " + ex.Message);
            }

            try {
                GameObject token5 = GameObject.Find("World/Region2/Sector 3/(R2D) (Smile V)/SMIlE V");
                GameObject loc54 = new GameObject("54_Smile Token Puzzle 5");
                loc54.transform.SetParent(APItemParent);
                loc54.transform.position = token5.transform.position;
                token5.SetActive(false);
                loc54.AddComponent<ArchipelagoItem>().locId = 54;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 54_Smile Token Puzzle 5: " + ex.Message);
            }

            try {
                GameObject seed4 = GameObject.Find("World/Region2/(R5-b)/Seeds/Seed (4)");
                GameObject loc55 = new GameObject("55_On top of the Rosetta Stone Pickup");
                loc55.transform.SetParent(APItemParent);
                loc55.transform.position = new Vector3(seed4.transform.position.x - 1f, seed4.transform.position.y - 1f, seed4.transform.position.z);
                seed4.SetActive(false);
                loc55.AddComponent<ArchipelagoItem>().locId = 55;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 55_On top of the Rosetta Stone Pickup: " + ex.Message);
            }

            try {
                GameObject seed5 = GameObject.Find("World/Region2/(R5-b)/Seeds/Seed (5)");
                GameObject loc56 = new GameObject("56_Long Parry Platforming Room Pickup");
                loc56.transform.SetParent(APItemParent);
                loc56.transform.position = new Vector3(seed5.transform.position.x + 2f, seed5.transform.position.y + 2f, seed5.transform.position.z);
                seed5.SetActive(false);
                loc56.AddComponent<ArchipelagoItem>().locId = 56;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 56_Long Parry Platforming Room Pickup: " + ex.Message);
            }

            // Dark Region
            try {
                GameObject fez = GameObject.Find("World/Region2/Lab/(R13i)>(R15G)/fezHat");
                GameObject loc57 = new GameObject("57_Secret Room Pickup");
                loc57.transform.SetParent(APItemParent);
                loc57.transform.position = fez.transform.position;
                fez.SetActive(false);
                loc57.AddComponent<ArchipelagoItem>().locId = 57;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 57_Secret Room Pickup: " + ex.Message);
            }

            try {
                GameObject seed3 = GameObject.Find("World/Region2/(R5-b)/Seeds/Seed (3)");
                GameObject loc58 = new GameObject("58_Large Room Pickup in the Corner");
                loc58.transform.SetParent(APItemParent);
                loc58.transform.position = new Vector3(seed3.transform.position.x + 2f, seed3.transform.position.y + 2f, seed3.transform.position.z);
                seed3.SetActive(false);
                loc58.AddComponent<ArchipelagoItem>().locId = 58;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 58_Large Room Pickup in the Corner: " + ex.Message);
            }

            try {
                GameObject nullBoss = GameObject.Find("World/Region2/Lab/(R17G) (Corrupted)/null");
                GameObject loc59 = new GameObject("59_Null Reward");
                loc59.transform.SetParent(APItemParent);
                loc59.transform.position = new Vector3(741f, -96f, 0f);
                loc59.SetActive(false);
                nullBoss.AddComponent<ReplaceOnDestroy>().replacement = loc59;
                loc59.AddComponent<ArchipelagoItem>().locId = 59;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 59_Null Reward: " + ex.Message);
            }

            // Smile Shop
            GameObject purchaseTriggerObj = GameObject.Find("World/Smile Shop/OnHeld/Purchase");
            if (purchaseTriggerObj)
                purchaseTriggerObj.AddComponent<ShopPurchaseTrigger>();
            ShopPurchaseTrigger purchaseTrigger = purchaseTriggerObj?.GetComponent<ShopPurchaseTrigger>();

            try {
                GameObject shopItem1 = GameObject.Find("World/Smile Shop/Sword ShopItem");
                shopItem1.name = "60_Smile Shop Item 1";
                ApShopItem shopItem = shopItem1.AddComponent<ApShopItem>();
                shopItem.shopId = 1;
                shopItem.price = 2;
                shopItem.locId = 60;
                purchaseTrigger.items.Add(shopItem);
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 60_Smile Shop Item 1: " + ex.Message);
            }

            try {
                GameObject shopItem2 = GameObject.Find("World/Smile Shop/Shroud ShopItem");
                shopItem2.name = "61_Smile Shop Item 2";
                ApShopItem shopItem = shopItem2.AddComponent<ApShopItem>();
                shopItem.shopId = 2;
                shopItem.price = 4;
                shopItem.locId = 61;
                purchaseTrigger.items.Add(shopItem);
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 61_Smile Shop Item 2: " + ex.Message);
            }

            try {
                GameObject shopItem3 = GameObject.Find("World/Smile Shop/Magic Recharge ShopItem");
                shopItem3.name = "62_Smile Shop Item 3";
                ApShopItem shopItem = shopItem3.AddComponent<ApShopItem>();
                shopItem.shopId = 3;
                shopItem.price = 2;
                shopItem.locId = 62;
                purchaseTrigger.items.Add(shopItem);
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 62_Smile Shop Item 3: " + ex.Message);
            }

            try {
                GameObject shopItem4 = GameObject.Find("World/Smile Shop/Parry Recharge ShopItem");
                shopItem4.name = "63_Smile Shop Item 4";
                ApShopItem shopItem = shopItem4.AddComponent<ApShopItem>();
                shopItem.shopId = 4;
                shopItem.price = 2;
                shopItem.locId = 63;
                purchaseTrigger.items.Add(shopItem);
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 63_Smile Shop Item 4: " + ex.Message);
            }

            try {
                GameObject partyHat = GameObject.Find("World/Smile Shop/Hat room/Pedestals/partyHat pickup");
                GameObject loc64 = new GameObject("64_Dash Puzzle Reward");
                loc64.transform.SetParent(APItemParent);
                loc64.transform.position = partyHat.transform.position;
                loc64.SetActive(false);
                partyHat.AddComponent<ReplaceOnEnable>().replacement = loc64;
                loc64.AddComponent<ArchipelagoItem>().locId = 64;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 64_Dash Puzzle Reward: " + ex.Message);
            }

            if (!betweenListener) betweenListener = GameObject.Find("Manager intro")?.GetComponent<BetweenListener>();

            try {
                GameObject loc65 = new GameObject("65_Between Construct");
                loc65.transform.SetParent(APItemParent);
                loc65.SetActive(false);
                loc65.AddComponent<ArchipelagoItem>().locId = 65;
                if (!betweenListener.apItems.ContainsKey("construct")) betweenListener.apItems.Add("construct", loc65);
                else betweenListener.apItems["construct"] = loc65;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 65_Between Construct: " + ex.Message);
            }

            try {
                GameObject loc66 = new GameObject("66_Between Serpent");
                loc66.transform.SetParent(APItemParent);
                loc66.SetActive(false);
                loc66.AddComponent<ArchipelagoItem>().locId = 66;
                if (!betweenListener.apItems.ContainsKey("serpent")) betweenListener.apItems.Add("serpent", loc66);
                else betweenListener.apItems["serpent"] = loc66;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 66_Between Serpent: " + ex.Message);
            }

            try {
                GameObject loc67 = new GameObject("67_Between Wizard");
                loc67.transform.SetParent(APItemParent);
                loc67.SetActive(false);
                loc67.AddComponent<ArchipelagoItem>().locId = 67;
                if (!betweenListener.apItems.ContainsKey("wizard")) betweenListener.apItems.Add("wizard", loc67);
                else betweenListener.apItems["wizard"] = loc67;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 67_Between Wizard: " + ex.Message);
            }

            try {
                GameObject loc68 = new GameObject("68_Hot Spring");
                loc68.transform.SetParent(APItemParent);
                loc68.SetActive(false);
                loc68.AddComponent<ArchipelagoItem>().locId = 68;
                if (!betweenListener.apItems.ContainsKey("fountain")) betweenListener.apItems.Add("fountain", loc68);
                else betweenListener.apItems["fountain"] = loc68;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 68_Hot Spring: " + ex.Message);
            }

            // locations 69 and 70 (between rewards) are cutscene rewards
        }

        private static void PlaceItemsMemory() {
            return; // memory doesn't actually have items
        }

        private static void PlaceItemsOuterVoid() {

        }

        private static void CheckForCutsceneReward(string sceneName) {
            if (!gamestate) return;
            switch (sceneName) {
                case "TheFalseEnding":
                    gamestate.SaveFlag("FalseEnding");
                    break;
                case "TheGoodEnding":
                    gamestate.SaveFlag("GoodEnding");
                    break;
                case "TheTrueEnding":
                    gamestate.SaveFlag("TrueEnding");
                    break;
                case "Smilemask":
                    gamestate.SaveFlag("SmilemaskEnding");
                    break;
                case "PerfectClarity":
                    gamestate.SaveFlag("PerfectClarity");
                    break;
                case "Omnipotence":
                    gamestate.SaveFlag("OmnipotenceEnding");
                    break;
                case "TheVeryEnd":
                    gamestate.SaveFlag("EpilogueEnding");
                    break;
                case "Escape":
                    if (!client) return;
                    if (!client.client.itemCache.checkedLocations.Contains(69))
                        client.client.CollectItem(69);
                    if (!client.client.itemCache.checkedLocations.Contains(70))
                        client.client.CollectItem(70);
                    break;
            }
        }

        private static int lastSceneHandle = -1;
        private static GamestateManager gamestate;
        private static BetweenListener betweenListener;
        private static ClientWrapper client;
    }
}