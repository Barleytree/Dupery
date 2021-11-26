using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dupery
{
    class AccessoryManager
    {
        private const string ID_CACHE_FILE_NAME = "accessory_id_cache.json";

        private readonly string idCacheFilePath;
        private AccessoryPool accessoryPool;

        public AccessoryManager()
        {
            idCacheFilePath = Path.Combine(DuperyPatches.DirectoryName, ID_CACHE_FILE_NAME);

            if (!File.Exists(idCacheFilePath))
            {
                accessoryPool = new AccessoryPool();
                SaveAccessoryPool();
            }
            else
            {
                LoadAccessoryPool();
            }
        }

        public string TryGetAccessoryId(AccessorySlot slot, int accessoryNumber)
        {
            if (accessoryPool.IsNativeAccessory(slot, accessoryNumber))
            {
                return null;
            }

            string id = accessoryPool.GetId(slot, accessoryNumber);
            if (id == null)
            {
                id = accessoryPool.GetMissingId(slot);
            }

            return id;
        }

        public int TryImportAccessories(string animName)
        {
            ResourceSet accessories = Db.Get().Accessories;

            KAnimFile anim = Assets.GetAnim(animName);
            KAnim.Build build = anim.GetData().build;

            int numImported = 0;
            for (int index = 0; index < build.symbols.Length; ++index)
            {
                string id = HashCache.Get().Get(build.symbols[index].hash);

                AccessorySlot slot = null;

                bool saveToPool = true;
                if (id.StartsWith("hair_"))
                {
                    slot = Db.Get().AccessorySlots.Hair;
                }
                if (id.StartsWith("hat_hair_"))
                {
                    slot = Db.Get().AccessorySlots.HatHair;
                    saveToPool = false;
                }

                Accessory accessory = new Accessory(id, accessories, slot, anim.batchTag, build.symbols[index]);
                slot.accessories.Add(accessory);
                Db.Get().ResourceTable.Add(accessory);

                if (saveToPool)
                {
                    int accessoryNumber = accessoryPool.GetAccessoryNumber(slot, id);
                    if (accessoryNumber > 0)
                    {
                        Debug.Log($"Loaded cached {slot.Name} accessory [{id}] using slot {accessoryNumber}");
                    }
                    else
                    {
                        accessoryNumber = accessoryPool.TryInsertId(slot, id);
                        SaveAccessoryPool();
                        Debug.Log($"Cached new {slot.Name} accessory [{id}] at slot {accessoryNumber}.");
                    }
                }

                numImported++;
            }

            Debug.Log($"Loaded anim {animName}");
            return numImported;
        }

        private void SaveAccessoryPool()
        {
            using (StreamWriter streamWriter = new StreamWriter(idCacheFilePath))
            {
                string json = JsonConvert.SerializeObject(accessoryPool, Formatting.Indented);
                streamWriter.Write(json);
            }
        }

        private void LoadAccessoryPool()
        {
            using (StreamReader streamReader = new StreamReader(idCacheFilePath))
            {
                accessoryPool = JsonConvert.DeserializeObject<AccessoryPool>(streamReader.ReadToEnd());
            }
        }
    }
}
