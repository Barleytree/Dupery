using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Dupery
{
    [HarmonyPatch(typeof(MinionStartingStats))]
    [HarmonyPatch("CreateBodyData")]
    internal class MinionStartingStats_CreateBodyData
    {
        [HarmonyPostfix]
        static void PostFix(ref KCompBuilder.BodyData __result, Personality p)
        {
            HashedString hairId = FindNewId(Db.Get().AccessorySlots.Hair, p.hair);
            if (hairId != null)
                __result.hair = hairId;

            HashedString bodyId = FindNewId(Db.Get().AccessorySlots.Body, p.body);
            if (bodyId != null)
                __result.body = bodyId;

            HashedString armId = FindNewId(Db.Get().AccessorySlots.Arm, p.body);
            if (armId != null)
                __result.arms = armId;
        }

        private static HashedString FindNewId(AccessorySlot slot, int accessoryNumber)
        {
            string id = DuperyPatches.AccessoryManager.TryGetAccessoryId(slot.Id, accessoryNumber);
            if (id != null)
                return HashCache.Get().Add(id);
            else
                return null;
        }
    }
}
