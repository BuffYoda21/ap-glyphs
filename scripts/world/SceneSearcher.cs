using System.Collections.Generic;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ApGlyphs {
    [HarmonyPatch]
    public static class SceneSearcher {
        public static Transform Find(string path) {
            if (string.IsNullOrEmpty(path)) return null;

            // fast lookup, kinda the point of this stupid ahhhhh thing
            if (loggedTransforms.TryGetValue(path, out Transform cached)) {
                if (cached == null) {
                    loggedTransforms.Remove(path);
                } else {
                    //MelonLogger.Msg($"Cache hit for {path}");
                    return cached;
                }
            }

            // build ancestor full-paths: ["A","A/B","A/B/C"] for "A/B/C/D"
            string[] parts = path.Split('/');
            List<string> ancestors = new List<string>();
            for (int i = 0; i < parts.Length - 1; i++) {
                if (i == 0) ancestors.Add(parts[0]);
                else ancestors.Add(string.Join("/", parts, 0, i + 1));
            }

            // find deepest cached ancestor; clear null entries
            Transform ancestorTransform = null;
            string ancestorPath = null;
            for (int i = ancestors.Count - 1; i >= 0; i--) {
                string anc = ancestors[i];
                if (loggedTransforms.TryGetValue(anc, out Transform t)) {
                    if (t == null) {
                        loggedTransforms.Remove(anc);
                        continue;
                    }
                    ancestorTransform = t;
                    ancestorPath = anc;
                    break;
                }
            }

            if (ancestorTransform != null) {
                // relative path from ancestor to target
                int ancestorDepth = ancestorPath.Split('/').Length;
                int relLen = parts.Length - ancestorDepth;
                if (relLen <= 0) return ancestorTransform;
                string relativePath = string.Join("/", parts, ancestorDepth, relLen);

                Transform found = ancestorTransform.Find(relativePath);
                if (found != null) {
                    CacheTransformChain(found);
                    //MelonLogger.Msg($"Used shortcut lookup for {path} starting from {ancestorPath}");
                    return found;
                }
            }

            // fallback
            GameObject obj = GameObject.Find(path);
            if (obj != null) {
                CacheTransformChain(obj.transform);
                //MelonLogger.Msg("Used fallback lookup for " + path);
                return obj.transform;
            }

            //MelonLogger.Msg("Failed to find " + path);
            return null;
        }

        private static void CacheTransformChain(Transform t) {
            // Build chain from root -> t
            List<Transform> chain = new List<Transform>();
            Transform cur = t;
            while (cur != null) {
                chain.Add(cur);
                cur = cur.parent;
            }
            chain.Reverse();

            string acc = "";
            for (int i = 0; i < chain.Count; i++) {
                acc = i == 0 ? chain[i].name : acc + "/" + chain[i].name;
                loggedTransforms[acc] = chain[i];
            }
        }

        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPrefix]
        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.handle == lastSceneHandle) return;
            lastSceneHandle = scene.handle;
            loggedTransforms.Clear();
        }

        private static Dictionary<string, Transform> loggedTransforms = new Dictionary<string, Transform>();
        private static int lastSceneHandle = -1;
    }
}