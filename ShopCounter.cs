using System.Collections.Generic;
using Il2Cpp;
using UnityEngine;

namespace ApGlyphs {
    // attach to smile shop parent
    public class ShopCounter : MonoBehaviour {
        public void Start() {
            inventory = GameObject.Find("Manager intro")?.GetComponent<InventoryManager>();
            gamestate = GameObject.Find("Manager intro")?.GetComponent<GamestateManager>();
            counters.Add(transform.Find("Counter")?.GetComponent<BuildText>());
            counters.Add(transform.Find("Refund Room!/Counter")?.GetComponent<BuildText>());
            counters.Add(transform.Find("Hat room/Counter")?.GetComponent<BuildText>());
            counters.Add(transform.Find("Smilemask Room/Counter")?.GetComponent<BuildText>());
        }

        public void Update() {
            if (!inventory || !gamestate || gamestate.spentTokens == -1) return;
            if (!inventory.items.ContainsKey("Smile Token")) unspentTokens = 0;
            else unspentTokens = inventory.items["Smile Token"] - gamestate.spentTokens;
            foreach (BuildText counter in counters) {
                if (counter == null) continue;
                if (counter.text == "" + unspentTokens || counter.text == "0" + unspentTokens) continue;
                for (int i = counter.transform.childCount - 1; i >= 0; i--) {
                    Destroy(counter.transform.GetChild(i).gameObject);
                }
                counter.text = "" + unspentTokens;
                if (counter.text.Length == 1) counter.text = "0" + counter.text;
                counter.x = 0f;
                counter.placed = false;
            }
        }

        private InventoryManager inventory;
        private GamestateManager gamestate;
        private List<BuildText> counters = new List<BuildText>();
        private int unspentTokens;
    }
}