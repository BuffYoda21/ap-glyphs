using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ApGlyphs {
    [HarmonyPatch]
    public static class TrapManager {
        public static void Init() {
            if (!player) player = SceneSearcher.Find("Player")?.GetComponent<PlayerController>();
            if (player && !rb) rb = player.GetComponent<Rigidbody2D>();
            if (!camera) camera = SceneSearcher.Find("Main Camera Parent/Main Camera");
        }

        public static void SpawnTrap(string trapName) {
            if (!traps.ContainsKey(trapName)) return;
            Trap trap = traps[trapName];
            switch (trap) {
                case Trap.Momentum:
                    MomentumTrap();
                    break;
                case Trap.John:
                    JohnTrap();
                    break;
                case Trap.Slow:
                    SlowTrap();
                    break;
                case Trap.Screen:
                    ScreenFlipTrap();
                    break;
                case Trap.Dash:
                    DashTrap();
                    break;
            }
        }

        private static void MomentumTrap() {
            if (!rb) return;
            rb.AddForce(new Vector2(UnityEngine.Random.Range(-30f, 30f), UnityEngine.Random.Range(-30f, 30f)), ForceMode2D.Impulse);
        }

        // call on mod start
        public static void PrepareJohnTrap() {
            if (johnTrapSetUp) return;
            SceneManager.LoadScene("Outer Void", LoadSceneMode.Single);
            MelonCoroutines.Start(WaitForVoidSetup());
        }

        private static IEnumerator WaitForVoidSetup() {
            do {
                vgm = GameObject.Find("Void [Game Manager]");
                john = GameObject.Find("Clarity Figure"); ;
                johnSolver = GameObject.Find("ClarityFigureSolver");
                flickerScreens = GameObject.Find("Main Camera Parent/Main Camera/FlickerScreens");
                yield return null;
            } while (!vgm || !john || !johnSolver || !flickerScreens);
            vgm.SetActive(false);
            john.SetActive(false);
            johnSolver.SetActive(false);
            flickerScreens.SetActive(false);
            Object.DontDestroyOnLoad(vgm);
            Object.DontDestroyOnLoad(john);
            Object.DontDestroyOnLoad(johnSolver);
            flickerScreens.transform.SetParent(null);
            Object.DontDestroyOnLoad(flickerScreens);
            johnTrapSetUp = true;
            SceneManager.LoadScene("Title", LoadSceneMode.Single);
        }

        private static void JohnTrap() {
            if (!john) return;
            john.SetActive(true);
            ClarityFigure johnAi = john.GetComponent<ClarityFigure>();
            johnAi.pursueNode = new GameObject("Pursue Node");
            johnSolver.SetActive(true);
            vgm.SetActive(true);
            vgm.GetComponent<VoidGameManager>().UI = new GameObject("dummy ahhh UI object so the void game manager can nuke your game without killing it's self lmao");
            flickerScreens.transform.SetParent(camera.transform, false);
            vgm.GetComponent<VoidGameManager>().Start();
            Vector3 offset = Random.insideUnitCircle.normalized * 30f;
            john.transform.position = player.transform.position + offset;
            johnAi.Start();
            johnAi.PlayerSighted();
            TimerManager.StartTimer(30f);
            MelonCoroutines.Start(JohnTrapTimer(30f, john));
        }

        private static IEnumerator JohnTrapTimer(float seconds, GameObject john) {
            MelonCoroutines.Start(JohnWarpDetector());
            yield return new WaitForSeconds(seconds);
            john.SetActive(false);
            johnSolver.SetActive(false);
            vgm.SetActive(false);
            Hover hover = SceneSearcher.Find("Main Camera Parent")?.GetComponent<Hover>();
            if (hover) hover.enabled = false;
        }

        private static IEnumerator JohnWarpDetector() {
            Vector3 lastPlayerPos = Vector3.zero;
            while (john.activeSelf) {
                if (Vector3.Distance(player.transform.position, lastPlayerPos) > 10f) john.GetComponent<ClarityFigure>().PlayerSighted(); // detect player warps
                lastPlayerPos = player.transform.position;
                yield return null;
            }
        }

        private static void SlowTrap() {
            player.speed /= 2f;
            player.maxspeed /= 2f;
            TimerManager.StartTimer(30f);
            MelonCoroutines.Start(SlowTrapTimer(30f));
        }

        private static IEnumerator SlowTrapTimer(float seconds) {
            yield return new WaitForSeconds(seconds);
            // reset to default speed
            player.speed = 6f;
            player.maxspeed = 6f;
        }

        private static void ScreenFlipTrap() {
            if (!camera) return;
            float z = (float)UnityEngine.Random.Range(-100, 100) / 100f;
            float w = 1f - z;
            camera.localRotation = new Quaternion(0f, 0f, z, w);
            TimerManager.StartTimer(30f);
            MelonCoroutines.Start(ScreenFlipTrapTimer(30f));
        }

        private static IEnumerator ScreenFlipTrapTimer(float seconds) {
            yield return new WaitForSeconds(seconds);
            // reset to default rotation
            camera.localRotation = Quaternion.identity;
        }

        private static void DashTrap() {
            player.dashspeed /= 2f;
            TimerManager.StartTimer(30f);
            MelonCoroutines.Start(DashTrapTimer(30f));
        }

        private static IEnumerator DashTrapTimer(float seconds) {
            yield return new WaitForSeconds(seconds);
            // reset to default speed
            player.dashspeed = 18f;
        }

        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPostfix]
        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.handle == lastSceneHandle) return;
            lastSceneHandle = scene.handle;
            if (scene.name == "Title") PrepareJohnTrap();
            if (scene.name != "Game" && scene.name != "Memory" && scene.name != "Outer Void") return;
            Init();
        }

        [HarmonyPatch(typeof(SceneManager), "Internal_ActiveSceneChanged")]
        [HarmonyPrefix]
        public static void BeforeSceneChange(Scene previousActiveScene, Scene newActiveScene) {
            if (!flickerScreens) return;
            flickerScreens.transform.SetParent(null);
            Object.DontDestroyOnLoad(flickerScreens);
        }

        private static PlayerController player;
        private static Rigidbody2D rb;
        private static Transform camera;
        private static GameObject vgm;
        private static GameObject john;
        private static GameObject johnSolver;
        private static GameObject flickerScreens;
        private static bool johnTrapSetUp = false;
        private static int lastSceneHandle = -1;
        private static Dictionary<string, Trap> traps = new Dictionary<string, Trap>() {
            { "Momentum Trap", Trap.Momentum },
            { "John Trap", Trap.John },
            { "Slow Trap", Trap.Slow },
            { "Screen Flip Trap", Trap.Screen },
            { "Dash Trap", Trap.Dash },
        };
        private enum Trap : int {
            None = 0,
            Momentum = 1,
            John = 2,
            Slow = 3,
            Screen = 4,
            Dash = 5,
        }
    }
}