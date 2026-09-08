using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ApGlyphs {
    [HarmonyPatch]
    public static class ButtonManager {
        [HarmonyPatch(typeof(ButtonObj), "Start")]
        [HarmonyPostfix]
        public static void Start(ButtonObj __instance) => Register(__instance);

        public static bool Register(ButtonObj button) {
            if (!button) return false;

            if (awaitingSlotData) {
                randomizeColors = Convert.ToBoolean(client.options["ButtonColorsRandomized"]);
                if (randomizeColors) {
                    if (client.slotData.TryGetValue("button_colors", out object rawColors)) {
                        Dictionary<int, int> buttonColors = ((JObject)rawColors).ToObject<Dictionary<int, int>>();
                        foreach (var (key, value) in buttonColors) {
                            if (value < 0 || value > 5) continue;
                            colorKey.Add(key, (ButtonColor)value);
                        }
                    }
                }

                if (client.slotData.TryGetValue("broken_buttons", out object rawBrokenButtonIds)) {
                    foreach (int buttonId in ((JArray)rawBrokenButtonIds).ToObject<List<int>>()) {
                        brokenButtonIds.Add(buttonId);
                    }
                }

                awaitingSlotData = false;
            }

            if (!randomizeColors) return false;

            if (button.gameObject.GetComponent<ApButton>()) {
                ApButton apb = button.gameObject.GetComponent<ApButton>();
                loadedButtons.Add(apb);
                // MelonLogger.Msg($"Registed button at {apb.path} under {apb.id}.");
                return true;
            }

            string path = button.transform.name;
            Transform current = button.transform;
            while (current.parent) {
                path = current.parent.name + "/" + path;
                current = current.parent;
            }

            if (paths.TryGetValue(path, out int id)) {
                Confirm(button, path, id);
                return true;
            } else if (offendingPaths.Contains(path)) {
                foreach (var (key, value) in fallbackCoords) {
                    if (Vector3.Distance(button.transform.position, key) < 1f) {
                        Confirm(button, path, value);
                        return true;
                    }
                }
            }
            // MelonLogger.Warning($"Failed to register button at {path}.");
            return false;
        }

        private static void Confirm(ButtonObj button, string path, int id) {
            ApButton apButton = button.gameObject.AddComponent<ApButton>();
            apButton.id = id;
            apButton.buttonObj = button;
            apButton.color = colorKey[id];
            apButton.path = path;
            loadedButtons.Add(apButton);
            // MelonLogger.Msg($"Registed button at {path} under {id}.");
        }

        public static void Unregister(ApButton button) {
            if (loadedButtons.Remove(button)) {
                // MelonLogger.Msg($"Successfully unregistered button at {button.path}");
            }
        }

        public static ButtonColor GetButtonColor(int id) => colorKey[id];

        public static bool IsBroken(int id) => brokenButtonIds.Contains(id);

        public static void CollectShard(int id) {
            if (!IsBroken(id)) return;

            brokenButtonIds.Remove(id);
            foreach (ApButton button in loadedButtons) {
                if (button.id == id) {
                    button.Fix();
                    break;
                }
            }
        }

        public static void UpdateButtons() {
            if (!inventory) inventory = SceneSearcher.Find("Manager intro")?.GetComponent<InventoryManager>();
            if (!inventory) return;
            foreach (var (key, value) in inventory.items) {
                MelonLogger.Msg($"{key}: {value}");
                if (key.StartsWith("Button Shard")) {
                    MelonLogger.Msg($"Found button shard {Convert.ToInt32(key.Split(' ')[2])}");
                    CollectShard(Convert.ToInt32(key.Split(' ')[2]));
                }
            }
        }

        private static readonly Dictionary<string, int> paths = new Dictionary<string, int>() {
            {"World/Region1/(R3A)/Save Button (HDD)/Button", 0},
            {"World/Region1/(R1E)/Launcher/Button/Button", 1},
            {"World/Region1/(R1E)/Launcher (1)/Button/Button", 2},
            {"World/Region1/(R1C)/Launcher (2)/Button/Button", 3},
            {"World/Region1/(R1C)/Button (1)/Button", 4},
            {"World/Region1/(R1C)/Button (2)/Button", 5},
            {"World/Region1/(R1B)/Launcher (3)/Button/Button", 6},
            {"World/Region1/(R1B)/Launcher (4)/Button/Button", 7},
            {"World/Region1/(R2B)(Map)/Button (1)/Button", 8},
            {"World/Region1/(R0B) (Fragment1)/Button/Button", 9},
            {"World/Region1/(SMILE IX)/Tiles/Launcher/Button/Button", 10},
            {"World/Region1/(R1B)/Save Button (1)/Button", 11},
            {"World/Region1/(R3C)/Button/Button", 12},
            {"World/Region1/(R2C)/Button/Button", 13},
            {"World/Region1/(R4E)/Save Button (1)/Button", 14},
            {"World/Region1/(R6A) > (R7A)/(R7A)/Button (1)/Button", 15},
            {"World/Region1/Transition/MegaDoor (1)/Button/Button", 16},
            {"World/Region1/Transition/MegaDoor (2)/Button (1)/Button", 17},
            {"World/Region1/Transition/Save Button/Button (HDD)", 18},
            {"World/Region1/Transition/Save Button (1)/Button (HDD)", 19},
            {"World/Region2/(R2A)/Button/Button", 20},
            {"World/Region2/(R5B) (Fragment 6)/Attack Button/Button", 21},
            {"World/Region2/(R7A) (Boss Door)/Multitrigger Door/Button (1)/Button", 22},
            {"World/Region2/(R5A)/Save Button (1)/Button", 23},
            {"World/Region2/(R7A) (Boss Door)/Save Button/Button", 24},
            {"World/Region2/Sector 1/(R1-E) (Fragment4)/Door/Save Button/Button", 25},
            {"World/Region2/(R6-B)/Save Button/Button", 26},
            {"World/Region2/Sector 1/(R3-E) > (R2-F)/Save Button/Button", 27},
            {"World/Region2/Sector 1/(R4-E)/Button/Button", 28},
            {"World/Region2/Sector 1/(R3-E) > (R2-F)/Button/Button", 29},
            {"World/Region2/Sector 1/(R1-E) (Fragment4)/Attack Button/Button", 30},
            {"World/Region2/Sector 1/(R4-F)/Button/Button", 31},
            {"World/Region2/(R6-C) (SMILE SHOP ENTRANCE)/Save Button/Button", 32},
            {"World/Region2/Sector 2/(R8-B)/Attack Button/Button", 33},
            {"World/Region2/Sector 2/(R8-C)/Attack Button (1)/Button", 34},
            {"World/Region2/Sector 2/(R8-C)/Tiles/Platform/Button/Button", 35},
            {"World/Region2/Sector 2/(R8-D)/Attack Button (1)/Button", 36},
            {"World/Region2/Sector 2/(R8-D)/Launcher/Button/Button", 37},
            {"World/Region2/Sector 2/(R9-D)/Attack Button/Button", 38},
            {"World/Region2/Sector 2/(R8-B)/Tiles/Attack Button (1)/Button", 39},
            {"World/Region2/Sector 2/(R6-F)(Button2)/Save Button/Button", 40},
            //{"World/Region2/Sector 2/(R6-E)<(R9-E) /Attack Button/Button", 41}, // duplicate name
            {"World/Region2/Sector 2/(R6-F)(Button2)/Button (1)/Button", 42},
            {"World/Region2/Sector 2/(R6-E)<(R9-E) /Attack Button (2)/Button", 43},
            //{"World/Region2/Sector 2/(R6-E)<(R9-E) /Attack Button/Button (renamed 1)", 44}, // duplicate name
            {"World/Region2/Sector 2/(R6-E)<(R9-E) /Attack Button (3)/Button", 45},
            {"World/Region2/Sector 2/(R6-E)<(R9-E) /Attack Button (1)/Button", 46},
            //{"World/Region2/Sector 2/(R6-E)<(R9-E) /Attack Button/Button (renamed 2)", 47}, // duplicate name
            {"World/Region2/Sector 2/(R6-E)<(R9-E) /Save Button/Button", 48},
            {"World/Region2/Sector 2/(R10-E) (Death Door)/Tiles/Multitrigger Door (1)/Button (1)/Button", 49},
            {"World/Region2/Sector 2/(R6-E)<(R9-E) /Save Button (1)/Button", 50},
            {"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/DashButton/Button", 51},
            {"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/Attack Button/Button", 52},
            {"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/DashButton (1)/Button", 53},
            {"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/Attack Button (1)/Button", 54},
            {"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/Button/Button", 55},
            //{"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/Dash Attack Button/Button", 56},    // duplicate name
            //{"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/Dash Attack Button/Button (renamed 3)", 57},    // duplicate name
            {"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/B8/Button", 58},
            {"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/B7/Button", 59},
            {"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/B6/Button", 60},
            {"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/B5/Button", 61},
            {"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/B3/Button", 62},
            {"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/B0/Button", 63},
            //{"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/FAKE/Button", 64}, // duplicate name
            //{"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/FAKE/Button (renamed 4)", 65}, // duplicate name
            {"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/B4/Button", 66},
            //{"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/FAKE/Button (renamed 5)", 67}, // duplicate name
            //{"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/FAKE/Button (renamed 6)", 68}, // duplicate name
            //{"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/FAKE/Button (renamed 7)", 69}, // duplicate name
            //{"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/FAKE/Button (renamed 8)", 70}, // duplicate name
            {"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/Launcher/Button/Button", 71},
            {"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/B2/Button", 72},
            {"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/B1/Button", 73},
            {"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/B1/DashButton (3)/Button", 74},
            {"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/Platform (1)/Button/Button", 75},
            {"World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/Multitrigger Door (1)/Button (1)/Button", 76},
            {"World/Region2/Sector 4/(R8C) > (R9C)/Button/Button", 77},
            {"World/Region2/(R6C)/Save Button (HDD)/Button", 78},
            {"World/Region2/Sector 4/(R5D) > (R5G)/Button (2)/Button", 79},
            //{"World/Region2/Sector 4/(R5D) > (R5G)/Button/Button", 80}, // duplicate name
            //{"World/Region2/Sector 4/(R5D) > (R5G)/Button/Button (renamed 9)", 81}, // duplicate name
            //{"World/Region2/Sector 4/(R5D) > (R5G)/Button/Button (renamed 10)", 82}, // duplicate name
            //{"World/Region2/Sector 4/(R5D) > (R5G)/Button/Button (renamed 11)", 83}, // duplicate name
            {"World/Region2/Sector 4/(R5H)/Save Button/Button", 84},
            {"World/Region2/Sector 4/(WATER ROOM)/Button /Button", 85},
            {"World/Region2/Sector 4/(R5H)/Button (2)/Button", 86},
            //{"World/Region2/(R10A) (Boss2)/Save Button/Button", 87}, // duplicate name
            {"World/Region2/(R10A) (Boss2)/Save Button (1)/Button", 87},
            {"World/Region2/(R10A) (Boss2)/Attack Button/Button", 88},
            {"World/Region2/(R10A) (Boss2)/Button/Button", 89},
            {"World/Region2/(R11E) < (R8E)/Button/Button", 90},
            {"World/Region2/(R7E) (Fragment8)/Button/Button", 91},
            //{"World/Region2/(R10A) (Boss2)/Save Button/Button (renamed 12)", 92}, // duplicate name
            {"World/Region2/(R8F)>(R8I)Transition/Save Button (HDD)/Button", 93},
            {"World/Region3/Green/(R2A) (Fragment 10 Mirror)/Attack Button/Button", 94},
            {"World/Region3/Green/(R2A) (Fragment 10 Mirror)/DashButton/Button", 95},
            {"World/Region3/Blue/(R14A) (Fragment 10)/Attack Button/Button", 94},
            {"World/Region3/Blue/(R14A) (Fragment 10)/DashButton/Button", 95},
            {"World/Region3/Green/(R4A)>(R3A) /Attack Button/Button", 96},
            {"World/Region3/Black/(R8B)/DashButton/Button", 97},
            {"World/Region3/Black/(R8B)/DashButton (1)/Button", 98},
            {"World/Region3/(R8A)Transition/Save Button (HDD)/Button", 99},
            {"World/Region3/Green/(R4B)>(R3B) /Save Button/Button", 100},
            {"World/Region3/Green/(R4E)>(R3E)/Attack Button/Button", 101},
            {"World/Region3/Green/(R4E)>(R3E)/Attack Button (2)/Button", 102},
            {"World/Region3/Green/(R4E)>(R3E)/Attack Button (1)/Button", 103},
            {"World/Region3/Green/(R4E)>(R3E)/Tiles/Multitrigger Door/Button (1)/Button", 104},
            {"World/Region3/Green/(R4D)>(R3D)/Save Button/Button", 105},
            {"World/Region3/Green/(R4G)>(R3F) (GLYPHSTONE)/Button/Button", 106},
            {"World/Region3/Green/(R4G)>(R3F) (GLYPHSTONE)/Attack Button/Button", 107},
            {"World/Region3/Green/(R4G)>(R3F) (GLYPHSTONE)/Multitrigger Door/Button (1)/Button", 108},
            {"World/Region3/Green/(R4E)>(R3E)/Save Button/Button", 109},
            {"World/Region3/Black/(R8C)/DashButton/Button", 110},
            {"World/Region3/Red/(R9G) (Fragment 13)/EnemyButton/Button", 111},
            {"World/Region3/Black/(R9C) (Fragment 12)/Button/Button", 112},
            {"World/Region3/Black/(R9C) (Fragment 12)/Button (1)/Button", 113},
            {"World/Region3/Black/(R9C) (Fragment 12)/Button (3)/Button", 114},
            {"World/Region3/Black/(R9C) (Fragment 12)/Button (2)/Button", 115},
            {"World/Region3/Red/(R8G)>(R8I)/DashButton (2)/Button", 116},
            {"World/Region3/Red/(R8G)>(R8I)/DashButton (1)/Button", 117},
            {"World/Region3/Red/(R8G)>(R8I)/DashButton/Button", 118},
            {"World/Region3/Red/(R8G)>(R8I)/Tiles/Multitrigger Door/Button (1)/Button", 119},
            {"World/Region3/Black/(R8C)/Save Button (HDD)/Button", 120},
            {"World/Region3/Red/(R10J) (counter room)/Multitrigger Door/Button (1)/Button", 121},
            {"World/Region3/Red/(R8J)/Save Button/Button", 122},
            {"World/Region3/Blue/(R12B)>(R13B)/Attack Button/Button", 123},
            {"World/Region3/Blue/(R12B)>(R13B)/Button/Button", 124},
            {"World/Region3/Blue/(R12B)>(R13B)/Save Button/Button", 125},
            {"World/Region3/Blue/(R12E)>(R13E)/Button (1)/Button", 126},
            {"World/Region3/Blue/(R12E)>(R13E)/Button/Button", 127},
            {"World/Region3/Blue/(R12E)>(R13E)/Attack Button/Button", 128},
            {"World/Region3/Blue/(R12E)>(R13E)/Tiles/Multitrigger Door/Button (1)/Button", 129},
            {"World/Region3/Blue/(R12D)>(R13D)/Save Button/Button", 130},
            {"World/Region3/Blue/(R12G)>(R13F) (GLYPHSTONE)/Platforming/Tile/Attack Button/Button", 131},
            {"World/Region3/Blue/(R12G)>(R13F) (GLYPHSTONE)/Platforming/Tile  (3)/Attack Button/Button", 132},
            {"World/Region3/Blue/(R12G)>(R13F) (GLYPHSTONE)/Platforming/Tile  (11)/Attack Button/Button", 133},
            {"World/Region3/Blue/(R12G)>(R13F) (GLYPHSTONE)/Platforming/Tile  (16)/Attack Button/Button", 134},
            {"World/Region3/Blue/(R12E)>(R13E)/Save Button/Button", 135},
            {"World/Region2/Sector 3/(R-1C)/Tiles/Attack Button/Button", 136},
            {"World/Region2/Sector 3/(R0C)/Tiles/DashButton/Button", 137},
            {"World/Region2/Sector 3/(R2C)/Tiles/Dash Attack Button/Button", 138},
            {"World/Region2/Sector 3/(R-1E)/Save Button/Button", 139},
            {"World/Region2/Sector 3/(R3E)/Parry Button/Button", 140},
            {"World/Region2/Sector 3/(R3F)/Parry Button/Button", 141},
            {"World/Region2/Sector 3/(R2F)/Tiles/Dash Attack Button/Button", 142},
            {"World/Region2/Sector 3/(R2F)/Parry Button/Button", 143},
            {"World/Region2/Sector 3/(R1F)/Parry Button (1)/Button", 144},
            {"World/Region2/Sector 3/(R1F)/Parry Button/Button", 145},
            {"World/Region2/Sector 3/(R0F)/Tiles/Attack Button/Button", 146},
            {"World/Region2/Sector 3/(R0F)/Parry Button (1)/Button", 147},
            {"World/Region2/Sector 3/(R-1F)/Parry Button (1)/Button", 148},
            {"World/Region2/Sector 3/(R-1F)/Attack Button (1)/Button", 149},
            {"World/Region2/Sector 3/(R-2F)/Parry Button (4)/Button", 150},
            {"World/Region2/Sector 3/(R-2F)/Parry Button/Button", 151},
            {"World/Region2/Sector 3/(R-2F)/Parry Button (3)/Button", 152},
            {"World/Region2/Sector 3/(R-2F)/Parry Button (1)/Button", 153},
            {"World/Region2/Sector 3/(R-2F)/Parry Button (2)/Button", 154},
            {"World/Region2/Sector 3/(R-2F)/Multitrigger Door/Button (1)/Button", 155},
            {"World/Region2/Sector 3/(R-4D)/Tiles/Attack Button/Button", 156},
            {"World/Region2/Sector 3/(R-4D)/Tiles/Attack Button (1)/Button", 157},
            {"World/Region2/Sector 3/(R-4D)/Tiles/Attack Button (2)/Button", 158},
            {"World/Region2/Sector 3/(R-5E)/Tiles/DashButton/Button", 159},
            {"World/Region2/Sector 3/(R-5F)/DashButton/Button", 160},
            {"World/Region2/Sector 3/(R-3C) (MULTIPARRY)/Parry Button/Button", 161},
            {"World/Region2/Sector 3/(R-1H) (GOLDEN FRAGMENT II)/Parry Button/Button", 162},
            {"World/Region2/Sector 3/(R-2G)/Parry Button/Button", 163},
            {"World/Region2/Sector 3/(R0G)/Parry Button (2)/Button", 164},
            {"World/Region2/Sector 3/(R0G)/Parry Button (4)/Button", 164},
            {"World/Region2/Sector 3/(R0G)/Parry Button/Button", 165},
            {"World/Region2/Sector 3/(R0G)/Parry Button (1)/Button", 166},
            {"World/Region2/Sector 3/(R0G)/Parry Button (3)/Button", 166},
            {"World/Region2/Sector 3/(R2G)/Tiles/DashButton/Button", 167},
            {"World/Region2/Sector 3/(R2G)/Tiles/Attack Button/Button", 168},
            {"World/Region2/Sector 3/(R2G)/Parry Button/Button", 169},
            {"World/Region2/Sector 3/(R-3F)/Save Button/Button", 170},
            {"World/Region2/Sector 3/(R-4H)(Flower)/Save Button (1)/Button", 171},
            {"World/Region2/Sector 3/(R3I) (murder)/Tiles/Attack Button/Button", 172},
            {"World/Region2/Sector 3/(R3H)/Save Button/Button", 173},
            //{"World/Region2/Sector 3/(R0H)/Parry Button (1)/Button", 174}, // duplicate name
            //{"World/Region2/Sector 3/(R0H)/Parry Button (1)/Button (renamed 13)", 175}, // duplicate name
            //{"World/Region2/Sector 3/(R0H)/Parry Button/Button", 176}, //duplicate name
            //{"World/Region2/Sector 3/(R0H)/Parry Button/Button (renamed 14)", 177}, //duplicate name
            {"World/Region2/Sector 3/(R0I)/Parry Button/Button", 178},
            {"World/Region2/Sector 3/(R-2I)/Parry Button/Button", 179},
            {"World/Region2/Sector 3/(R-4I)/Parry Button/Button", 180},
            {"World/Region2/Sector 3/(R-3I)/Save Button/Button", 181},
            {"World/Region2/Sector 3/(R-3K) (Vault Door)/Tiles/Button/Button", 182},
            {"World/Region2/Sector 3/(R-3K) (Vault Door)/Tiles/Button (1)/Button", 183},
            {"World/Region2/Sector 3/(R-3K) (Vault Door)/Tiles/Multitrigger Door/Button (1)/Button", 184},
            {"World/Region2/Sector 3/(R-3K) (Vault Door)/Save Button/Button", 185},
            {"World/Region2/Sector 3/Bottom Of The Well/Save Button/Button", 186},
            {"World/Region2/Lab/(R11H)/Attack Button/Button", 187},
            {"World/Region2/Lab/(R10G)/Attack Button/Button", 188},
            {"World/Region2/Lab/(R10G)/Dash Attack Button/Button", 189},
            {"World/Region2/Lab/(R13i)>(R15G)/Tiles/Dash Attack Button/Button", 190},
            {"World/Region2/Lab/(R13i)>(R15G)/Tiles/Attack Button/Button", 191},
            {"World/Region2/Lab/(R13i)>(R15G)/Tiles/DashButton/Button", 192},
            {"World/Region2/Lab/(R18G) (Clarity Altar)/Save Button/Button", 193},
            {"World/Smile Shop/Refund Room!/Attack Button/Button", 194},
            {"World/Smile Shop/Smilemask Room/Tiles/Attack Button/Button", 195},
            {"World/Escape Sequence/Region 2/Button/Button", 196},
            {"WORLD/Chase Sequence/(R7A)/DashButton/Button", 197},
            {"WORLD/Chase Sequence/(R7A)/Button/Button", 198},
            {"WORLD/Chase Sequence/(R7-B)/Dash Attack Button/Button", 199},
            {"WORLD/Chase Sequence/(R11-B)/Dash Attack Button/Button", 200},
            {"WORLD/Chase Sequence/(R8-B)/Multitrigger Door/Button (1)/Button", 201},
            {"WORLD/THE VERY END/RUN/Tiles/Room/Button/Button", 202},
            {"Inbetweeen/start/Tiles (2)/Save Button (1)/Button", 203},
            {"Inbetweeen/start/Tiles (2)/Multitrigger Door (1)/Button (1)/Button", 204},
            {"Room1  _ _(Clone)/Attack Button/Button", 205},
            {"Room4  _ -(Clone)/Button/Button", 206},
            {"Room5  _ ^(Clone)/Button/Button", 207},
            {"Room10 _ -(Clone)/DashButton/Button", 208},
            {"Room13  _ _(Clone)/Dash Attack Button/Button", 209},
            {"Room19  _ _(Clone)/Dash Attack Button/Button", 210},
            {"Room28  _ _(Clone)/Parry Button/Button", 211},
            {"Room30 ^ ^(Clone)/Parry Button/Button", 212},
            {"Room39 ^ ^(Clone)/Button/Button", 213},
            {"Room40 _ -(Clone)/Parry Button/Button", 214},
            {"Room42 - _(Clone)/Launcher/Button/Button", 215},
            {"Room42 - _(Clone)/Launcher (1)/Button/Button", 216},
            {"Room42 - _(Clone)/Launcher (2)/Button/Button", 217},
            {"Room42 - _(Clone)/Launcher (3)/Button/Button", 218},
            {"Room42 - _(Clone)/Launcher (4)/Button/Button", 219},
            {"Room42 - _(Clone)/Launcher (5)/Button/Button", 220},
            {"Room42 - _(Clone)/Launcher (6)/Button/Button", 221},
            {"Room43  - ^(Clone)/DashButton/Button", 222},
            {"Room43  - ^(Clone)/Parry Button/Button", 223},
            {"Room57 ^ ^(Clone)/Parry Button/Button", 224},
            {"Room66 ^ ^(Clone)/DashButton/Button", 225},
            {"Room71  ^ -(Clone)/Button/Button", 226},
            {"^ _(Clone)/Button/Button", 227},
        };

        private static readonly List<string> offendingPaths = new List<string>() {
            "World/Region2/Sector 2/(R6-E)<(R9-E) /Attack Button/Button",
            "World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/Dash Attack Button/Button",
            "World/Region2/Sector 2/(R11-E)>(R20-E)  (Shadow Rush)/SMILE X ROOM/FAKE/Button",
            "World/Region2/Sector 4/(R5D) > (R5G)/Button/Button",
            "World/Region2/(R10A) (Boss2)/Save Button/Button",
            "World/Region2/Sector 3/(R0H)/Parry Button (1)/Button",
            "World/Region2/Sector 3/(R0H)/Parry Button/Button",
        };

        private static readonly Dictionary<Vector3, int> fallbackCoords = new Dictionary<Vector3, int>() {
            {new Vector3(466f, 87.301f, 10f), 41},
            {new Vector3(491.5f, 73.949f, 10f), 44},
            {new Vector3(500.949f, 78f, 10f), 47},
            {new Vector3(797f, 73.949f, 0f), 56},
            {new Vector3(598.426f, 80.563f, 0f), 57},
            {new Vector3(618.5f, 52.699f, 0f), 64},
            {new Vector3(646.801f, 56f, 0f), 65},
            {new Vector3(606f, 43.449f, 0f), 67},
            {new Vector3(610f, 43.449f, 0f), 68},
            {new Vector3(612.75f, 43.449f, 0f), 69},
            {new Vector3(615.5f, 43.449f, 0f), 70},
            {new Vector3(406.301f, -49.5f, 10f), 79},
            {new Vector3(411.801f, -58.5f, 10f), 80},
            {new Vector3(400.5f, -80.199f, 10f), 81},
            {new Vector3(386.449f, -99.5f, 10f), 82},
            {new Vector3(548.5f, -2.78f, 0.3553f), 86},
            {new Vector3(609f, -68.03f, 0.3553f), 91},
            {new Vector3(276.75f, -105.199f, 10f), 173},
            {new Vector3(281.75f, -105.199f, 10f), 174},
            {new Vector3(286.75f, -105.199f, 10f), 175},
            {new Vector3(291.75f, -105.199f, 10f), 176},
        };

        private static readonly List<ApButton> loadedButtons = new List<ApButton>();
        private static readonly Dictionary<int, ButtonColor> colorKey = new Dictionary<int, ButtonColor>();
        private static readonly List<int> brokenButtonIds = new List<int>();
        public static NetworkClient client;
        private static InventoryManager inventory;
        private static bool randomizeColors = false;
        private static bool awaitingSlotData = true;
        public enum ButtonColor {
            SAVE = -1,
            RED = 0,
            BLUE = 1,
            GREEN = 2,
            YELLOW = 3,
            PINK = 4,
            BLACK = 5,
        }
    }
}