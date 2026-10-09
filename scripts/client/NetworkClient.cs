using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using MelonLoader;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;

namespace ApGlyphs {
    public static class NetworkClient {
        static NetworkClient() {
            // retreive network info from json
            string userDataDir = Path.Combine(Environment.CurrentDirectory, "UserData");
            string settingsPath = Path.Combine(userDataDir, "ConnectionConfig.json");
            if (!Directory.Exists(userDataDir))
                Directory.CreateDirectory(userDataDir);

            // create ConnectionConfig.json if it doesn't exist
            if (!File.Exists(settingsPath)) {
                var defaultObj = new {
                    WebHostUrl = ConnectionInfo.WebHostUrl,
                    WebHostPort = ConnectionInfo.WebHostPort,
                    SlotName = ConnectionInfo.SlotName,
                    password = ConnectionInfo.password
                };
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(defaultObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(settingsPath, json);
                MelonLogger.Msg($"Created default ConnectionConfig.json at {settingsPath}");
            }

            // read ConnectionConfig.json
            try {
                string json = File.ReadAllText(settingsPath);
                JObject root = JObject.Parse(json);
                ConnectionInfo.WebHostUrl = root["WebHostUrl"] != null ? (string)root["WebHostUrl"] : ConnectionInfo.WebHostUrl;
                ConnectionInfo.WebHostPort = root["WebHostPort"] != null ? (int)root["WebHostPort"] : ConnectionInfo.WebHostPort;
                ConnectionInfo.SlotName = root["SlotName"] != null ? (string)root["SlotName"] : ConnectionInfo.SlotName;
                ConnectionInfo.password = root["password"] != null ? (string)root["password"] : ConnectionInfo.password;
                MelonLogger.Msg($"Loaded ConnectionConfig.json from {settingsPath}");
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to read ConnectionConfig.json: {ex.Message}");
                return;
            }
        }

        public static void Update() {
            if (!indicator) CreateConnectionIndicator();
            if (indicator) indicator.SetConnectionState(isConnected);

            if (!isConnected && !isConnecting && Time.time - lastConnectAttempt > CONNECTION_RETRY_INTERVAL) {
                lastConnectAttempt = Time.time;
                isConnecting = true;
                MelonLogger.Msg("Attempting to connect to server at " + ConnectionInfo.WebHostUrl + ":" + ConnectionInfo.WebHostPort);
                _ = ConnectAsync();
            } else if (isConnected) {
                isConnected = session.Socket.Connected;
                if (!sessionUpdateInProgress && Time.time - lastSessionUpdate > SessionUpdateIntervalSeconds) {
                    lastSessionUpdate = Time.time;
                    sessionUpdateInProgress = true;
                    _ = RefreshSessionAsync();
                }
            }
        }

        private static async Task RefreshSessionAsync() {
            try {
                if (session == null || itemCache == null) return;
                await itemCache.FetchItemPool(session, session.Locations.AllLocations);
            } catch (Exception ex) {
                MelonLogger.Error($"Session refresh failed: {ex.Message}");
            } finally {
                sessionUpdateInProgress = false;
            }
        }

        private static async Task ConnectAsync() {
            session = ArchipelagoSessionFactory.CreateSession(ConnectionInfo.WebHostUrl, ConnectionInfo.WebHostPort);
            LoginResult loginResult = session.TryConnectAndLogin(
                "GLYPHS",
                ConnectionInfo.SlotName,
                ItemsHandlingFlags.AllItems,
                AP_PROTOCOL_VERSION,
                null,
                null,
                ConnectionInfo.password,
                true
            );

            if (loginResult is LoginFailure failure) {
                MelonLogger.Error($"Failed to connect: {string.Join(", ", failure.Errors)}");
                isConnecting = false;
                return;
            } else {
                LoginSuccessful loginSuccessful = loginResult as LoginSuccessful;
                slotData = loginSuccessful.SlotData;
                try {
                    if (slotData != null && slotData.ContainsKey("options") && slotData["options"] is JObject jOptions) {
                        options = new Dictionary<string, object>();
                        foreach (JProperty prop in jOptions.Properties()) {
                            JToken val = prop.Value;
                            switch (val.Type) {
                                case JTokenType.Integer:
                                    options[prop.Name] = (int)val;
                                    break;
                                case JTokenType.Float:
                                    options[prop.Name] = (double)val;
                                    break;
                                case JTokenType.Boolean:
                                    options[prop.Name] = (bool)val;
                                    break;
                                case JTokenType.String:
                                    options[prop.Name] = (string)val;
                                    break;
                                default:
                                    options[prop.Name] = val.ToString();
                                    break;
                            }
                        }
                    }
                } catch (Exception ex) {
                    MelonLogger.Error($"Failed to parse slot options: {ex.Message}");
                }
            }

            MelonLogger.Msg("Connected to Multiworld server");
            MelonLogger.Msg("-------------------------------------------------------------------------");
            MelonLogger.Msg("Session DEBUG:");
            MelonLogger.Msg("  Game: " + session.ConnectionInfo.Game);
            MelonLogger.Msg("  ItemsHandling: " + session.ConnectionInfo.ItemsHandlingFlags);
            MelonLogger.Msg("  Slot: " + session.ConnectionInfo.Slot);
            MelonLogger.Msg("  Tags: " + string.Join(", ", session.ConnectionInfo.Tags));
            MelonLogger.Msg("  Team: " + session.ConnectionInfo.Team);
            MelonLogger.Msg("  Uuid: " + session.ConnectionInfo.Uuid);
            MelonLogger.Msg("-------------------------------------------------------------------------");
            MelonLogger.Msg($"You are connected on slot {session.ConnectionInfo.Slot}, on team {session.ConnectionInfo.Team}");
            MelonLogger.Msg($"You have {session.RoomState.HintPoints}, and need {session.RoomState.HintCost} for a hint");

            isConnected = true;
            isConnecting = false;

            ConnectionInfo.SlotId = session.ConnectionInfo.Slot;

            if (options.ContainsKey("DeathLink") && Convert.ToBoolean(options["DeathLink"]))
                DeathLinkManager.EnableDeathLink();

            await OnConnectionSuccess();
        }

        private static async Task OnConnectionSuccess() => await itemCache.FetchItemPool(session, session.Locations.AllLocations);

        public static void CollectItem(ArchipelagoItem apItem) => CollectItem(apItem.locId);

        public static void CollectItem(long locId) {
            if (!session.Locations.AllMissingLocations.Contains(locId)) // Already collected or invalid ID
                return;

            long[] locationArray = new long[1];
            locationArray[0] = locId;
            itemCache.TryGetItem(locId, out var itemInfo);
            string notifMsg;
            Color notifColor;
            if (itemInfo == null) {
                notifMsg = "Found unknown item";
                notifColor = Color.red;
            } else {
                if (itemInfo.Player.Slot == ConnectionInfo.SlotId) {
                    notifMsg = $"Found {itemInfo.ItemName}";
                } else {
                    notifMsg = $"Sent {itemInfo.ItemName} to {itemInfo.Player.Name}";
                }
                if (itemInfo.Flags.HasFlag(ItemFlags.Advancement)) notifColor = new Color32(255, 0, 163, 255); // clarity pink
                else if (itemInfo.Flags.HasFlag(ItemFlags.Trap)) notifColor = Color.red;
                else notifColor = Color.white;
            }
            NotificationManager.Notify(notifMsg, notifColor);
            session.Locations.CompleteLocationChecks(locationArray);
        }

        public static void ClearGoal() => session.SetGoalAchieved();

        private static void CreateConnectionIndicator() {
            GameObject canvasObj = new GameObject("AP Canvas");
            UnityEngine.Object.DontDestroyOnLoad(canvasObj);
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            GameObject rootObj = new GameObject("ConnectionIndicator");
            rootObj.transform.SetParent(canvasObj.transform, false);
            RectTransform rootRect = rootObj.AddComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(1f, 0f);
            rootRect.anchorMax = new Vector2(1f, 0f);
            rootRect.pivot = new Vector2(1f, 0f);
            rootRect.anchoredPosition = new Vector2(-50f, 50f);
            rootRect.sizeDelta = new Vector2(96f, 96f);
            indicator = rootObj.AddComponent<ConnectionIndicator>();
        }

        private static Sprite CreateCircleSprite(int diameter) {
            Texture2D tex = new Texture2D(diameter, diameter, TextureFormat.ARGB32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            float r = diameter / 2f;
            Vector2 center = new Vector2(r, r);
            for (int y = 0; y < diameter; y++) {
                for (int x = 0; x < diameter; x++) {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    tex.SetPixel(x, y, dist <= r ? Color.white : Color.clear);
                }
            }
            tex.Apply();
            return Sprite.Create(
                tex,
                new Rect(0, 0, diameter, diameter),
                new Vector2(0.5f, 0.5f),
                100f
            );
        }

        public class ConnectionIndicator : MonoBehaviour {
            void Start() {
                img = gameObject.AddComponent<Image>();
                disconnectedSprite = SpriteCache.GetSprite("AP Logo Blue");
                connectedSprite = SpriteCache.GetSprite("AP Logo Color");
                img.sprite = disconnectedSprite;
            }

            public void SetConnectionState(bool status) {
                if (status == connected) return;
                connected = status;
                if (connected) {
                    if (!connectedSprite)
                        connectedSprite = SpriteCache.GetSprite("AP Logo Color");
                    img.sprite = connectedSprite;
                } else {
                    if (!disconnectedSprite)
                        disconnectedSprite = SpriteCache.GetSprite("AP Logo Blue");
                    img.sprite = disconnectedSprite;
                }
            }

            private bool connected = false;
            private Image img;
            private Sprite disconnectedSprite;
            private Sprite connectedSprite;
        }

        public static void DEBUG_get_unchecked_locations() {
            MelonLogger.Msg("DEBUG:");
            MelonLogger.Msg("Unchecked locations:");
            foreach (var location in session.Locations.AllMissingLocations) {
                MelonLogger.Msg(session.Locations.GetLocationNameFromId(location, null));
            }
        }

        public static void DEBUG_get_checked_locations() {
            MelonLogger.Msg("DEBUG:");
            MelonLogger.Msg("Checked locations:");
            foreach (var location in session.Locations.AllLocationsChecked) {
                MelonLogger.Msg(session.Locations.GetLocationNameFromId(location, null));
            }
        }

        public static void DEBUG_collect_location(string location) {
            DEBUG_collect_location(session.Locations.GetLocationIdFromName("GLYPHS", location));
        }

        public static void DEBUG_collect_location(long location) {
            long[] locationArray = new long[1];
            locationArray[0] = location;
            session.Locations.CompleteLocationChecks(locationArray);
        }

        private static bool isConnecting = false;
        public static bool isConnected = false;
        private static float lastConnectAttempt = -15f;
        public static ArchipelagoSession session;
        private static readonly Version AP_PROTOCOL_VERSION = new Version(0, 6, 8);
        public static Dictionary<string, object> slotData;
        public static Dictionary<string, object> options;
        private static ConnectionIndicator indicator;
        public static ItemCache itemCache = new ItemCache();
        private const float CONNECTION_RETRY_INTERVAL = 15f;

        public struct ConnectionInfo {
            public static string WebHostUrl = "archipelago.gg";
            public static int WebHostPort = 12345;
            public static string SlotName = "player 1";
            public static int SlotId = 0;
            public static string password = null;
        }

        // Periodic session update control
        private static float lastSessionUpdate = -999f;
        public static float SessionUpdateIntervalSeconds = 5f;
        private static bool sessionUpdateInProgress = false;
    }
}