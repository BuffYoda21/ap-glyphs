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
    public class ClientWrapper : MonoBehaviour {
        public void Start() {
            client = new NetworkClient();
            client.itemCache = itemCache;
            client.inventory = GameObject.Find("Manager intro")?.GetComponent<InventoryManager>();

            // retreive network info from json
            string userDataDir = Path.Combine(Environment.CurrentDirectory, "UserData");
            string settingsPath = Path.Combine(userDataDir, "ConnectionConfig.json");
            if (!Directory.Exists(userDataDir))
                Directory.CreateDirectory(userDataDir);

            // create ConnectionConfig.json if it doesn't exist
            if (!File.Exists(settingsPath)) {
                var defaultObj = new {
                    WebHostUrl = client.WebHostUrl,
                    WebHostPort = client.WebHostPort,
                    SlotName = client.SlotName,
                    password = client.password
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
                client.SlotName = root["SlotName"] != null ? (string)root["SlotName"] : client.SlotName;
                client.password = root["password"] != null ? (string)root["password"] : client.password;
                MelonLogger.Msg($"Loaded ConnectionConfig.json from {settingsPath}");
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to read ConnectionConfig.json: {ex.Message}");
                return;
            }

            // start connecting to server
            client.initialized = true;
        }

        public void SetItemCacheRef(ItemCache cache) {
            itemCache = cache;
        }

        public void Update() => client.Update();

        public class ConnectionIndicator : MonoBehaviour {
            public void SetConnectionState(bool connected) {
                if (connected) {
                    foreach (Image orb in this.GetComponentsInChildren<Image>()) {
                        int id = int.Parse(orb.transform.name.Split('_')[1]);
                        if (id == 0) orb.color = new Color32(117, 194, 117, 255);
                        if (id == 1) orb.color = new Color32(201, 118, 130, 255);
                        if (id == 2) orb.color = new Color32(238, 227, 145, 255);
                        if (id == 3) orb.color = new Color32(118, 126, 189, 255);
                        if (id == 4) orb.color = new Color32(217, 160, 125, 255);
                        if (id == 5) orb.color = new Color32(202, 148, 194, 255);
                    }
                } else {
                    foreach (Image orb in this.GetComponentsInChildren<Image>()) {
                        orb.color = new Color32(49, 107, 132, 255);
                    }
                }
            }
        }

        public NetworkClient client;
        private ItemCache itemCache;
    }

    public class NetworkClient {
        public void Update() {
            if (!initialized) return;

            if (!indicator) CreateConnectionIndicator();
            if (indicator) indicator.SetConnectionState(isConnected);

            if (!isConnected && !isConnecting && Time.time - lastConnectAttempt > 15) {
                lastConnectAttempt = Time.time;
                isConnecting = true;
                MelonLogger.Msg("Attempting to connect to server at " + WebHostUrl + ":" + WebHostPort);
                _ = ConnectAsync();
            } else if (isConnected) {
                isConnected = session.Socket.Connected;
                if (enablePeriodicSessionUpdates && !sessionUpdateInProgress && Time.time - lastSessionUpdate > SessionUpdateIntervalSeconds) {
                    lastSessionUpdate = Time.time;
                    sessionUpdateInProgress = true;
                    _ = RefreshSessionAsync();
                }
            }
        }

        private async Task RefreshSessionAsync() {
            try {
                if (session == null || itemCache == null) return;
                await itemCache.FetchItemPool(session, session.Locations.AllLocations);
            } catch (Exception ex) {
                MelonLogger.Error($"Session refresh failed: {ex.Message}");
            } finally {
                sessionUpdateInProgress = false;
            }
        }

        private async Task ConnectAsync() {
            session = ArchipelagoSessionFactory.CreateSession(WebHostUrl, WebHostPort);
            LoginResult loginResult = session.TryConnectAndLogin(
                "GLYPHS",
                SlotName,
                ItemsHandlingFlags.AllItems,
                ArchipelagoProtocolVersion,
                null,
                null,
                password,
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

            SlotId = session.ConnectionInfo.Slot;

            await OnConnectionSuccess();
        }

        private async Task OnConnectionSuccess() {
            await itemCache.FetchItemPool(session, session.Locations.AllLocations);
        }

        public void CollectItem(ArchipelagoItem apItem) {
            long[] locationArray = new long[1];
            locationArray[0] = apItem.locId;
            session.Locations.CompleteLocationChecks(locationArray);
        }

        public void ClearGoal() {
            session.SetGoalAchieved();
        }

        private void CreateConnectionIndicator() {
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
            const int orbCount = 6;
            const float radius = 27f;
            const float orbSize = 32f;
            Sprite orbSprite = CreateCircleSprite((int)orbSize);
            List<RectTransform> orbRects = new List<RectTransform>();
            for (int i = 0; i < orbCount; i++) {
                float angleRad = Mathf.Deg2Rad * (i * 360f / orbCount + 360f / (orbCount * 2f));
                GameObject orbObj = new GameObject($"Orb_{i}");
                orbObj.transform.SetParent(rootObj.transform, false);
                RectTransform rect = orbObj.AddComponent<RectTransform>();
                orbRects.Add(rect);
                rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = new Vector2(
                    Mathf.Cos(angleRad) * radius,
                    Mathf.Sin(angleRad) * radius
                );
                rect.sizeDelta = new Vector2(orbSize, orbSize);
                Image img = orbObj.AddComponent<Image>();
                img.color = new Color32(49, 107, 132, 255);
                img.sprite = orbSprite;
            }
            orbRects.Sort((a, b) =>
                b.anchoredPosition.y.CompareTo(a.anchoredPosition.y));
            foreach (RectTransform rect in orbRects) {
                rect.SetAsLastSibling();
            }
            indicator = rootObj.AddComponent<ClientWrapper.ConnectionIndicator>();
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

        /*
        // that's not how that works bruh
        public void DEBUG_get_roomstate() {
            MelonLogger.Msg("DEBUG: " + session.RoomState.ToString());
        }
        */

        public void DEBUG_get_unchecked_locations() {
            MelonLogger.Msg("DEBUG:");
            MelonLogger.Msg("Unchecked locations:");
            foreach (var location in session.Locations.AllMissingLocations) {
                MelonLogger.Msg(session.Locations.GetLocationNameFromId(location, null));
            }
        }

        public void DEBUG_get_checked_locations() {
            MelonLogger.Msg("DEBUG:");
            MelonLogger.Msg("Checked locations:");
            foreach (var location in session.Locations.AllLocationsChecked) {
                MelonLogger.Msg(session.Locations.GetLocationNameFromId(location, null));
            }
        }

        public void DEBUG_collect_location(string location) {
            DEBUG_collect_location(session.Locations.GetLocationIdFromName("GLYPHS", location));
        }

        public void DEBUG_collect_location(long location) {
            long[] locationArray = new long[1];
            locationArray[0] = location;
            session.Locations.CompleteLocationChecks(locationArray);
        }

        public bool initialized = false;
        private bool isConnecting = false;
        public bool isConnected = false;
        private float lastConnectAttempt = -15f;
        public ArchipelagoSession session;
        private readonly Version ArchipelagoProtocolVersion = new Version(0, 6, 0);
        public string WebHostUrl = "archipelago.gg";
        public int WebHostPort = 0;
        public string SlotName = "Player1";
        public int SlotId = 0;
        public string password = null;
        public Dictionary<string, object> slotData;
        public Dictionary<string, object> options;
        public ClientWrapper.ConnectionIndicator indicator;
        public ItemCache itemCache;
        public InventoryManager inventory;

        // Periodic session update control
        private float lastSessionUpdate = -999f;
        public float SessionUpdateIntervalSeconds = 5f;
        private bool sessionUpdateInProgress = false;
        public bool enablePeriodicSessionUpdates = true;
    }
}