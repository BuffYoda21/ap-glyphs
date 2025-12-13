using System.Collections.Generic;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Models;

namespace ApGlyphs {
    public class ItemCache {
        private Dictionary<long, ScoutedItemInfo> itemPool =
            new Dictionary<long, ScoutedItemInfo>();

        public async Task FetchItemPool(ArchipelagoSession session, IEnumerable<long> locationIds) {
            List<long> idList = new List<long>();
            foreach (long id in locationIds)
                idList.Add(id);
            Dictionary<long, ScoutedItemInfo> result = await session.Locations.ScoutLocationsAsync(idList.ToArray());
            foreach (KeyValuePair<long, ScoutedItemInfo> kv in result)
                itemPool[kv.Key] = kv.Value;
            itemsReady = true;
        }

        public bool TryGetItem(long locationId, out ScoutedItemInfo info) {
            info = null;
            if (!itemsReady) return false;
            return itemPool.TryGetValue(locationId, out info);
        }

        public bool itemsReady = false;
    }
}