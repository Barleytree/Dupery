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
        public AccessoryPool Pool { get { return this.accessoryPool; } }

        private const string ID_CACHE_FILE_NAME = "accessory_id_cache.json";

        private AccessoryPool accessoryPool;

        public AccessoryManager()
        {
            string idCacheFilePath = Path.Combine(DuperyPatches.DirectoryName, ID_CACHE_FILE_NAME);
            accessoryPool = new AccessoryPool(idCacheFilePath);
        }

        public int GetAccessoryNumber(AccessorySlot slot, string accessoryId)
        {
            return accessoryPool.GetAccessoryNumber(slot, accessoryId);
        }

        public string TryGetAccessoryId(AccessorySlot slot, int accessoryNumber)
        {
            if (accessoryPool.IsNativeAccessory(slot, accessoryNumber))
            {
                return null;
            }

            return accessoryPool.GetId(slot, accessoryNumber);
        }

        public int LoadAccessories(string animName, bool saveToCache = false)
        {
            ResourceSet accessories = Db.Get().Accessories;

            KAnimFile anim = Assets.GetAnim(animName);
            KAnim.Build build = anim.GetData().build;

            int numLoaded = 0;
            int numCached = 0;
            for (int index = 0; index < build.symbols.Length; ++index)
            {
                string id = HashCache.Get().Get(build.symbols[index].hash);

                AccessorySlot slot;
                bool cachable = true;

                if (id.StartsWith("hair_"))
                {
                    slot = Db.Get().AccessorySlots.Hair;
                }
                else if (id.StartsWith("hat_hair_"))
                {
                    slot = Db.Get().AccessorySlots.HatHair;
                    cachable = false;
                }
                else if (id.StartsWith("body_"))
                {
                    slot = Db.Get().AccessorySlots.Body;
                }
                else if (id.StartsWith("arm_"))
                {
                    slot = Db.Get().AccessorySlots.Arm;
                }
                else
                {
                    continue;
                }

                Accessory accessory = new Accessory(id, accessories, slot, anim.batchTag, build.symbols[index]);
                slot.accessories.Add(accessory);
                Db.Get().ResourceTable.Add(accessory);

                if (cachable && saveToCache)
                {
                    bool saved = accessoryPool.TrySaveId(slot, id);
                    if (saved)
                        numCached++;
                }

                numLoaded++;
            }

            if (numCached > 0)
                Logger.Log($"Added {numCached} new accessories IDs to the cache.");

            return numLoaded;
        }
    }
}
