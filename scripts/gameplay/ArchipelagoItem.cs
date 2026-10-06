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
            inventory = SceneSearcher.Find("Manager intro")?.GetComponent<InventoryManager>();
            itemCache = NetworkClient.itemCache;
            if (alertJohn) john = SceneSearcher.Find("Clarity Figure")?.GetComponent<ClarityFigure>();
        }

        public void Update() {
            if (locId == -1) { Destroy(gameObject); return; }  // AP items must have a location id defined on creation
            if (itemInfo == null) FetchItemInfo();
            if (itemInfo == null) return;
            if (transform.parent.name == "Heal") transform.parent.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 0); // for boss rush checks
            if (fallenBackToAPLogo) RecoverSprite();
            if (isUsingConstructedModel || sr) return;
            sr = gameObject.GetComponent<SpriteRenderer>();
            if (!sr) sr = gameObject.AddComponent<SpriteRenderer>();
            string spriteName = itemInfo.ItemName;
            if (spriteName.StartsWith("Progressive")) {
                if (inventory.items.ContainsKey(itemInfo.ItemName))
                    spriteName += "_" + (inventory.items[itemInfo.ItemName] + 1);
                else
                    spriteName += "_1";
            }
            if (itemInfo.Player.Slot == NetworkClient.ConnectionInfo.SlotId) SpriteCache.ApplySprite(spriteName, sr);
            if (!sr.sprite)
                if (itemInfo.Player.Slot != NetworkClient.ConnectionInfo.SlotId)
                    CreateAPLogo();
                else {
                    switch (itemInfo.ItemName) {
                        case "Grapple":
                            GameObject grapple = Instantiate(Resources.Load<GameObject>("prefabs/game/Grapple Worm"), transform);
                            Destroy(grapple.GetComponent<Pickup>());
                            break;
                        case "Rune Cube":
                            GameObject cube = Instantiate(Resources.Load<GameObject>("prefabs/game/Cube"), transform);
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
            if (transform.parent.name == "Heal")
                transform.parent.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255); // for boss rush checks
        }

        public void Collect() {
            NetworkClient.CollectItem(this);
            if (itemInfo.Player.Slot == NetworkClient.ConnectionInfo.SlotId) {
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
            MelonLogger.Msg($"{NetworkClient.ConnectionInfo.SlotName} sent {itemInfo.ItemName} to {itemInfo.Player.Name} ({itemInfo.ItemGame})");
            Destroy(gameObject);
        }

        protected void FetchItemInfo() {
            itemInfo = itemCache.TryGetItem(locId, out var info) ? info : null;
            if (itemInfo != null) isUsingConstructedModel = false;    // this item has data now so we need to check it again to see if we still need the AP logo
            if (itemInfo != null && itemCache.checkedLocations.Contains(itemInfo.LocationId))
                Destroy(gameObject);
        }

        // I don't think this is used anymore. Was an attempt to fix a bug that didn't work iirc but keeping it around for now.
        protected void RecoverSprite() {
            if (!fallenBackToAPLogo) return;
            fallenBackToAPLogo = false;
            if (itemInfo == null || itemInfo.Player.Slot != NetworkClient.ConnectionInfo.SlotId) return;

            MelonLogger.Warning($"DEBUG: Sprite Recovery processing for Location: {locId}");

            sr = gameObject.GetComponent<SpriteRenderer>();
            if (!sr) sr = gameObject.AddComponent<SpriteRenderer>();
            string spriteName = itemInfo.ItemName;
            if (spriteName.StartsWith("Progressive")) {
                if (inventory.items.ContainsKey(itemInfo.ItemName))
                    spriteName += "_" + (inventory.items[itemInfo.ItemName] + 1);
                else
                    spriteName += "_1";
            }
            if (itemInfo.Player.Slot == NetworkClient.ConnectionInfo.SlotId) SpriteCache.ApplySprite(spriteName, sr);

            if (!sr.sprite) {
                switch (itemInfo.ItemName) {
                    case "Grapple":
                        GameObject grapple = Instantiate(Resources.Load<GameObject>("prefabs/game/Grapple Worm"), transform);
                        Destroy(grapple.GetComponent<Pickup>());
                        break;
                    case "Rune Cube":
                        GameObject cube = Instantiate(Resources.Load<GameObject>("prefabs/game/Cube"), transform);
                        cube.transform.localPosition = Vector3.zero;
                        Destroy(cube.GetComponent<Pickup>());
                        break;
                    default:
                        fallenBackToAPLogo = true;
                        break;
                }
                isUsingConstructedModel = true;
            }
        }

        protected void CreateAPLogo() {
            if (itemInfo == null || itemInfo.Player.Slot == NetworkClient.ConnectionInfo.SlotId) fallenBackToAPLogo = true;
            const int orbCount = 6;
            const float radius = .333f;
            const float orbSize = .6f;
            if (!orbSprite) orbSprite = CreateCircleSprite(64);
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
        protected ItemCache itemCache;
        protected InventoryManager inventory;
        public long locId;
        public ScoutedItemInfo itemInfo;
        protected bool isUsingConstructedModel = false;
        protected bool fallenBackToAPLogo = false;
        public bool alertJohn = false;
        private ClarityFigure john;
        protected static Sprite orbSprite;
    }
}