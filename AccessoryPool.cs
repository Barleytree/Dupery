using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dupery
{
    class AccessoryPool
    {
        private const string MISSING_ACCESSORY_ID = "missing";

        [JsonProperty]
        public Dictionary<int, string> Hair { get; set; }
        [JsonProperty]
        public Dictionary<int, string> Body { get; set; }

        private Dictionary<string, Dictionary<int, string>> pool;
        private Dictionary<string, int> startingAccessoryNumbers;
        private Dictionary<string, string> missingAccessoryIds;

        public AccessoryPool()
        {
            if (Hair == null)
                Hair = new Dictionary<int, string>();
            if (Body == null)
                Body = new Dictionary<int, string>();

            pool = new Dictionary<string, Dictionary<int, string>>
            {
                { "Hair", Hair },
                { "Body", Body }
            };

            startingAccessoryNumbers = new Dictionary<string, int>
            {
                { "Hair", Db.Get().AccessorySlots.Hair.accessories.Count + 1 },
                { "Body", Db.Get().AccessorySlots.Body.accessories.Count + 1 }
            };

            missingAccessoryIds = new Dictionary<string, string>
            {
                { "Hair", $"hair_{MISSING_ACCESSORY_ID}" },
                { "Body", $"body_{MISSING_ACCESSORY_ID}" }
            };
        }

        public bool IsNativeAccessory(AccessorySlot slot, int accessoryNumber)
        {
            return accessoryNumber > 0 && accessoryNumber <= startingAccessoryNumbers[slot.Id];
        }

        public int GetAccessoryNumber(AccessorySlot slot, string id)
        {
            foreach (KeyValuePair<int, string> entry in pool[slot.Id])
            {
                if (entry.Value == id)
                {
                    return entry.Key;
                }
            }

            return -1;
        }

        public string GetId(AccessorySlot slot, int accessoryNumber)
        {
            pool[slot.Id].TryGetValue(accessoryNumber, out string id);
            return id;
        }

        public string GetMissingId(AccessorySlot slot)
        {
            return missingAccessoryIds[slot.Id];
        }

        public int TryInsertId(AccessorySlot slot, string id)
        {
            int accessoryNumber = startingAccessoryNumbers[slot.Id];
            while (true)
            {
                if (pool[slot.Id].ContainsKey(accessoryNumber))
                {
                    accessoryNumber++;
                }
                else
                {
                    break;
                }
            }

            pool[slot.Id][accessoryNumber] = id;
            return accessoryNumber;
        }
    }
}
