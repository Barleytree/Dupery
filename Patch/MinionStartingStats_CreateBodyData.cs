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
            HashedString hairId = FindNewId(Db.Get().AccessorySlots.Hair, p.nameStringKey);
            if (hairId != null)
                __result.hair = hairId;

            HashedString headShapeId = FindNewId(Db.Get().AccessorySlots.HeadShape, p.nameStringKey);
            if (headShapeId != null)
                __result.headShape = headShapeId;

            HashedString mouthId = FindNewId(Db.Get().AccessorySlots.Mouth, p.nameStringKey);
            if (mouthId != null)
                __result.mouth = mouthId;

            HashedString eyesId = FindNewId(Db.Get().AccessorySlots.Eyes, p.nameStringKey);
            if (eyesId != null)
                __result.eyes = eyesId;

            HashedString bodyId = FindNewId(Db.Get().AccessorySlots.Body, p.nameStringKey);
            if (bodyId != null)
                __result.body = bodyId;

            HashedString armId = FindNewId(Db.Get().AccessorySlots.Arm, p.nameStringKey);
            if (armId != null)
                __result.arms = armId;
        }

        private static HashedString FindNewId(AccessorySlot slot, string duplicantId)
        {
            string id = DuperyPatches.PersonalityManager.FindOwnedAccessory(duplicantId, slot.Id);
            if (id != null)
                return HashCache.Get().Add(id);
            else
                return null;
        }
    }
}
