using System.Collections.Generic;
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

            if (name == null || !sprites.ContainsKey(name)) return;
            sr.sprite = sprites[name];
            if (name == "Gold Shard") sr.color = new Color32(255, 197, 0, 255);
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