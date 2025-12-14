using System.Collections.Generic;
using Archipelago.MultiClient.Net.Models;
using Il2Cpp;
using UnityEngine;

namespace ApGlyphs {
    public class ArchipelagoItem : MonoBehaviour {
        public void Start() {
            player = GameObject.Find("Player")?.GetComponent<PlayerController>();
            col = gameObject.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            client = GameObject.Find("Manager intro")?.GetComponent<ClientWrapper>();
            inventory = GameObject.Find("Manager intro")?.GetComponent<InventoryManager>();
            itemCache = client.client.itemCache;
        }

        public void Update() {
            if (locId == -1) { Destroy(gameObject); return; }  // AP items must have a location id defined on creation
            if (itemInfo == null) FetchItemInfo();
            if (!sr) sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = GetItemSprite();
            if (!sr.sprite) CreateAPLogo();
        }

        public void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.name != "Player") return;
            client.client.CollectItem(this);
            if (itemInfo.ItemGame == "GLYPHS") {
                inventory.CollectAndSaveLocalInventory(new List<string> { itemInfo.ItemName });
            }
            Destroy(gameObject);
        }

        private void FetchItemInfo() {
            itemInfo = itemCache.TryGetItem(locId, out var info) ? info : null;
        }

        private Sprite GetItemSprite() {
            if (itemInfo.ItemGame != "GLYPHS") return null;
            if (itemInfo.ItemName.EndsWith("Dash Orb")) return Resources.Load<Sprite>("sprites/items/dashorb/DashOrb");
            return null;
        }

        private void CreateAPLogo() {

        }

        private PlayerController player;
        private BoxCollider2D col;
        private SpriteRenderer sr;
        private ClientWrapper client;
        private ItemCache itemCache;
        private InventoryManager inventory;
        public long locId;
        public ScoutedItemInfo itemInfo;
    }
}