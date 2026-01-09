using System.Collections;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ApGlyphs {
    [HarmonyPatch]
    public static class NotificationManager {
        public static void Init() {
            if (acceptingNotifications) return;
            MelonCoroutines.Start(NotificationProcesser());
        }

        public static void Notify(string msg) {
            Notify(msg, Color.white);
        }

        public static void Notify(string msg, Color color) {
            notificationQueue.Enqueue(new SerializedNotification { msg = msg, color = color });
        }

        private static IEnumerator NotificationProcesser() {
            MelonLogger.Msg("[NotificationManager] Accepting notifications");
            acceptingNotifications = true;
            while (true) {
                while (!ready || notificationQueue.Count == 0 || notifications.Count >= maxNotifications) yield return null;
                SerializedNotification notification = notificationQueue.Dequeue();
                CreateNotification(notification.msg, notification.color);
                yield return new WaitForSeconds(0.25f);
            }
        }

        private static void CreateNotification(string msg, Color color) {
            GameObject obj = new GameObject("Notification");
            obj.transform.SetParent(mainCamera.transform, false);
            obj.transform.localPosition = new Vector3(-13.5f, -7.5f, 20f);
            Notification notif = obj.AddComponent<Notification>();
            notif.msg = obj.AddComponent<BuildText>();
            BuildText bt = notif.msg;
            bt.text = msg;
            bt.normaltext = true;
            bt.col = color;
            bt.textsize = 0.5f;
            bt.order = 10000;
            bt.width = 26.5f;
            if (notifications.Count > 0) {
                foreach (Notification notification in notifications) {
                    notification.transform.localPosition += Vector3.up * offset;
                }
            }
            notifications.Add(notif);
        }

        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPostfix]
        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.handle == lastSceneHandle) return;
            lastSceneHandle = scene.handle;
            ready = false;
            notifications.Clear();
            if (scene.name != "Game" && scene.name != "Memory" && scene.name != "Outer Void") return;
            if (!mainCamera) mainCamera = SceneSearcher.Find("Main Camera Parent/Main Camera")?.gameObject;
            if (mainCamera) { ready = true; MelonLogger.Msg("NotificationManager ready"); }
        }

        private static int lastSceneHandle;
        private static bool ready = false;
        private static bool acceptingNotifications = false;
        private static readonly float offset = .75f;
        private static readonly int maxNotifications = 3;
        private static GameObject mainCamera;
        private static Queue<SerializedNotification> notificationQueue = new Queue<SerializedNotification>();
        public static List<Notification> notifications = new List<Notification>();

        private struct SerializedNotification {
            public string msg;
            public Color color;
        }

        public class Notification : MonoBehaviour {
            public void Start() {
                if (!msg || msg.text == null || msg.text == "") Destroy(gameObject);
                timer = Time.time + lifetime;
                msg.text = NormalizeText(msg.text);
            }

            public void Update() {
                if (Time.time < timer) return;
                notifications.Remove(this);
                Destroy(gameObject);
            }

            private string NormalizeText(string origText) {
                if (origText == null) return null;
                StringBuilder sb = new StringBuilder(origText.Length);
                foreach (char c in origText.ToLowerInvariant()) {
                    if (c == '_') sb.Append(' ');
                    else if (c == '?') sb.Append(')'); // for whatever reason, BuildText recognizes ) as a question mark and doesn't like ?s
                    else if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)) sb.Append(c);
                }
                return sb.ToString();
            }

            public BuildText msg;
            private float lifetime = 5f;
            private float timer = -1f;
        }
    }
}