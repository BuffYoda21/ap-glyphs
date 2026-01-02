using System.Collections.Generic;
using System.Text;
using Archipelago.MultiClient.Net.Enums;
using Il2Cpp;
using UnityEngine;

namespace ApGlyphs {
    public class ApShopItem : ArchipelagoItem {
        public new void Start() {
            base.Start();
            sr = GetComponent<SpriteRenderer>();
            if (!sr) sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = null;
            vanillaItem = GetComponent<ShopItem>();
            displayText = transform.Find("nametext").GetComponent<BuildText>();
            displayText.text = "";
            displayText.normaltext = true;
            reactParent = SceneSearcher.Find("World/Smile Shop/reacttext")?.gameObject;
            if (!reactParent) return;
            for (int i = 0; i < reactParent.transform.childCount; i++) {
                Transform child = reactParent.transform.GetChild(i);
                otherReactions.Add(child.gameObject);
            }
            apItemReaction = GameObject.Instantiate(reactParent.transform.Find("reaction (9)")?.gameObject, reactParent.transform)?.GetComponent<BuildText>();
            if (apItemReaction) apItemReaction.text = "";
        }

        public new void Update() {
            base.Update();
            if (!gamestate) gamestate = SceneSearcher.Find("Manager intro")?.GetComponent<GamestateManager>();
            if (!vanillaItem) return;
            if (vanillaItem.startpos != transform.position) {
                if (!apItemReaction.gameObject.activeSelf) apItemReaction.gameObject.SetActive(true);
                foreach (GameObject otherReaction in otherReactions) {
                    if (otherReaction.activeSelf) otherReaction.SetActive(false);
                }
                itemHeld = true;
            } else {
                if (apItemReaction.gameObject.activeSelf) apItemReaction.gameObject.SetActive(false);
                itemHeld = false;
            }
            if (itemInfo == null || displayText.text != "") return;
            if (itemInfo.Player.Slot != client.client.SlotId)
                displayText.text = itemInfo.Player.Name + "s ";
            displayText.text += itemInfo.ItemName;
            displayText.text = NormalizeText(displayText.text);
            if (apItemReaction.text != "") return;
            List<string> possibleReactions = new List<string>();
            if (itemInfo.Player.Slot == client.client.SlotId) {
                //reactions for vanilla items
                switch (itemInfo.ItemName) {
                    case "Progressive Sword":
                        possibleReactions.Add("it is dangerous to go alone take this");
                        break;
                    case "Progressive Dash Orb":
                        possibleReactions.Add("seems important");
                        break;
                    case "Map":
                        possibleReactions.Add("it will let you know where you are going");
                        break;
                    case "Grapple":
                        possibleReactions.Add("i promise i didn't kidnap him");
                        break;
                    case "Progressive Parry":
                        possibleReactions.Add("good luck lol");
                        break;
                    case "Shroud":
                        possibleReactions.Add("not a minecraft totem of undying");
                        break;
                    case "Progressive Essence of George":
                        possibleReactions.Add("how did i get this? idk");
                        break;
                    case "Silver Shard":
                        possibleReactions.Add("i hear that it will make you stronger");
                        break;
                    case "Gold Shard":
                        possibleReactions.Add("that seems like a nice thing to have");
                        break;
                    case "Smile Token":
                        possibleReactions.Add("it is not a scam i swear");
                        break;
                    case "Rune Cube":
                        possibleReactions.Add("you spin me right round baby");
                        break;
                    case "Void Gate Shard":
                        possibleReactions.Add("john sent this");
                        break;
                    case "Glyphstone":
                        possibleReactions.Add("use its power wisely");
                        break;
                    case "Seeds":
                        possibleReactions.Add("george certified");
                        break;
                    case "Pink Bow":
                        possibleReactions.Add("pretty pink");
                        break;
                    case "Propeller Hat":
                        possibleReactions.Add("that will not let you fly");
                        break;
                    case "Traffic Cone":
                        possibleReactions.Add("why do you want that?");
                        break;
                    case "John Hat":
                        possibleReactions.Add("mini john mini john");
                        break;
                    case "Top Hat":
                        possibleReactions.Add("it will look great on you");
                        break;
                    case "Fez":
                        possibleReactions.Add("i like fez");
                        break;
                    case "Party Hat":
                        possibleReactions.Add("just make sure to clean up after yourself");
                        break;
                    case "Crown":
                        possibleReactions.Add("oooh fancy");
                        break;
                    case "Bomb Hat":
                        possibleReactions.Add("just don't wear it in here");
                        break;
                    case "Progressive Chicken Hat":
                        possibleReactions.Add("hi george");
                        break;
                    case "HP Refill":
                        possibleReactions.Add("worth it");
                        break;
                }
            } else {
                if (itemInfo.ItemGame == "GLYPHS") {
                    // special reactions if ap item is from another glyphs world
                    possibleReactions.Add("an item from another timeline?");
                    possibleReactions.Add("seems oddly familiar");
                    possibleReactions.Add("i think i recognize that one");
                    if (itemInfo.ItemName == "Smile Token") possibleReactions.Add("pay it forward");
                } else {
                    // special reactions if ap item is not from another glyphs world
                    possibleReactions.Add($"whats a {itemInfo.ItemName}?");
                    possibleReactions.Add($"never heard of {itemInfo.ItemGame}");
                    possibleReactions.Add($"how did that get here?");
                }
                // reactions for ap items
                possibleReactions.Add($"whos {itemInfo.Player.Name}?");
                possibleReactions.Add("what is that?");
                possibleReactions.Add("an item from another world? strange");
                possibleReactions.Add("how do you even plan on getting it to them?");
                possibleReactions.Add("i love archipelago");
                if (itemInfo.Flags.HasFlag(ItemFlags.Advancement)) {
                    // special reactions for advancement ap items
                    possibleReactions.Add("that seems important");
                    possibleReactions.Add($"{itemInfo.Player.Name} will surely want that");
                }
            }
            apItemReaction.text = possibleReactions[UnityEngine.Random.Range(0, possibleReactions.Count)];
            apItemReaction.text = NormalizeText(apItemReaction.text);
            apItemReaction.transform.localPosition = new Vector3(13.2f - apItemReaction.text.Length * 0.4f, apItemReaction.transform.localPosition.y, apItemReaction.transform.localPosition.z);
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

        private string NormalizeText(string origText) {
            if (origText == null) return null;
            StringBuilder sb = new StringBuilder(origText.Length);
            foreach (char c in origText.ToLowerInvariant()) {
                if (c == '_') sb.Append(' ');
                else if (c == '?') sb.Append(')'); // for whatever reason, BuildText recognizes ) as a question mark and doesn't like ?s
                else if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)) sb.Append(c);
            }
            return sb.ToString();
        }

        public bool itemHeld = false;
        public int price = 2;
        public int shopId = -1;
        private ShopItem vanillaItem;
        private GamestateManager gamestate;
        private BuildText displayText;
        private GameObject reactParent;
        private BuildText apItemReaction;
        private List<GameObject> otherReactions = new List<GameObject>();
    }
}