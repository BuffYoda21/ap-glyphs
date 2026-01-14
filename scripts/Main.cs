using MelonLoader;
using Il2CppInterop.Runtime.Injection;
using System;
using UnityEngine;
using HarmonyLib;
using UnityEngine.SceneManagement;

[assembly: MelonInfo(typeof(ApGlyphs.Main), "ApGlyphs", "1.1.1", "BuffYoda21")]
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
            ClassInjector.RegisterTypeInIl2Cpp<ApShopItem>();
            ClassInjector.RegisterTypeInIl2Cpp<ApSmileMask>();
            ClassInjector.RegisterTypeInIl2Cpp<ArchipelagoItem>();
            ClassInjector.RegisterTypeInIl2Cpp<BetweenListener>();
            ClassInjector.RegisterTypeInIl2Cpp<BombHatPickupReplacer>();
            ClassInjector.RegisterTypeInIl2Cpp<ClientWrapper>();
            ClassInjector.RegisterTypeInIl2Cpp<ClientWrapper.ConnectionIndicator>();
            ClassInjector.RegisterTypeInIl2Cpp<HatRoomManager>();
            ClassInjector.RegisterTypeInIl2Cpp<InventoryManager>();
            ClassInjector.RegisterTypeInIl2Cpp<GamestateManager>();
            ClassInjector.RegisterTypeInIl2Cpp<MainThreadDispatcher>();
            ClassInjector.RegisterTypeInIl2Cpp<NotificationManager.Notification>();
            ClassInjector.RegisterTypeInIl2Cpp<ReplaceOnEnable>();
            ClassInjector.RegisterTypeInIl2Cpp<ReplaceOnDestroy>();
            ClassInjector.RegisterTypeInIl2Cpp<SeedCounter>();
            ClassInjector.RegisterTypeInIl2Cpp<ShopCounter>();
            ClassInjector.RegisterTypeInIl2Cpp<ShopPurchaseTrigger>();
            ClassInjector.RegisterTypeInIl2Cpp<VoidGateManager>();
            ClassInjector.RegisterTypeInIl2Cpp<WizardTriggerManager>();

            NotificationManager.Init();

            isInitialized = true;
        }


#pragma warning disable IDE0060 // Remove unused parameter warning
        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPostfix]
        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.handle == lastSceneHandle) return;
            lastSceneHandle = scene.handle;
            if (scene.name != "Intro" || client) return;  // only run on Intro scene when NetworkClient is not initialized

            // create required class instances
            GameObject manager = SceneSearcher.Find("Manager intro")?.gameObject;
            client = manager?.AddComponent<ClientWrapper>();
            inventory = manager?.AddComponent<InventoryManager>();
            gamestate = manager?.AddComponent<GamestateManager>();
            betweenListener = manager?.AddComponent<BetweenListener>();
            itemCache = new ItemCache();
            itemCache.dispatcher = manager?.AddComponent<MainThreadDispatcher>();
            if (!client) MelonLogger.Error("Failed to create ClientWrapper");
            if (!inventory) MelonLogger.Error("Failed to create InventoryManager instance");
            if (itemCache == null) MelonLogger.Error("Failed to create ItemCache instance");
            if (!itemCache?.dispatcher) MelonLogger.Error("Failed to create MainThreadDispatcher instance");
            if (!client || !inventory || itemCache == null || !itemCache.dispatcher) return;

            client.SetItemCacheRef(itemCache);
        }
#pragma warning restore IDE0060 // Restore unused parameter warning


        public static ClientWrapper client;
        public static ItemCache itemCache;
        public static InventoryManager inventory;
        public static GamestateManager gamestate;
        public static BetweenListener betweenListener;
        private static int lastSceneHandle;
        private bool isInitialized = false;
    }
}