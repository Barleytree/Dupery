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

        private Dictionary<string, Dictionary<int, string>> pool;
        private Dictionary<string, int> startingAccessoryNumbers;
        private Dictionary<string, string> missingAccessoryIds;

        private string idCacheFilePath;

        public AccessoryPool(string idCacheFilePath)
        {
            this.idCacheFilePath = idCacheFilePath;
            if (!File.Exists(idCacheFilePath))
            {
                pool = new Dictionary<string, Dictionary<int, string>>
                {
                    { "Hair", new Dictionary<int, string>() },
                    { "Body", new Dictionary<int, string>() },
                    { "Arm", new Dictionary<int, string>() }
                };
            }
            else
            {
                LoadPool();
            }

            startingAccessoryNumbers = new Dictionary<string, int>
            {
                { "Hair", Db.Get().AccessorySlots.Hair.accessories.Count + 1 },
                { "Body", Db.Get().AccessorySlots.Body.accessories.Count + 1 },
                { "Arm", Db.Get().AccessorySlots.Body.accessories.Count + 1 }
            };

            missingAccessoryIds = new Dictionary<string, string>
            {
                { "Hair", $"hair_{MISSING_ACCESSORY_ID}" },
                { "Body", $"body_{MISSING_ACCESSORY_ID}" },
                { "Arm", $"arm_{MISSING_ACCESSORY_ID}" }
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

        public bool ContainsId(AccessorySlot slot, string id)
        {
            return GetAccessoryNumber(slot, id) > 0;
        }

        public bool TrySaveId(AccessorySlot slot, string id)
        {
            if (ContainsId(slot, id))
            {
                return false;
            }

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
            SavePool();
            return true;
        }

        private void LoadPool()
        {
            using (StreamReader streamReader = new StreamReader(idCacheFilePath))
            {
                pool = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, string>>>(streamReader.ReadToEnd());
            }
        }

        private void SavePool()
        {
            using (StreamWriter streamWriter = new StreamWriter(idCacheFilePath))
            {
                string json = JsonConvert.SerializeObject(pool, Formatting.Indented);
                streamWriter.Write(json);
            }
        }
    }
}
