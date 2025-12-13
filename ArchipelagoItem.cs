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
            itemCache = client.client.itemCache;
        }

        public void Update() {
            if (locId == -1) { Destroy(gameObject); return; }  // AP items must have these parameteres defined on creation
            if (itemInfo == null) FetchItemInfo();
            if (!sr) sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = GetItemSprite();
            if (!sr.sprite) CreateAPLogo();
        }

        public void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.name != "Player") return;
            client.client.CollectItem(this);
            if (itemInfo.ItemGame == "GLYPHS" && itemInfo.ItemName.EndsWith("Dash Orb")) {
                player.midairJumpsMax++;
            }
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
        public long locId;
        private bool ranStart = false;
        public ScoutedItemInfo itemInfo;
    }
}