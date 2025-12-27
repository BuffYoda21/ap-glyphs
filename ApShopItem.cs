using System;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace ApGlyphs {
    public class ApShopItem : ArchipelagoItem {
        public new void Start() {
            base.Start();
            sr = GetComponent<SpriteRenderer>();
            if (!sr) sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = null;
            vanillaItem = GetComponent<ShopItem>();
        }

        public new void Update() {
            base.Update();
            if (!gamestate) gamestate = GameObject.Find("Manager intro")?.GetComponent<GamestateManager>();
            if (!vanillaItem) return;
            if (vanillaItem.startpos != transform.position)
                itemHeld = true;
            else
                itemHeld = false;
        }

        public new void OnTriggerEnter2D(Collider2D other) {
            return;
        }

        public void Purchase() {
            if (!vanillaItem || !gamestate) return;
            //MelonLogger.Msg($"Attempting to purchase {shopId} for {price} tokens. Have {inventory.items["Smile Token"] - gamestate.spentTokens} tokens.");
            if (inventory.items.ContainsKey("Smile Token") && inventory.items["Smile Token"] - gamestate.spentTokens >= price) {
                gamestate.SaveFlag($"purchased item {shopId}");
                gamestate.spentTokens += price;
                base.Collect();
                Destroy(gameObject);
            }
        }

        public bool itemHeld = false;
        public int price = 2;
        public int shopId = -1;
        private ShopItem vanillaItem;
        private GamestateManager gamestate;
    }
}