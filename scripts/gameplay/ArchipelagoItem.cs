using System.Collections.Generic;
using Archipelago.MultiClient.Net.Models;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace ApGlyphs {
    public class ArchipelagoItem : MonoBehaviour {
        public void Start() {
            player = SceneSearcher.Find("Player")?.GetComponent<PlayerController>();
            col = GetComponent<BoxCollider2D>();
            if (!col) col = gameObject.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            client = SceneSearcher.Find("Manager intro")?.GetComponent<ClientWrapper>();
            inventory = SceneSearcher.Find("Manager intro")?.GetComponent<InventoryManager>();
            itemCache = client.client.itemCache;
            if (alertJohn) john = SceneSearcher.Find("Clarity Figure")?.GetComponent<ClarityFigure>();
        }

        public void Update() {
            if (locId == -1) { Destroy(gameObject); return; }  // AP items must have a location id defined on creation
            if (itemInfo == null) FetchItemInfo();
            if (itemInfo == null) return;
            if (transform.parent.name == "Heal") transform.parent.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 0); // for boss rush checks
            if (isUsingConstructedModel) return;
            if (!sr) sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = GetItemSprite();
            if (!sr.sprite)
                if (itemInfo.Player.Slot != client.client.SlotId)
                    CreateAPLogo();
                else {
                    switch (itemInfo.ItemName) {
                        case "Grapple":
                            GameObject grapple = Object.Instantiate(Resources.Load<GameObject>("prefabs/game/Grapple Worm"), transform);
                            Destroy(grapple.GetComponent<Pickup>());
                            break;
                        case "Rune Cube":
                            GameObject cube = Object.Instantiate(Resources.Load<GameObject>("prefabs/game/Cube"), transform);
                            cube.transform.localPosition = Vector3.zero;
                            Destroy(cube.GetComponent<Pickup>());
                            break;
                        default:
                            CreateAPLogo();
                            break;
                    }
                    isUsingConstructedModel = true;
                }
            else {
                for (int i = 0; i < transform.childCount; i++) {
                    Transform child = transform.GetChild(i);
                    if (child.name.StartsWith("Orb_"))
                        child.gameObject.SetActive(false);
                }
            }
        }

        public void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.name != "Player") return;
            Collect();
        }

        public void OnDestroy() {
            if (transform.parent.name == "Heal") transform.parent.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255); // for boss rush checks
        }

        public void Collect() {
            client.client.CollectItem(this);
            if (itemInfo.Player.Slot == client.client.SlotId) {
                inventory.CollectAndSaveLocalInventory(new List<string> { itemInfo.ItemName });
                if (alertJohn && john && john.isActiveAndEnabled)
                    AbilityManager.UpdatePlayer(false);
                else
                    AbilityManager.UpdatePlayer(true);
            }
            if (alertJohn && john && john.isActiveAndEnabled) {
                john.PlayerSighted();
                player.hp = player.maxHp;
            }
            MelonLogger.Msg($"{client.client.SlotName} sent {itemInfo.ItemName} to {itemInfo.Player.Name} ({itemInfo.ItemGame})");
            Destroy(gameObject);
        }

        protected void FetchItemInfo() {
            itemInfo = itemCache.TryGetItem(locId, out var info) ? info : null;
            if (itemInfo != null) isUsingConstructedModel = false;    // this item has data now so we need to check it again to see if we still need the AP logo
            if (itemInfo != null && itemCache.checkedLocations.Contains(itemInfo.LocationId))
                Destroy(gameObject);
        }

        protected Sprite GetItemSprite() {
            if (itemInfo.Player.Slot != client.client.SlotId) return null;
            switch (itemInfo.ItemName) {
                case "Progressive Sword":
                    if (!player.hasWeapon) return Resources.Load<Sprite>("sprites/items/SwordFull");
                    return Resources.Load<Sprite>("sprites/items/Sword Upgrade");
                case "Progressive Dash Orb":
                    if (player.midairJumpsMax == 0) return Resources.Load<Sprite>("sprites/items/dashorb/DashOrb");
                    if (!player.dashAttack) return Resources.Load<Sprite>("sprites/items/DashAttack");
                    return Resources.Load<Sprite>("sprites/items/DashAttackUpgrade");
                case "Map":
                    return Resources.Load<Sprite>("sprites/items/Map");
                // constructed model bundled in prefabs
                // case "Grapple":
                //     return Resources.Load<Sprite>("sprites/items/Grapple");
                case "Progressive Parry":
                    if (!player.hasParry) return Resources.Load<Sprite>("sprites/items/parry/Parry");
                    return Resources.Load<Sprite>("sprites/items/Parry Upgrade");
                case "Shroud":
                    return Resources.Load<Sprite>("sprites/items/Shroud");
                case "Progressive Essence of George":
                    return Resources.Load<Sprite>("sprites/default/hats/chicken/egg");
                case "Silver Shard":
                    return Resources.Load<Sprite>("sprites/items/Fragment");
                case "Gold Shard":
                    sr.color = new UnityEngine.Color32(255, 197, 0, 255);
                    return Resources.Load<Sprite>("sprites/items/Fragment");
                case "Smile Token":
                    return Resources.Load<Sprite>("sprites/default/smile coin");
                // constructed model bundled in prefabs
                // case: "Rune Cube":
                //    return Resources.Load<Sprite>("sprites/items/Rune Cube");
                case "Void Gate Shard":
                    return Resources.Load<Sprite>("sprites/items/GateFragment");
                case "Glyphstone":
                    return Resources.Load<Sprite>("sprites/depictions/glyphstone/GlyphStone 0");
                case "Seeds":
                    return Resources.Load<Sprite>("sprites/default/hats/chicken/seed");
                case "Pink Bow":
                    return Resources.Load<Sprite>("sprites/default/hats/Pink Bow");
                case "Propeller Hat":
                    return Resources.Load<Sprite>("sprites/default/hats/PropellerHat");
                case "Traffic Cone":
                    return Resources.Load<Sprite>("sprites/default/hats/ConeHat");
                case "John Hat":
                    return Resources.Load<Sprite>("sprites/default/hats/JohnHat");
                case "Top Hat":
                    return Resources.Load<Sprite>("sprites/default/hats/Top Hat");
                case "Fez":
                    return Resources.Load<Sprite>("sprites/default/hats/Fez");
                case "Party Hat":
                    return Resources.Load<Sprite>("sprites/default/hats/PartyHat");
                case "Bomb Hat":
                    return Resources.Load<Sprite>("sprites/default/hats/bombHat");
                case "Progressive Chicken Hat":
                    if (!inventory.items.ContainsKey("Progressive Chicken Hat") || inventory.items["Progressive Chicken Hat"] == 0)
                        return Resources.Load<Sprite>("sprites/default/hats/chicken/chicken");
                    return Resources.Load<Sprite>("sprites/default/hats/chicken/chicken 1");
                case "Crown":
                    return Resources.Load<Sprite>("sprites/default/hats/crown");
                case "HP Refill":
                    return Resources.Load<Sprite>("sprites/items/Heal");
            }
            return null;
        }

        protected void CreateAPLogo() {
            const int orbCount = 6;
            const float radius = .333f;
            const float orbSize = .6f;
            Sprite orbSprite = CreateCircleSprite(64);
            List<Transform> transforms = new List<Transform>();
            for (int i = 0; i < orbCount; i++) {
                float angleRad = Mathf.Deg2Rad * (i * 360f / orbCount + 360f / (orbCount * 2f));
                GameObject orbObj = new GameObject($"Orb_{i}");
                orbObj.transform.SetParent(transform, false);
                Transform trans = orbObj.transform;
                transforms.Add(trans);
                trans.localPosition = new Vector2(
                    Mathf.Cos(angleRad) * radius,
                    Mathf.Sin(angleRad) * radius
                );
                trans.localScale = new Vector2(orbSize, orbSize);
                SpriteRenderer spriteRenderer = orbObj.AddComponent<SpriteRenderer>();
                int id = int.Parse(trans.name.Split('_')[1]);
                if (id == 0) spriteRenderer.color = new Color32(117, 194, 117, 255);
                if (id == 1) spriteRenderer.color = new Color32(201, 118, 130, 255);
                if (id == 2) spriteRenderer.color = new Color32(238, 227, 145, 255);
                if (id == 3) spriteRenderer.color = new Color32(118, 126, 189, 255);
                if (id == 4) spriteRenderer.color = new Color32(217, 160, 125, 255);
                if (id == 5) spriteRenderer.color = new Color32(202, 148, 194, 255);
                spriteRenderer.sprite = orbSprite;
            }
            transforms.Sort((a, b) =>
                b.localPosition.y.CompareTo(a.localPosition.y));
            int baseOrder = sr != null ? sr.sortingOrder : 0;
            for (int i = 0; i < transforms.Count; i++) {
                transforms[i].GetComponent<SpriteRenderer>().sortingLayerID = sr.sortingLayerID;
                transforms[i].GetComponent<SpriteRenderer>().sortingOrder = baseOrder + i + 1;
            }
            isUsingConstructedModel = true;
        }

        protected static Sprite CreateCircleSprite(int diameter) {
            Texture2D tex = new Texture2D(diameter, diameter, TextureFormat.ARGB32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            float r = diameter / 2f;
            Vector2 center = new Vector2(r, r);
            for (int y = 0; y < diameter; y++) {
                for (int x = 0; x < diameter; x++) {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    tex.SetPixel(x, y, dist <= r ? UnityEngine.Color.white : UnityEngine.Color.clear);
                }
            }
            tex.Apply();
            return Sprite.Create(
                tex,
                new Rect(0, 0, diameter, diameter),
                new Vector2(0.5f, 0.5f),
                100f
            );
        }

        protected PlayerController player;
        protected BoxCollider2D col;
        protected SpriteRenderer sr;
        protected ClientWrapper client;
        protected ItemCache itemCache;
        protected InventoryManager inventory;
        public long locId;
        public ScoutedItemInfo itemInfo;
        protected bool isUsingConstructedModel = false;
        public bool alertJohn;
        private ClarityFigure john;
    }
}