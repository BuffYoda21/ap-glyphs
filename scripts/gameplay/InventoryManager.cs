using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
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
            client = SceneSearcher.Find("Manager intro")?.GetComponent<ClientWrapper>();
        }

        public void Update() {
            if (scene.name != "Game" && scene.name != "Memory" && scene.name != "Outer Void") return;
            if (!player) player = SceneSearcher.Find("Player")?.GetComponent<PlayerController>();
            if (!player) return;
            if (!player.hasGeorge && items.Keys.Contains("Progressive Essence of George") && items["Progressive Essence of George"] > 0)
                player.hasGeorge = true;
        }

        public void ImportInventoryFromServer(ReadOnlyCollection<ItemInfo> importedItems) {
            if (importedItems.Count <= lastNotifiedItemIndex) lastNotifiedItemIndex = importedItems.Count - 1;

            bool hasNewNotifications = false;
            for (int i = lastNotifiedItemIndex + 1; i < importedItems.Count; i++) {
                ItemInfo itemInfo = importedItems[i];
                hasNewNotifications = true;

                if (itemInfo.Player.Slot != client.client.SlotId) {
                    string notifMsg = $"Received {itemInfo.ItemName} from {itemInfo.Player.Name}";
                    UnityEngine.Color notifColor;

                    if (itemInfo.Flags.HasFlag(ItemFlags.Advancement)) notifColor = new Color32(255, 0, 163, 255); // clarity pink
                    else if (itemInfo.Flags.HasFlag(ItemFlags.Trap)) notifColor = UnityEngine.Color.red;
                    else notifColor = UnityEngine.Color.white;

                    NotificationManager.Notify(notifMsg, notifColor);
                }

                if (itemInfo.ItemName.EndsWith("Trap")) { // itemInfo.Flags.HasFlag(ItemFlags.Trap) doesnt work for !getitem since that sends without flags
                    TrapManager.SpawnTrap(itemInfo.ItemName);
                }
            }

            if (hasNewNotifications) {
                lastNotifiedItemIndex = importedItems.Count - 1;
            }

            Dictionary<string, int> loadedItems = new Dictionary<string, int>();
            foreach (ItemInfo item in importedItems) {
                if (loadedItems.ContainsKey(item.ItemName))
                    loadedItems[item.ItemName]++;
                else
                    loadedItems.Add(item.ItemName, 1);
            }

            bool inventoryChanged = !(loadedItems.Count == items.Count && loadedItems.All(kv => items.TryGetValue(kv.Key, out var v) && v == kv.Value));

            if (!inventoryChanged && !hasNewNotifications) return;
            if (inventoryChanged) {
                items = loadedItems;
                AbilityManager.UpdatePlayer();
                SaveInventoryToFile();
            }
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

                InventorySaveData saveData = JsonConvert.DeserializeObject<InventorySaveData>(json);
                if (saveData == null || saveData.items == null) {
                    items = JsonConvert.DeserializeObject<Dictionary<string, int>>(json) ?? new Dictionary<string, int>();
                    lastNotifiedItemIndex = -1;
                    MelonLogger.Warning("Loaded old inventory format, converting to new format");
                } else {
                    items = saveData.items;
                    lastNotifiedItemIndex = saveData.lastNotifiedItemIndex;
                }

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
                InventorySaveData saveData = new InventorySaveData {
                    items = items,
                    lastNotifiedItemIndex = lastNotifiedItemIndex
                };

                string json = JsonConvert.SerializeObject(
                    saveData,
                    Formatting.Indented
                );
                File.WriteAllText(inventoryPath, json);
                MelonLogger.Msg($"Saved inventory to localInventory.json");
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
        private int lastNotifiedItemIndex = -1;
        private ClientWrapper client;

        [Serializable]
        private class InventorySaveData {
            public Dictionary<string, int> items = new Dictionary<string, int>();
            public int lastNotifiedItemIndex = -1;
        }
    }
}