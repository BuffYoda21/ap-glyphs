using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Archipelago.MultiClient.Net.Models;
using Il2Cpp;
using MelonLoader;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ApGlyphs {
    public class InventoryManager : MonoBehaviour {
        public void Start() {
            LoadInventoryFromFile();
        }

        public void Update() {
            if (scene.name != "Game" && scene.name != "Memory" && scene.name != "Outer Void") return;
            if (!player) player = SceneSearcher.Find("Player")?.GetComponent<PlayerController>();
            if (!player) return;
            if (!player.hasGeorge && items.Keys.Contains("Progressive Essence of George") && items["Progressive Essence of George"] > 0)
                player.hasGeorge = true;
        }

        public void ImportInventoryFromServer(ReadOnlyCollection<ItemInfo> importedItems) {
            Dictionary<string, int> loadedItems = new Dictionary<string, int>();
            foreach (ItemInfo item in importedItems) {
                if (loadedItems.ContainsKey(item.ItemName))
                    loadedItems[item.ItemName]++;
                else
                    loadedItems.Add(item.ItemName, 1);
            }
            if (loadedItems.Count == items.Count && loadedItems.All(kv => items.TryGetValue(kv.Key, out var v) && v == kv.Value))
                return;
            items = loadedItems;
            AbilityManager.UpdatePlayer();
            SaveInventoryToFile();
        }

        public bool LoadInventoryFromFile() {
            string inventoryPath = GetInventoryPath();

            try {
                string json = "{}";
                if (File.Exists(inventoryPath))
                    json = File.ReadAllText(inventoryPath);
                else {
                    MelonLogger.Warning("Found no localInventory.json. Creating new.");
                    SaveInventoryToFile();
                }
                Dictionary<string, int> loadedItems = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
                items = loadedItems;
                MelonLogger.Msg($"Loaded {items.Count} items from localInventory.json");
                inventoryLoaded = true;
                return true;
            } catch (Exception e) {
                MelonLogger.Error($"Failed to load inventory from localInventory.json: {e}");
                inventoryLoaded = true;
                return false;
            }
        }

        public bool SaveInventoryToFile() {
            string inventoryPath = GetInventoryPath();
            try {
                string json = JsonConvert.SerializeObject(
                    items,
                    Formatting.Indented
                );
                File.WriteAllText(inventoryPath, json);
                MelonLogger.Msg("Saved inventory to localInventory.json");
                return true;
            } catch (Exception e) {
                MelonLogger.Error($"Failed to save inventory: {e}");
                return false;
            }
        }

        public void CollectAndSaveLocalInventory(List<string> items) {
            foreach (string item in items) {
                if (this.items.ContainsKey(item))
                    this.items[item]++;
                else
                    this.items.Add(item, 1);
            }
            AbilityManager.UpdatePlayer();
            SaveInventoryToFile();
        }

        private string GetInventoryPath() {
            string userDataDir = Path.Combine(Environment.CurrentDirectory, "UserData");
            if (!Directory.Exists(userDataDir))
                Directory.CreateDirectory(userDataDir);
            string inventoryPath = Path.Combine(userDataDir, "localInventory.json");
            return inventoryPath;
        }

        public Dictionary<string, int> items = new Dictionary<string, int>(); // <name, count>
        public bool inventoryLoaded = false;
        private PlayerController player;
        public Scene scene;
    }
}







