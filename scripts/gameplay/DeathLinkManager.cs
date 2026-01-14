using System;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using HarmonyLib;
using Il2Cpp;
using UnityEngine.SceneManagement;

namespace ApGlyphs {
    [HarmonyPatch]
    public static class DeathLinkManager {
        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPostfix]
        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.handle == lastSceneHandle) return;
            lastSceneHandle = scene.handle;
            if (scene.name != "Game" && scene.name != "Memory" && scene.name != "Outer Void") return;
            if (!player) player = SceneSearcher.Find("Player")?.GetComponent<PlayerController>();
        }

        public static void EnableDeathLink() {
            if (!client) client = SceneSearcher.Find("Manager intro")?.GetComponent<ClientWrapper>();
            if (!client) return;
            dl = client.client.session.CreateDeathLinkService();
            dl.OnDeathLinkReceived += (deathLinkObject) => {
                if (!player) return;
                lastDeathSceneHandle = lastSceneHandle; // prevents firing deathlink again when receiving deathlink
                player.hp = 0;
            };
            dl.EnableDeathLink();
        }

        public static void DisableDeathLink() {
            if (dl == null) return;
            dl.DisableDeathLink();
            dl = null;
        }

        [HarmonyPatch(typeof(SaveManager), "upCounter", new Type[] { typeof(string) })]
        [HarmonyPatch(typeof(SaveManager), "upCounter", new Type[] { typeof(string), typeof(bool) })]
        [HarmonyPrefix]
        public static void OnCounterUp(string id) {
            if (id != "TotalDeaths" || lastDeathSceneHandle == lastSceneHandle || dl == null) return;
            dl.SendDeathLink(new DeathLink(client.client.SlotName));
        }


        private static int lastSceneHandle = -1;
        private static int lastDeathSceneHandle = -1;
        private static ClientWrapper client;
        private static PlayerController player;
        private static DeathLinkService dl;
    }
}