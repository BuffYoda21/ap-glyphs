using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ApGlyphs {
    public class GamestateManager : MonoBehaviour {
        public void Start() {
            if (!client) client = SceneSearcher.Find("Manager intro")?.GetComponent<ClientWrapper>();
            if (itemCache == null) itemCache = itemCache = client.client.itemCache;
            LoadGamestateFromFile();
            if (!stateLoaded || itemCache == null) return;
            foreach (string flag in flags) {
                if (flag.StartsWith("purchased item ")) {
                    string[] split = flag.Split(' ');
                    purchasedItemIds.Add(int.Parse(split[2]));
                }
            }
        }

        public void Update() {
            if (itemCache == null || !itemCache.itemsReady || spentTokens != -1) return;
            spentTokens = 0;
            foreach (int itemId in purchasedItemIds) {
                if (itemCache.checkedLocations.Contains(itemId + 59)) {
                    switch (itemId) {
                        case 1:
                            spentTokens += Convert.ToInt32((client.client.slotData["shop_prices"] as JArray)[itemId - 1]);
                            break;
                        case 2:
                            spentTokens += Convert.ToInt32((client.client.slotData["shop_prices"] as JArray)[itemId - 1]);
                            break;
                        case 3:
                            spentTokens += Convert.ToInt32((client.client.slotData["shop_prices"] as JArray)[itemId - 1]);
                            break;
                        case 4:
                            spentTokens += Convert.ToInt32((client.client.slotData["shop_prices"] as JArray)[itemId - 1]);
                            break;
                    }
                }
            }
        }

        private void FetchGoal() {
            if (!client) client = SceneSearcher.Find("Manager intro")?.GetComponent<ClientWrapper>();
            if (!client) return;
            try {
                switch ((int)client.client.options["Goal"]) {
                    case (int)Goal.FalseEnding: goal = Goal.FalseEnding; break;
                    case (int)Goal.GoodEnding: goal = Goal.GoodEnding; break;
                    case (int)Goal.TrueEnding: goal = Goal.TrueEnding; break;
                    case (int)Goal.AllStarEndings: goal = Goal.AllStarEndings; break;
                    case (int)Goal.Epilogue: goal = Goal.Epilogue; break;
                    case (int)Goal.AllEndings: goal = Goal.AllEndings; break;
                    default:
                        MelonLogger.Error("Failed to parse " + client.client.options["Goal"]);
                        break;
                }
            } catch (Exception ex) {
                MelonLogger.Error("Failed to get Goal: " + ex.Message);
            }
        }

        public bool LoadGamestateFromFile() {
            string statePath = GetStatePath();

            try {
                if (!File.Exists(statePath)) {
                    MelonLogger.Warning("Found no Gamestate.json. Creating new.");
                    SaveStateToFile();
                }

                string json = File.Exists(statePath) ? File.ReadAllText(statePath) : "[]";
                List<string> loadedFlags = JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
                flags = loadedFlags;
                MelonLogger.Msg($"Loaded {flags.Count} flags from Gamestate.json");
                stateLoaded = true;
                return true;
            } catch (Exception e) {
                MelonLogger.Error($"Failed to load flags from Gamestate.json: {e}");
                stateLoaded = true;
                return false;
            }
        }

        public bool SaveStateToFile() {
            string statePath = GetStatePath();
            try {
                string json = JsonConvert.SerializeObject(
                    flags,
                    Formatting.Indented
                );
                File.WriteAllText(statePath, json);
                MelonLogger.Msg("Saved flags to Gamestate.json");
                return true;
            } catch (Exception e) {
                MelonLogger.Error($"Failed to save gamestate: {e}");
                return false;
            }
        }

        public void SaveFlag(string flag) {
            if (flags.Contains(flag)) return;
            flags.Add(flag);
            CheckForGameCompletion();
            SaveStateToFile();
        }

        public void CheckForGameCompletion() {
            if (goal == Goal.None) FetchGoal();
            if (goal == Goal.None) return;
            switch (goal) {
                case Goal.FalseEnding:
                    if (flags.Contains("FalseEnding"))
                        client.client.ClearGoal();
                    break;
                case Goal.GoodEnding:
                    if (flags.Contains("GoodEnding"))
                        client.client.ClearGoal();
                    break;
                case Goal.TrueEnding:
                    if (flags.Contains("TrueEnding"))
                        client.client.ClearGoal();
                    break;
                case Goal.AllStarEndings:
                    if (flags.Contains("SmilemaskEnding") && flags.Contains("PerfectClarity") && flags.Contains("OmnipotenceEnding"))
                        client.client.ClearGoal();
                    break;
                case Goal.Epilogue:
                    if (flags.Contains("EpilogueEnding"))
                        client.client.ClearGoal();
                    break;
                case Goal.AllEndings:
                    if (flags.Contains("FalseEnding") && flags.Contains("GoodEnding") && flags.Contains("TrueEnding") && flags.Contains("SmilemaskEnding") && flags.Contains("PerfectClarity") && flags.Contains("OmnipotenceEnding") && flags.Contains("EpilogueEnding"))
                        client.client.ClearGoal();
                    break;
            }
        }

        private string GetStatePath() {
            string userDataDir = Path.Combine(Environment.CurrentDirectory, "UserData");
            if (!Directory.Exists(userDataDir))
                Directory.CreateDirectory(userDataDir);
            string statePath = Path.Combine(userDataDir, "Gamestate.json");
            return statePath;
        }

        private List<string> flags = new List<string>();
        public bool stateLoaded = false;
        private ClientWrapper client;
        private Goal goal = Goal.None;
        private List<int> purchasedItemIds = new List<int>();
        public int spentTokens = -1;
        private ItemCache itemCache;
        private enum Goal : int {
            None = 0,
            FalseEnding = 1,
            GoodEnding = 2,
            TrueEnding = 3,
            AllStarEndings = 4,
            Epilogue = 5,
            AllEndings = 6
        }
    }
}