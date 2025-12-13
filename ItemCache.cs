using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Models;
using UnityEngine;

namespace ApGlyphs {
    public class ItemCache {
        public async Task FetchItemPool(ArchipelagoSession session, IEnumerable<long> locationIds) {
            List<long> idList = new List<long>();
            foreach (long id in locationIds)
                idList.Add(id);
            Dictionary<long, ScoutedItemInfo> result = await session.Locations.ScoutLocationsAsync(idList.ToArray());
            foreach (KeyValuePair<long, ScoutedItemInfo> kv in result)
                itemPool[kv.Key] = kv.Value;
            ReadOnlyCollection<ItemInfo> receivedItems = new List<ItemInfo>(session.Items.AllItemsReceived).AsReadOnly();
            MainThreadDispatcher.Enqueue(() => {
                ApplyInventory(receivedItems);
                itemsReady = true;
            });
        }

        private void ApplyInventory(ReadOnlyCollection<ItemInfo> receivedItems) {
            if (!inventoryManager) {
                GameObject manager = GameObject.Find("Manager intro");
                if (manager)
                    inventoryManager = manager.GetComponent<InventoryManager>();
            }
            if (inventoryManager)
                inventoryManager.ImportInventoryFromServer(receivedItems);
        }

        public bool TryGetItem(long locationId, out ScoutedItemInfo info) {
            if (!itemsReady) { info = null; return false; }
            return itemPool.TryGetValue(locationId, out info);
        }

        private readonly Dictionary<long, ScoutedItemInfo> itemPool = new Dictionary<long, ScoutedItemInfo>();
        private InventoryManager inventoryManager;
        public MainThreadDispatcher dispatcher;
        public bool itemsReady;
    }

    public class MainThreadDispatcher : MonoBehaviour {
        public static void Enqueue(Action action) {
            lock (actions)
                actions.Enqueue(action);
        }

        public void Update() {
            lock (actions) {
                while (actions.Count > 0)
                    actions.Dequeue().Invoke();
            }
        }

        private static readonly Queue<Action> actions = new Queue<Action>();
    }
}