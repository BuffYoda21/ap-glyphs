using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ApGlyphs {
    [HarmonyPatch]
    public static class SpriteCache {
        public static void LoadSprites() {
            if (sprites != null && sprites.Count > 0) return;

            sprites = new Dictionary<string, Sprite>() {
                {"Progressive Sword_1", Resources.Load<Sprite>("sprites/items/Sword Full")},
                {"Progressive Sword_2", Resources.Load<Sprite>("sprites/items/Sword Upgrade")},
                {"Progressive Dash Orb_1", Resources.Load<Sprite>("sprites/items/dashorb/DashOrb")},
                {"Progressive Dash Orb_2", Resources.Load<Sprite>("sprites/items/DashAttack")},
                {"Progressive Dash Orb_3", Resources.Load<Sprite>("sprites/items/DashAttackUpgrade")},
                {"Map", Resources.Load<Sprite>("sprites/items/Map")},
                {"Grapple", null},
                {"Progressive Parry_1", Resources.Load<Sprite>("sprites/items/parry/Parry")},
                {"Progressive Parry_2", Resources.Load<Sprite>("sprites/items/Parry Upgrade")},
                {"Shroud", Resources.Load<Sprite>("sprites/items/Shroud")},
                {"Progressive Essence of George_1", Resources.Load<Sprite>("sprites/default/hats/chicken/egg")},
                {"Progressive Essence of George_2", Resources.Load<Sprite>("sprites/default/hats/chicken/egg")},
                {"Silver Shard", Resources.Load<Sprite>("sprites/items/Fragment")},
                {"Gold Shard", Resources.Load<Sprite>("sprites/items/Fragment")},
                {"Smile Token", Resources.Load<Sprite>("sprites/default/smile coin")},
                {"Rune Cube", null},
                {"Void Gate Shard", Resources.Load<Sprite>("sprites/items/GateFragment")},
                {"Glyphstone", Resources.Load<Sprite>("sprites/depictions/glyphstone/GlyphStone 0")},
                {"Seeds", Resources.Load<Sprite>("sprites/default/hats/chicken/seed")},
                {"Pink Bow", Resources.Load<Sprite>("sprites/default/hats/Pink Bow")},
                {"Propeller Hat", Resources.Load<Sprite>("sprites/default/hats/PropellerHat")},
                {"Traffic Cone", Resources.Load<Sprite>("sprites/default/hats/ConeHat")},
                {"John Hat", Resources.Load<Sprite>("sprites/default/hats/JohnHat")},
                {"Top Hat", Resources.Load<Sprite>("sprites/default/hats/Top Hat")},
                {"Fez", Resources.Load<Sprite>("sprites/default/hats/Fez")},
                {"Party Hat", Resources.Load<Sprite>("sprites/default/hats/PartyHat")},
                {"Bomb Hat", Resources.Load<Sprite>("sprites/default/hats/bombHat")},
                {"Progressive Chicken Hat_1", Resources.Load<Sprite>("sprites/default/hats/chicken/chicken")},
                {"Progressive Chicken Hat_2", Resources.Load<Sprite>("sprites/default/hats/chicken/chicken 1")},
                {"Crown", Resources.Load<Sprite>("sprites/default/hats/crown")},
                {"HP Refill", Resources.Load<Sprite>("sprites/items/Heal")},
            };

            MelonLogger.Msg("Loaded " + sprites.Count + " sprites");
        }

        public static Sprite GetSprite(string name) {
            if (sprites == null || sprites.Count == 0) LoadSprites();

            if (name != null && sprites.ContainsKey(name)) return sprites[name];
            return null;
        }

        public static void ApplySprite(string name, SpriteRenderer sr) {
            if (sprites == null || sprites.Count == 0) LoadSprites();

            if (name == null) return;
            if (name.EndsWith("Trap")) {
                List<Sprite> spriteList = sprites.Values.ToList();
                sr.sprite = spriteList[UnityEngine.Random.Range(0, spriteList.Count)];
                return;
            }
            if (!sprites.ContainsKey(name)) return;
            sr.sprite = sprites[name];

            switch (name) {
                case "Gold Shard": sr.color = new Color32(255, 197, 0, 255); break;
                case "Glyphstone":
                    switch (UnityEngine.Random.Range(0, 3)) {
                        case 0: sr.color = new Color(0f, 1f, 0.0929f, 1f); break;
                        case 1: sr.color = new Color(20f, 0.3467f, 1f, 1f); break;
                        case 2: sr.color = new Color(1f, 0f, 0.0861f, 1f); break;
                    }
                    break;
                case "HP Refill":
                    sr.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                    break;
                case "Smile Token":
                    sr.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                    if (sr.GetComponent<BoxCollider2D>())
                        sr.GetComponent<BoxCollider2D>().size = new Vector2(2f, 2f);
                    break;
                case "Propeller Hat":
                    sr.transform.localScale = new Vector3(10f, 10f, 1f);
                    if (sr.GetComponent<BoxCollider2D>())
                        sr.GetComponent<BoxCollider2D>().size = new Vector2(0.1f, 0.1f);
                    break;
                case "Void Gate Shard":
                    sr.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                    break;
            }
        }

        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPostfix]
        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.handle == lastSceneHandle) return;
            lastSceneHandle = scene.handle;
            sprites.Clear();
        }

        private static Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        private static int lastSceneHandle = -1;
    }
}