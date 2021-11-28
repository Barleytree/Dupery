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

            HashedString headShapeId = FindNewId(Db.Get().AccessorySlots.HeadShape, p.headShape);
            if (headShapeId != null)
                __result.headShape = headShapeId;

            HashedString mouthId = FindNewId(Db.Get().AccessorySlots.Mouth, p.mouth);
            if (mouthId != null)
                __result.mouth = mouthId;

            HashedString eyesId = FindNewId(Db.Get().AccessorySlots.Eyes, p.eyes);
            if (eyesId != null)
                __result.eyes = eyesId;
        }

        private static HashedString FindNewId(AccessorySlot slot, int accessoryNumber)
        {
            string id = DuperyPatches.AccessoryManager.TryGetAccessoryId(slot, accessoryNumber);
            if (id != null)
                return HashCache.Get().Add(id);
            else
                return null;
        }
    }
}
