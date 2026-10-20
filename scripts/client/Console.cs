using System;
using System.Collections;
using System.Collections.Generic;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace ApGlyphs {
    [HarmonyPatch]
    public static class Console {
        public static void Init() {
            APCanvas = Main.APCanvas;

            overlay = new GameObject("ConsoleOverlay");
            overlay.transform.SetParent(APCanvas.transform, false);

            RectTransform rect = overlay.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = overlay.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.6f);
            image.raycastTarget = false;

            overlay.SetActive(false);
        }

        public static void Update() {
            if (!overlay) return;
            if (closeNextFrame) {
                FlushMessage();
                overlay.SetActive(false);
                closeNextFrame = false;
                consoleOpen = false;
                return;
            }
            if (consoleOpen) {
                // printing many messages at once can cause the game to crash
                if (logQueue.Count > 0)
                    ProcessLogMessage(logQueue.Dequeue());

                // probably should refactor to be fully event driven but since BuildTextPlus doesn't populate newLines until the next frame, it is difficult
                RepositionLogMessages();

                try {
                    isConsoleCall = true;
                    if (!currentText) CreateBlankMessage();
                    if (Input.GetKeyDown(KeyCode.Escape)) {
                        closeNextFrame = true;
                        return;
                    }
                    if (Input.GetKeyDown(KeyCode.Return)) {
                        Send();
                        FlushMessage();
                        return;
                    }
                    foreach (char c in Input.inputString) {
                        if (c == '\n' || c == '\r') continue;
                        if (c == '\b') currentText.SetText(currentText.GetFullText()[..^1]);
                        else currentText.AppendText(c.ToString());
                    }
                } finally {
                    isConsoleCall = false; // prevents a broken state if any errors are thrown
                }
            } else {
                if (Input.GetKeyDown(KeyCode.T)) {
                    consoleOpen = true;
                    overlay.SetActive(true);
                    return;
                }
            }
        }

        [HarmonyPatch(typeof(Input), nameof(Input.GetKeyDown), new Type[] { typeof(KeyCode) })]
        [HarmonyPrefix]
        public static bool GetKeyDown(KeyCode key) {
            if (key == KeyCode.F7) return true; // F7 is the default UnityExplorer toggle keybind. Allowing this at all times. Maybe comment out for release?
            if (!consoleOpen || isConsoleCall) return true; // Allow exclusively calls from the console when the console is open.
            return false;
        }

        [HarmonyPatch(typeof(Input), nameof(Input.GetKey), new Type[] { typeof(KeyCode) })]
        [HarmonyPrefix]
        public static bool GetKey() {
            if (!consoleOpen) return true;
            return false;
        }

        [HarmonyPatch(typeof(Input), nameof(Input.GetAxis), new Type[] { typeof(string) })]
        [HarmonyPrefix]
        public static bool GetAxis() {
            if (!consoleOpen) return true;
            return false;
        }

        private static void CreateBlankMessage() {
            GameObject msgObj = new GameObject("Message");
            RectTransform rect = msgObj.AddComponent<RectTransform>();
            msgObj.transform.SetParent(overlay.transform, false);

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.pivot = Vector2.zero;
            rect.anchoredPosition = consoleAnchorPos;

            currentText = msgObj.AddComponent<BuildTextPlus>();
            currentText.order = 10000;
            currentText.alignment = BuildTextPlus.Alignment.Left;
            currentText.textsize = TEXT_SIZE;
            currentText.width = LOG_MAX_WIDTH;
            currentText.pushUpOnNewLine = true;
            // currentText.OnNewLine += () => RepositionLogMessages(currentText.newLines);
        }

        private static void FlushMessage() {
            UnityEngine.Object.Destroy(currentText.gameObject);
            currentText = null;
        }

        private static void Send() {
            string msg = currentText.GetFullText();
            if (string.IsNullOrWhiteSpace(msg)) return;
            // MelonLogger.Msg("[UserConsole]: " + msg);
            NetworkClient.Say(CommandProcesser.ParseMessage(msg));
            // Log(currentText.substrings.ToArray());
        }

        public static void Log(string msg) => Log((msg, Color.white));

        public static void Log(LogMessage msg) {
            (string, Color)[] resolvedParts = new (string, Color)[msg.Parts.Length];
            for (int i = 0; i < msg.Parts.Length; i++)
                resolvedParts[i] = (msg.Parts[i].Text, new Color32(msg.Parts[i].Color.R, msg.Parts[i].Color.G, msg.Parts[i].Color.B, 255));
            Log(resolvedParts);
        }

        public static void Log(params (string, Color)[] parts) => logQueue.Enqueue(parts);

        private static void ProcessLogMessage(params (string, Color)[] parts) {
            string fullText = "";
            foreach (var part in parts)
                fullText += part.Item1;

            if (string.IsNullOrWhiteSpace(fullText)) return;

            GameObject msgObj = new GameObject("LogMsg");
            RectTransform rect = msgObj.AddComponent<RectTransform>();
            msgObj.transform.SetParent(overlay.transform, false);

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.pivot = Vector2.zero;
            rect.anchoredPosition = consoleAnchorPos + new Vector2(0f, TEXT_SIZE * 1.15f * (currentText ? currentText.newLines : 1));

            BuildTextPlus btp = msgObj.AddComponent<BuildTextPlus>();
            btp.Init(); // Call Init manually since if the console is hidden, it will wait to start until it is shown.
            btp.order = 10000;
            btp.alignment = BuildTextPlus.Alignment.Left;
            btp.textsize = TEXT_SIZE;
            btp.width = LOG_MAX_WIDTH;
            btp.pushUpOnNewLine = true;

            btp.SetText(parts);
            MelonLogger.Msg("[Archipelago]: " + fullText);
            RepositionLogMessages(btp.newLines);
            log.Add(btp);
        }

        public static void RepositionLogMessages(int incomingLines = 1) {
            int newLines = incomingLines + (currentText ? currentText.newLines : 1) - 1;
            foreach (BuildTextPlus entry in log)
                newLines += entry.newLines;

            List<int> toRemove = new List<int>();
            int i = 0;
            foreach (BuildTextPlus entry in log) {
                RectTransform entryRect = entry.GetComponent<RectTransform>();
                newLines -= entry.newLines;
                entryRect.anchoredPosition = consoleAnchorPos + new Vector2(0f, TEXT_SIZE * 1.15f * (newLines + 1));
                if (entryRect.anchoredPosition.y > LOG_MAX_HEIGHT)
                    toRemove.Add(i);
                i++;
            }

            foreach (int index in toRemove) {
                UnityEngine.Object.Destroy(log[index].gameObject);
                log.RemoveAt(index);
            }
        }

        public static void Clear() {
            foreach (BuildTextPlus text in log) {
                UnityEngine.Object.Destroy(text.gameObject);
            }
            log.Clear();
        }

        private static bool consoleOpen = false;
        private static bool closeNextFrame = false;
        private static Canvas APCanvas;
        private static GameObject overlay;
        private static bool isConsoleCall = false;
        private static BuildTextPlus currentText;
        private static readonly List<BuildTextPlus> log = new List<BuildTextPlus>();
        private static readonly Queue<(string, Color)[]> logQueue = new Queue<(string, Color)[]>();
        private static readonly Vector2 consoleAnchorPos = new Vector2(-20f, -20f);

        // constants but temporarily static for runtime editing
        private static float TEXT_SIZE = 15f;
        private static float LOG_MAX_HEIGHT = 1000f;
        private static float LOG_MAX_WIDTH = 1860f;
    }
}