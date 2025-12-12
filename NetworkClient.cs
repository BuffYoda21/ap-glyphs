using System;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using MelonLoader;
using UnityEngine;

namespace ApGlyphs {
    public class NetworkClient : MonoBehaviour {
        public void Update() {
            if (!initialized) return;

            if (!isConnected && Time.time - lastConnectAttempt > 15) {
                // connect to server
                lastConnectAttempt = Time.time;
                MelonLogger.Msg("Attempting to connect to server at " + WebHostUrl + ":" + WebHostPort);
                bool success = Connect();
                if (!success) {
                    MelonLogger.Error("Failed to connect to server");
                    return;
                } else {
                    MelonLogger.Msg("Connected to Multiworld server");
                    MelonLogger.Msg("-------------------------------------------------------------------------");
                    MelonLogger.Msg("Session Info:");
                    MelonLogger.Msg("  Game: " + session.ConnectionInfo.Game);
                    MelonLogger.Msg("  ItemsHandling: " + session.ConnectionInfo.ItemsHandlingFlags);
                    MelonLogger.Msg("  Slot: " + session.ConnectionInfo.Slot);
                    MelonLogger.Msg("  Tags: " + string.Join(", ", session.ConnectionInfo.Tags));
                    MelonLogger.Msg("  Team: " + session.ConnectionInfo.Team);
                    MelonLogger.Msg("  Uuid: " + session.ConnectionInfo.Uuid);
                    MelonLogger.Msg("-------------------------------------------------------------------------");
                    isConnected = true;
                }
            }
        }

        public bool Connect() {
            session = ArchipelagoSessionFactory.CreateSession(WebHostUrl, WebHostPort);
            LoginResult loginResult = session.TryConnectAndLogin("GLYPHS",
                                                            SlotName,
                                                            ItemsHandlingFlags.AllItems,
                                                            ArchipelagoProtocolVersion,
                                                            null,
                                                            null,
                                                            password,
                                                            false);

            if (loginResult is LoginFailure failure) {
                string errors = string.Join(", ", failure.Errors);
                MelonLogger.Error($"Unable to connect to Archipelago because: {string.Join(", ", failure.Errors)}");
                return false;
            } else if (loginResult is LoginSuccessful success) {
                return true;
            } else {
                MelonLogger.Error($"Unexpected LoginResult type when connecting to Archipelago: {loginResult}");
                return false;
            }
        }

        public void DEBUG_get_roomstate() {
            MelonLogger.Msg("DEBUG: " + session.RoomState.ToString());
        }

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

        public bool initialized = false;
        public bool isConnected = false;
        private float lastConnectAttempt = -15f;
        public ArchipelagoSession session;
        private readonly Version ArchipelagoProtocolVersion = new Version(0, 6, 0);
        public string WebHostUrl = "archipelago.gg";
        public int WebHostPort = 0;
        public string SlotName = "Player1";
        public string password = null;
    }
}