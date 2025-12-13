using MelonLoader;
using Il2CppInterop.Runtime.Injection;
using System;
using UnityEngine;
using HarmonyLib;
using UnityEngine.SceneManagement;

[assembly: MelonInfo(typeof(ApGlyphs.Main), "ApGlyphs", "1.0.0", "BuffYoda21")]
[assembly: MelonGame("Vortex Bros.", "GLYPHS")]

namespace ApGlyphs {
    [HarmonyPatch]
    public class Main : MelonMod {
        [Obsolete]
        public override void OnApplicationStart() {
            if (isInitialized) return;
            var harmony = new HarmonyLib.Harmony("ApGlyphs.Patches");
            harmony.PatchAll();

            // class injection here
            ClassInjector.RegisterTypeInIl2Cpp<ArchipelagoItem>();
            ClassInjector.RegisterTypeInIl2Cpp<ClientWrapper>();
            ClassInjector.RegisterTypeInIl2Cpp<ClientWrapper.ConnectionIndicator>();

            isInitialized = true;
        }


#pragma warning disable IDE0060 // Remove unused parameter warning
        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPostfix]
        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.handle == lastSceneHandle) return;
            lastSceneHandle = scene.handle;
            if (scene.name != "Intro" || client) return;  // only run on Intro scene when NetworkClient is not initialized

            // create NetworkClient and ItemCache instances
            GameObject manager = GameObject.Find("Manager intro");
            client = manager?.AddComponent<ClientWrapper>();
            itemCache = new ItemCache();
            if (!client)
                MelonLogger.Error("Failed to create NetworkClient instance");
            if (itemCache == null)
                MelonLogger.Error("Failed to create ItemCache instance");
            if (!client || itemCache == null) return;

            client.SetItemCacheRef(itemCache);
        }
#pragma warning restore IDE0060 // Restore unused parameter warning


        public static ClientWrapper client;
        public static ItemCache itemCache;
        private static int lastSceneHandle;
        private bool isInitialized = false;
    }
}