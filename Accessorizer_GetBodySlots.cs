using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dupery
{
    [HarmonyPatch(typeof(Accessorizer))]
    [HarmonyPatch("GetBodySlots")]
    internal class Accessorizer_GetBodySlots
    {
        [HarmonyPostfix]
        static void PostFix(ref KCompBuilder.BodyData fd)
        {
            
        }
    }
}
