using System;
using System.Collections.Generic;
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
                loc26.transform.position = seed6.transform.position;
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

            // George rewards need a seperate script
            try {
                GameObject loc32 = new GameObject("32_George Reward 1");
                loc32.transform.SetParent(APItemParent);
                loc32.transform.position = new Vector3(395f, 15f, 0f);
                loc32.SetActive(false);
                loc32.AddComponent<ArchipelagoItem>().locId = 32;
            } catch (Exception ex) {
                MelonLogger.Error("Failed to place 32_George Reward 1: " + ex.Message);
            }

            try {
                GameObject loc33 = new GameObject("33_George Reward 2");
                loc33.transform.SetParent(APItemParent);
                loc33.transform.position = new Vector3(402f, 15f, 0f);
                loc33.SetActive(false);
                loc33.AddComponent<ArchipelagoItem>().locId = 33;
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
                loc47.transform.position = seed2.transform.position;
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