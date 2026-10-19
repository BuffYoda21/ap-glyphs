using System.Collections.Generic;

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
                        if (kv.Value.Contains(id))
                            return $"!hint {kv.Key}";
                    }
                    return msg;
            }

            return msg;
        }
    }
}