using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ApGlyphs {
    public static class CommandProcesser {
        public static string ParseMessage(string msg) {
            if (string.IsNullOrWhiteSpace(msg) || msg[0] != '!') return msg;
            string[] parts = msg.ToLower().Split(' ');
            switch (parts[0]) {
                case "!hint":
                    // if the player is hinting for a button shard, check if it is in a bundle and if so, hint for the bundle instead
                    if (parts.Length < 2 || !msg[6..].Trim().StartsWith("Button Shard".ToLower()) ||
                        NetworkClient.session.RoomState.HintPoints < NetworkClient.session.RoomState.HintCost)
                        return msg;

                    int id;
                    try {
                        id = int.Parse(parts[^1]);
                    } catch {
                        return msg;
                    }

                    foreach (KeyValuePair<string, List<int>> kv in ButtonManager.bundleContents) {
                        if (kv.Value.Contains(id)) {
                            Console.Log($"Button Shard {id} can be found in {kv.Key}. Hinting for {kv.Key}.");
                            return $"!hint {kv.Key}";
                        }
                    }
                    return msg;

                case "!clear":
                    Console.Clear();
                    return null;

                case "!info":
                    if (parts.Length < 2) {
                        Console.Log($"{NetworkClient.ConnectionInfo.SlotName}: {msg}");
                        Console.Log("Usage: !info <option name>");
                        return null;
                    }
                    string option = string.Join("_", parts[1..]);
                    Console.Log($"{option}: {((JObject)NetworkClient.slotData["options"]).ToObject<Dictionary<string, string>>()[option]}");
                    return null;
            }

            return msg;
        }
    }
}