using MelonLoader;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using HarmonyLib;
using UnityEngine.SceneManagement;

[assembly: MelonInfo(typeof(ApGlyphs.Main), "ApGlyphs", "1.4.0", "BuffYoda21")]
[assembly: MelonGame("Vortex Bros.", "GLYPHS")]

namespace ApGlyphs {
    [HarmonyPatch]
    public class Main : MelonMod {
        public override void OnInitializeMelon() {
            if (isInitialized) return;

            // class injection here
            ClassInjector.RegisterTypeInIl2Cpp<ApButton>();
            ClassInjector.RegisterTypeInIl2Cpp<ApShopItem>();
            ClassInjector.RegisterTypeInIl2Cpp<ApSmileMask>();
            ClassInjector.RegisterTypeInIl2Cpp<ArchipelagoItem>();
            ClassInjector.RegisterTypeInIl2Cpp<BetweenListener>();
            ClassInjector.RegisterTypeInIl2Cpp<BombHatPickupReplacer>();
            ClassInjector.RegisterTypeInIl2Cpp<HatRoomManager>();
            ClassInjector.RegisterTypeInIl2Cpp<MainThreadDispatcher>();
            ClassInjector.RegisterTypeInIl2Cpp<NetworkClient.ConnectionIndicator>();
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

        public override void OnUpdate() => NetworkClient.Update();

        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPostfix]
        public static void OnSceneLoaded(Scene scene) {
            if (scene.handle == lastSceneHandle) return;
            lastSceneHandle = scene.handle;
            if (scene.name != "Intro") return;  // only run on Intro scene

            // create required class instances
            GameObject manager = SceneSearcher.Find("Manager intro")?.gameObject;
            betweenListener = manager?.AddComponent<BetweenListener>();
            NetworkClient.itemCache.dispatcher = manager?.AddComponent<MainThreadDispatcher>();
        }

        public static BetweenListener betweenListener;
        private static int lastSceneHandle;
        private bool isInitialized = false;
    }
}