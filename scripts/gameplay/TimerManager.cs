using System.Collections;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ApGlyphs {
    [HarmonyPatch]
    public static class TimerManager {
        public static void Init() {
            if (!timer) timer = SceneSearcher.Find("Main Camera Parent/Main Camera/timerParent")?.gameObject;
            if (!timer) return;
            if (!min) min = timer.transform.Find("Minutes")?.GetComponent<BuildText>();
            if (!sec) sec = timer.transform.Find("Second")?.GetComponent<BuildText>();
        }

        public static void StartTimer(float seconds) {
            if (!timer || !min || !sec || timer.activeSelf) return;
            MelonCoroutines.Start(RunTimer(seconds));
        }

        private static IEnumerator RunTimer(float seconds) {
            timer.SetActive(true);
            while (seconds >= 0) {
                UpdateTimer(seconds);
                seconds -= 1f;
                yield return new WaitForSeconds(1f);
            }
            timer.SetActive(false);
        }

        private static void UpdateTimer(float newSeconds) {
            float newMinutes = (int)newSeconds / 60f;
            newSeconds %= 60f;
            for (int i = min.transform.childCount - 1; i >= 0; i--) {
                Transform child = min.transform.GetChild(i);
                Object.Destroy(child.gameObject);
            }
            for (int i = sec.transform.childCount - 1; i >= 0; i--) {
                Transform child = sec.transform.GetChild(i);
                Object.Destroy(child.gameObject);
            }
            float minutes = newMinutes;
            float seconds = newSeconds;
            min.text = minutes.ToString("0");
            sec.text = seconds.ToString("00");
            min.x = 0f;
            sec.x = 0f;
            min.placed = false;
            sec.placed = false;
        }

        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPostfix]
        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.handle == lastSceneHandle) return;
            lastSceneHandle = scene.handle;
            if (scene.name != "Game" && scene.name != "Memory" && scene.name != "Outer Void") return;
            Init();
        }

        private static GameObject timer;
        private static BuildText min;
        private static BuildText sec;
        private static int lastSceneHandle = -1;
    }
}