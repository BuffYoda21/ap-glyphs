using System.Collections.Generic;
using Il2Cpp;
using UnityEngine;

namespace ApGlyphs {
    public class ShopPurchaseTrigger : MonoBehaviour {
        public void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.name != "Player") return;
            foreach (ApShopItem item in items) {
                if (item.itemHeld) {
                    item.Purchase();
                    break;
                }
            }
        }

        public List<ApShopItem> items = new List<ApShopItem>();
    }
}