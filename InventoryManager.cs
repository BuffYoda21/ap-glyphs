using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Archipelago.MultiClient.Net.Models;
using MelonLoader;
using Newtonsoft.Json;
using UnityEngine;

namespace ApGlyphs {
    public class InventoryManager : MonoBehaviour {
        public void Start() {
            LoadInventoryFromFile();
        }

        public void ImportInventoryFromServer(ReadOnlyCollection<ItemInfo> importedItems) {
            Dictionary<string, int> loadedItems = new Dictionary<string, int>();
            foreach (ItemInfo item in importedItems) {
                if (loadedItems.ContainsKey(item.ItemName))
                    loadedItems[item.ItemName]++;
                else
                    loadedItems.Add(item.ItemName, 1);
            }
            items = loadedItems;
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
    }
}







