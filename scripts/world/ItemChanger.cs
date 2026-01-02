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
            APItemParent = new GameObject("AP_Items").transform;
            long locId;
            string name;

            // Region 1
            PlaceItem(1, "Starting Item", null, Vector3.zero);
            PlaceItem(2, "Sword Pedestal", GameObject.Find("World/Region1/(R3D)(sword)/Sword").transform, Vector3.zero);

            locId = 3;
            name = "Runic Construct Reward";
            try {
                GameObject dashOrb = GameObject.Find("World/Region1/Runic Construct(R3E)/Dash Orb");
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.transform.position = dashOrb.transform.position;
                dashOrb.SetActive(false);
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
                apItem.SetActive(false);
                dashOrb.AddComponent<ReplaceOnEnable>().replacement = apItem;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            locId = 4;
            name = "Map Pedestal";
            try {
                GameObject map = GameObject.Find("World/Region1/(R2B)(Map)/Map");
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.transform.position = map.transform.position;
                map.transform.position = map.transform.position = new Vector3(-9.95f, -1.4814f, 0f); // for some reason setting playerprefs doesn't give map so just moving it to the player on load
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            PlaceItem(5, "Silver Shard Puzzle 1", GameObject.Find("World/Region1/(R0B) (Fragment1)/Fragment 1").transform, Vector3.zero);
            PlaceItem(6, "Silver Shard Puzzle 2", GameObject.Find("World/Region1/(R6-A) (Fragment2)/Fragment 2").transform, Vector3.zero);
            PlaceItem(7, "Silver Shard Puzzle 3", GameObject.Find("World/Region1/(R8B) (Fragment3)/Fragment 3").transform, Vector3.zero);
            PlaceItem(8, "Smile Token Puzzle 1", GameObject.Find("World/Region1/(R4C) (MegaPuzzle2)/SMILE I").transform, Vector3.zero);
            PlaceItem(9, "Smile Token Puzzle 9", GameObject.Find("World/Region1/(SMILE IX)/SMILE I").transform, Vector3.zero); // not a typo
            PlaceItem(10, "Color Cypher Room Pickup", GameObject.Find("World/Region2/(R5-b)/Seeds/Seed").transform, Vector3.zero);

            locId = 11;
            name = "Master Puzzle 2";
            try {
                GameObject cube2 = GameObject.Find("World/Region1/(R4C) (MegaPuzzle2)/Cube II");
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.transform.position = cube2.transform.position;
                cube2.SetActive(false);
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
                apItem.SetActive(false);
                cube2.AddComponent<ReplaceOnEnable>().replacement = apItem;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            // Region 2
            PlaceItem(12, "Silver Shard Puzzle 4", GameObject.Find("World/Region2/Sector 1/(R1-E) (Fragment4)/Fragment 4").transform, Vector3.zero);
            PlaceItem(13, "Silver Shard Puzzle 5", GameObject.Find("World/Region2/(R7-A) (Fragment 5)/Fragment 5").transform, Vector3.zero);
            PlaceItem(14, "Silver Shard Puzzle 6", GameObject.Find("World/Region2/(R5B) (Fragment 6)/Fragment 6").transform, Vector3.zero);
            PlaceItem(15, "Silver Shard Puzzle 7", GameObject.Find("World/Region2/Sector 2/(R10-D)>(R11-D) (Fragment 7)/Fragment 7").transform, Vector3.zero);
            PlaceItem(16, "Silver Shard Puzzle 8", GameObject.Find("World/Region2/(R7E) (Fragment8)/Fragment 8").transform, Vector3.zero);

            locId = 17;
            name = "Silver Shard Puzzle 9";
            try {
                GameObject shard9 = GameObject.Find("World/Region2/(R7F) (Fragment9)/Fragment  9");
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.transform.position = shard9.transform.position;
                shard9.SetActive(false);
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
                apItem.SetActive(false);
                shard9.AddComponent<ReplaceOnEnable>().replacement = apItem;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            PlaceItem(18, "Silver Shard Puzzle 15", GameObject.Find("World/Region2/(R12B) (Fragment 15)/Fragment 15").transform, Vector3.zero);
            PlaceItem(19, "Smile Token Puzzle 3", GameObject.Find("World/Region2/Sector 2/(R7-F)(Secret)/SMIlE III").transform, Vector3.zero);
            PlaceItem(20, "Smile Token Puzzle 6", GameObject.Find("World/Region2/(R10-A)(SMILE VI)/SMIlE VI").transform, Vector3.zero);
            PlaceItem(21, "Smile Token Puzzle 8", GameObject.Find("World/Region2/Sector 1/(SMILE VIII)/SMIlE VIII").transform, Vector3.zero);
            PlaceItem(22, "Flower Puzzle Reward", GameObject.Find("World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/SMIlE X").transform, Vector3.zero);
            PlaceItem(23, "Gilded Serpent Reward", GameObject.Find("World/Region2/(R10A) (Boss2)/Grapple Worm").transform, Vector3.zero);
            PlaceItem(24, "Cameo Room Pickup", GameObject.Find("World/Region2/Sector 1/(R5-D) (Cameo Room)/propellerHat").transform, Vector3.zero);

            locId = 25;
            name = "Car Hall Pickup";
            try {
                GameObject coneHat = GameObject.Find("World/Region2/Sector 2/(R7-F)(Secret)/Car grouping/coneHat");
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.transform.position = coneHat.transform.position;
                coneHat.SetActive(false);
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
                apItem.SetActive(false);
                GameObject carGrouping = coneHat.transform.parent.gameObject;
                carGrouping.AddComponent<ReplaceOnEnable>().replacement = apItem;
                carGrouping.GetComponent<ReplaceOnEnable>().destroyTarget = coneHat;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            PlaceItem(26, "Near Shooters Pickup", GameObject.Find("World/Region2/(R5-b)/Seeds/Seed (6)").transform, new Vector3(-2f, 0f, 0f));
            PlaceItem(27, "Collapsed Tunnel Pickup", GameObject.Find("World/Region2/(R5-b)/Seeds/Seed (7)").transform, Vector3.zero);
            PlaceItem(28, "Nest Room Pickup", GameObject.Find("World/Region2/(R5-b)/Seeds/Seed (9)").transform, Vector3.zero);
            PlaceItem(29, "Serpent Boss Room Pickup", GameObject.Find("World/Region2/(R5-b)/Seeds/Seed (10)").transform, Vector3.zero);
            PlaceItem(30, "Shadow Chase Reward", GameObject.Find("World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/Dash Attack Orb").transform, Vector3.zero);
            PlaceItem(31, "Water Room Pickup", GameObject.Find("World/Region2/Sector 4/(WATER ROOM)/topHat").transform, Vector3.zero);

            locId = 32;
            name = "George Reward 1";
            try {
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.transform.position = new Vector3(395f, 15f, 0f);
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
                apItem.AddComponent<SeedCounter>().hiddenPosition = new Vector3(395f, 50f, 0f);
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            locId = 33;
            name = "George Reward 2";
            try {
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.transform.position = new Vector3(402f, 15f, 0f);
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
                apItem.AddComponent<SeedCounter>().hiddenPosition = new Vector3(402f, 50f, 0f);
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            PlaceItem(34, "Shadow Chase Pickup", GameObject.Find("World/Region2/(R5-b)/Seeds/Seed (8)").transform, Vector3.zero);

            locId = 35;
            name = "Master Puzzle 1";
            try {
                GameObject cube1 = GameObject.Find("World/Region2/Sector 2/R10-C (Map Room)/Cube");
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.transform.position = cube1.transform.position;
                cube1.SetActive(false);
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
                cube1.AddComponent<ReplaceOnEnable>().replacement = apItem;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            // Region 3
            PlaceItem(36, "Green Stone Trial", GameObject.Find("World/Region3/Green/(R4G)>(R3F) (GLYPHSTONE)/GlyphStone").transform, Vector3.zero);

            locId = 37;
            name = "Blue Stone Trial";
            try {
                GameObject blueStone = GameObject.Find("World/Region3/Blue/(R12G)>(R13F) (GLYPHSTONE)/GlyphStone");
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.transform.position = blueStone.transform.position;
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
                apItem.SetActive(false);
                blueStone.AddComponent<ReplaceOnDestroy>().replacement = apItem;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            locId = 38;
            name = "Red Stone Trial";
            try {
                GameObject redStone = GameObject.Find("World/Region3/Red/(R7K)>(R9K) (GLYPHSTONE)/GlyphStone");
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.transform.position = redStone.transform.position;
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
                apItem.SetActive(false);
                redStone.AddComponent<ReplaceOnDestroy>().replacement = apItem;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            PlaceItem(39, "Silver Shard Puzzle 10", GameObject.Find("World/Region3/Blue/(R14A) (Fragment 10)/Fragment 10").transform, Vector3.zero);
            PlaceItem(40, "Silver Shard Puzzle 11", GameObject.Find("World/Region3/Green/(R2C) (Fragment 11)/Fragment 11").transform, Vector3.zero);

            locId = 41;
            name = "Silver Shard Puzzle 12";
            try {
                GameObject shard12 = GameObject.Find("World/Region3/Black/(R9C) (Fragment 12)/Fragment 12");
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.transform.position = shard12.transform.position;
                shard12.SetActive(false);
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
                apItem.SetActive(false);
                shard12.AddComponent<ReplaceOnEnable>().replacement = apItem;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            PlaceItem(42, "Silver Shard Puzzle 13", GameObject.Find("World/Region3/Red/(R9G) (Fragment 13)/Fragment 13").transform, Vector3.zero);
            PlaceItem(43, "Silver Shard Puzzle 14", GameObject.Find("World/Region3/Blue/(R14E) (Fragment 14)/Fragment 14").transform, Vector3.zero);
            PlaceItem(44, "Smile Token Puzzle 2", GameObject.Find("World/Region3/Black/(R6D) (SMILE II)/SMILE II").transform, Vector3.zero);

            locId = 45;
            name = "Smile Token Puzzle 7";
            try {
                GameObject token7 = GameObject.Find("World/Region3/Blue/(SMILE VII)/SMILE VII");
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(token7.transform);    // used to follow token positon but at the cost of not appearing with other AP items in heiarchy tree
                apItem.transform.position = token7.transform.position;
                token7.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0f);
                UnityEngine.Object.Destroy(token7.GetComponent<Pickup>());
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            GameObject collapseController = GameObject.Find("World/Region3/Black/(R7D)>(R9F) The False Primary Glyph/Collapse Sequence Controller");
            if (!collapseController) MelonLogger.Msg("Collapse controller not found");

            locId = 46;
            name = "Wizard Reward";
            try {
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.transform.position = new Vector3(475f, -225f, 0f);
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
                apItem.SetActive(false);
                collapseController.AddComponent<ReplaceOnEnable>().replacement = apItem;
                collapseController.GetComponent<ReplaceOnEnable>().doNotDestroy = true;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            PlaceItem(47, "Room Below Wizard Pickup", GameObject.Find("World/Region2/(R5-b)/Seeds/Seed (2)").transform, new Vector3(-2f, 2f, 0f));
            PlaceItem(48, "Master Puzzle 3", GameObject.Find("World/Region3/Red/(R11J) (CUBE III)/Cube III").transform, Vector3.zero);

            // Region 4
            PlaceItem(49, "Spearman Reward", GameObject.Find("World/Region2/Sector 3/(R1E) (Parry)/Parry").transform, Vector3.zero);

            locId = 50;
            name = "Multipary Gold Shard Puzzle";
            try {
                GameObject gold2 = GameObject.Find("World/Region2/Sector 3/(R-1H) (GOLDEN FRAGMENT II)/Gold Fragment II");
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.transform.position = gold2.transform.position;
                gold2.SetActive(false);
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
                apItem.SetActive(false);
                gold2.AddComponent<ReplaceOnEnable>().replacement = apItem;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            PlaceItem(51, "Platforming Gold Shard Room", GameObject.Find("World/Region2/Sector 3/(R-4F) (GOLD FRAGMENT I)/Gold Fragment").transform, Vector3.zero);

            locId = 52;
            name = "Flower Puzzle Reward";
            try {
                GameObject gold3 = GameObject.Find("World/Region2/Sector 3/(R-4H)(Flower)/Gold Fragment III");
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.transform.position = gold3.transform.position;
                gold3.SetActive(false);
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
                apItem.SetActive(false);
                gold3.AddComponent<ReplaceOnEnable>().replacement = apItem;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            PlaceItem(53, "Smile Token Puzzle 4", GameObject.Find("World/Region2/Sector 3/(R-4C) (SMILE IV)/SMIlE IV").transform, Vector3.zero);
            PlaceItem(54, "Smile Token Puzzle 5", GameObject.Find("World/Region2/Sector 3/(R2D) (Smile V)/SMIlE V").transform, Vector3.zero);
            PlaceItem(55, "On top of the Rosetta Stone Pickup", GameObject.Find("World/Region2/(R5-b)/Seeds/Seed (4)").transform, new Vector3(-1f, -1f, 0f));
            PlaceItem(56, "Long Parry Platforming Room Pickup", GameObject.Find("World/Region2/(R5-b)/Seeds/Seed (5)").transform, new Vector3(2f, 2f, 0f));

            // Dark Region
            PlaceItem(57, "Secret Room Pickup", GameObject.Find("World/Region2/Lab/(R13i)>(R15G)/fezHat").transform, Vector3.zero);
            PlaceItem(58, "Large Room Pickup in the Corner", GameObject.Find("World/Region2/(R5-b)/Seeds/Seed (3)").transform, new Vector3(2f, 2f, 0f));

            locId = 59;
            name = "Null Reward";
            try {
                GameObject nullBoss = GameObject.Find("World/Region2/Lab/(R17G) (Corrupted)/null");
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.transform.position = new Vector3(741f, -96f, 0f);
                apItem.SetActive(false);
                nullBoss.AddComponent<ReplaceOnDestroy>().replacement = apItem;
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            // Smile Shop
            GameObject purchaseTriggerObj = GameObject.Find("World/Smile Shop/OnHeld/Purchase");
            if (purchaseTriggerObj) purchaseTriggerObj.AddComponent<ShopPurchaseTrigger>();
            ShopPurchaseTrigger purchaseTrigger = purchaseTriggerObj?.GetComponent<ShopPurchaseTrigger>();

            locId = 60;
            name = "Smile Shop Item 1";
            try {
                GameObject shopItem1 = GameObject.Find("World/Smile Shop/Sword ShopItem");
                shopItem1.name = $"{locId}_{name}";
                ApShopItem shopItem = shopItem1.AddComponent<ApShopItem>();
                shopItem.shopId = 1;
                shopItem.price = 2;
                shopItem.locId = locId;
                purchaseTrigger.items.Add(shopItem);
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            locId = 61;
            name = "Smile Shop Item 2";
            try {
                GameObject shopItem2 = GameObject.Find("World/Smile Shop/Shroud ShopItem");
                shopItem2.name = $"{locId}_{name}";
                ApShopItem shopItem = shopItem2.AddComponent<ApShopItem>();
                shopItem.shopId = 2;
                shopItem.price = 4;
                shopItem.locId = locId;
                purchaseTrigger.items.Add(shopItem);
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            locId = 62;
            name = "Smile Shop Item 3";
            try {
                GameObject shopItem3 = GameObject.Find("World/Smile Shop/Magic Recharge ShopItem");
                shopItem3.name = $"{locId}_{name}";
                ApShopItem shopItem = shopItem3.AddComponent<ApShopItem>();
                shopItem.shopId = 3;
                shopItem.price = 2;
                shopItem.locId = locId;
                purchaseTrigger.items.Add(shopItem);
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            locId = 63;
            name = "Smile Shop Item 4";
            try {
                GameObject shopItem4 = GameObject.Find("World/Smile Shop/Parry Recharge ShopItem");
                shopItem4.name = $"{locId}_{name}";
                ApShopItem shopItem = shopItem4.AddComponent<ApShopItem>();
                shopItem.shopId = 4;
                shopItem.price = 2;
                shopItem.locId = locId;
                purchaseTrigger.items.Add(shopItem);
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            locId = 64;
            name = "Dash Puzzle Reward";
            try {
                GameObject partyHat = GameObject.Find("World/Smile Shop/Hat room/Pedestals/partyHat pickup");
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.transform.position = partyHat.transform.position;
                apItem.SetActive(false);
                partyHat.AddComponent<ReplaceOnEnable>().replacement = apItem;
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            if (!betweenListener) betweenListener = GameObject.Find("Manager intro")?.GetComponent<BetweenListener>();

            locId = 65;
            name = "Between Construct";
            try {
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.SetActive(false);
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
                if (!betweenListener.apItems.ContainsKey("construct")) betweenListener.apItems.Add("construct", apItem);
                else betweenListener.apItems["construct"] = apItem;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            locId = 66;
            name = "Between Serpent";
            try {
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.SetActive(false);
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
                if (!betweenListener.apItems.ContainsKey("serpent")) betweenListener.apItems.Add("serpent", apItem);
                else betweenListener.apItems["serpent"] = apItem;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            locId = 67;
            name = "Between Wizard";
            try {
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.SetActive(false);
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
                if (!betweenListener.apItems.ContainsKey("wizard")) betweenListener.apItems.Add("wizard", apItem);
                else betweenListener.apItems["wizard"] = apItem;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            locId = 68;
            name = "Hot Spring";
            try {
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.SetActive(false);
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
                if (!betweenListener.apItems.ContainsKey("fountain")) betweenListener.apItems.Add("fountain", apItem);
                else betweenListener.apItems["fountain"] = apItem;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }

            // locations 69 and 70 (between rewards) are cutscene rewards

            locId = 71;
            name = "Escape Normal Sequence Pickup";
            try {
                GameObject apItem = new GameObject($"{locId}_{name}");
                apItem.transform.SetParent(APItemParent);
                apItem.SetActive(false);
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
                collapseController.AddComponent<BombHatPickupReplacer>().replacement = apItem;

            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {locId}_{name}: {ex.Message}");
            }
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

        private static GameObject PlaceItem(long locId, string name, Transform target, Vector3 offset) {
            GameObject apItem = new GameObject($"{locId}_{name}");
            try {
                apItem.transform.SetParent(APItemParent);
                if (target) {
                    apItem.transform.position = new Vector3(target.transform.position.x + offset.x, target.transform.position.y + offset.y, target.transform.position.z + offset.z);
                    target.gameObject.SetActive(false);
                } else {
                    apItem.transform.position = offset;
                }
                apItem.AddComponent<ArchipelagoItem>().locId = locId;
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to place {apItem.name}: {ex.Message}");
            }
            return apItem;
        }

        private static int lastSceneHandle = -1;
        private static GamestateManager gamestate;
        private static BetweenListener betweenListener;
        private static ClientWrapper client;
        private static Transform APItemParent;
    }
}