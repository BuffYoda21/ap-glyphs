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
            if (itemInfo == null) return;
            if (!sr) sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = GetItemSprite();
            if (!sr.sprite && !isUsingAPlogo) CreateAPLogo();
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
            if (itemInfo != null && itemCache.checkedLocations.Contains(itemInfo.LocationId))
                Destroy(gameObject);
        }

        private Sprite GetItemSprite() {
            if (itemInfo.ItemGame != "GLYPHS") return null;
            if (itemInfo.ItemName.EndsWith("Dash Orb")) return Resources.Load<Sprite>("sprites/items/dashorb/DashOrb");
            return null;
        }

        private void CreateAPLogo() {
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
            isUsingAPlogo = true;
        }

        private static Sprite CreateCircleSprite(int diameter) {
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

        private PlayerController player;
        private BoxCollider2D col;
        private SpriteRenderer sr;
        private ClientWrapper client;
        private ItemCache itemCache;
        private InventoryManager inventory;
        public long locId;
        public ScoutedItemInfo itemInfo;
        private bool isUsingAPlogo = false;
    }
}