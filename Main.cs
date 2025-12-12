using MelonLoader;
using Il2CppInterop.Runtime.Injection;
using System.IO;
using System;
using UnityEngine;
using HarmonyLib;
using UnityEngine.SceneManagement;

[assembly: MelonInfo(typeof(ApGlyphs.Main), "ApGlyphs", "1.0.0", "BuffYoda21")]
[assembly: MelonGame("Vortex Bros.", "GLYPHS")]

namespace ApGlyphs {
    [HarmonyPatch]
    public class Main : MelonMod {
        [System.Obsolete]
        public override void OnApplicationStart() {
            if (isInitialized) return;
            var harmony = new HarmonyLib.Harmony("ApGlyphs.Patches");
            harmony.PatchAll();

            // class injection here
            ClassInjector.RegisterTypeInIl2Cpp<NetworkClient>();

            isInitialized = true;
        }


#pragma warning disable IDE0060 // Remove unused parameter warning
        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPostfix]
        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.handle == lastSceneHandle) return;
            lastSceneHandle = scene.handle;
            if (scene.name != "Intro" || client) return;  // only run on Intro scene when NetworkClient is not initialized


            // create NetworkClient instance
            client = GameObject.Find("Manager intro")?.AddComponent<NetworkClient>();
            if (!client) {
                MelonLogger.Error("Failed to create NetworkClient instance");
                return;
            }

            // retreive network info from json
            string userDataDir = Path.Combine(Environment.CurrentDirectory, "UserData");
            string settingsPath = Path.Combine(userDataDir, "ConnectionConfig.json");
            if (!Directory.Exists(userDataDir))
                Directory.CreateDirectory(userDataDir);

            // create ConnectionConfig.json if it doesn't exist
            if (!File.Exists(settingsPath)) {
                var defaultObj = new {
                    WebHostUrl = client.WebHostUrl,
                    WebHostPort = client.WebHostPort
                };
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(defaultObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(settingsPath, json);
                MelonLogger.Msg($"Created default ConnectionConfig.json at {settingsPath}");
            }

            // read ConnectionConfig.json
            try {
                string json = File.ReadAllText(settingsPath);
                var root = Newtonsoft.Json.Linq.JObject.Parse(json);
                client.WebHostUrl = root["WebHostUrl"] != null ? (string)root["WebHostUrl"] : client.WebHostUrl;
                client.WebHostPort = root["WebHostPort"] != null ? (int)root["WebHostPort"] : client.WebHostPort;
                MelonLogger.Msg($"Loaded ConnectionConfig.json from {settingsPath}");
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to read ConnectionConfig.json: {ex.Message}");
                return;
            }
        }
#pragma warning restore IDE0060 // Restore unused parameter warning


        public static NetworkClient client;
        private static int lastSceneHandle;
        private bool isInitialized = false;
    }
}