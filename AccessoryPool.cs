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
        private Dictionary<string, Dictionary<int, string>> pool;
        private Dictionary<string, int> startingAccessoryNumbers;

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
                { "Hair", Db.Get().AccessorySlots.Hair.accessories.Count },
                { "Body", Db.Get().AccessorySlots.Body.accessories.Count },
                { "Arm", Db.Get().AccessorySlots.Body.accessories.Count }
            };
        }

        public bool IsNativeAccessory(string slotId, int accessoryNumber)
        {
            return accessoryNumber > 0 && accessoryNumber <= startingAccessoryNumbers[slotId];
        }

        public int GetAccessoryNumber(string slotId, string id)
        {
            foreach (KeyValuePair<int, string> entry in pool[slotId])
            {
                if (entry.Value == id)
                {
                    return entry.Key;
                }
            }

            return 0;
        }

        public string GetId(string slotId, int accessoryNumber)
        {
            pool[slotId].TryGetValue(accessoryNumber, out string id);
            return id;
        }

        public bool ContainsId(string slotId, string id)
        {
            return GetAccessoryNumber(slotId, id) > 0;
        }

        public bool TrySaveId(string slotId, string id)
        {
            if (ContainsId(slotId, id))
            {
                return false;
            }

            int accessoryNumber = startingAccessoryNumbers[slotId] + 1;
            while (true)
            {
                if (pool[slotId].ContainsKey(accessoryNumber))
                {
                    accessoryNumber++;
                }
                else
                {
                    break;
                }
            }

            pool[slotId][accessoryNumber] = id;
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
