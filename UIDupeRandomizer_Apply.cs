using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dupery
{
    [HarmonyPatch(typeof(UIDupeRandomizer))]
    [HarmonyPatch("Apply")]
    internal class UIDupeRandomizer_Apply
    {
        [HarmonyPostfix]
        static void Apply(Database.AccessorySlots ___slots, KBatchedAnimController dupe, Personality personality)
        {
            KCompBuilder.BodyData bodyData = MinionStartingStats.CreateBodyData(personality);
            AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.Hair, personality.hair, bodyData.hair);
            AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.Body, personality.body, bodyData.body);
            AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.Arm, personality.body, bodyData.arms);
        }

        private static void AddAccessoryIfMissing(KBatchedAnimController dupe, AccessorySlot slot, int accessoryNumber, HashedString accessoryId)
        {
            if (!DuperyPatches.AccessoryManager.Pool.IsNativeAccessory(slot.Id, accessoryNumber))
            {
                Accessory accessory = slot.accessories.Find((Predicate<Accessory>)(a => a.IdHash == accessoryId));
                if (accessory == null)
                    accessory = slot.accessories[0];

                UIDupeRandomizer.AddAccessory(dupe, accessory);
            }
        }
    }
}
