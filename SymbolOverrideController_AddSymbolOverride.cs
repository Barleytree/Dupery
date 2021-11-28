using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dupery
{
    [HarmonyPatch(typeof(SymbolOverrideController))]
    [HarmonyPatch("AddSymbolOverride")]
    internal class SymbolOverrideController_AddSymbolOverride
    {
        [HarmonyPrefix]
        static bool PreFix(
            ref HashedString target_symbol,
            ref KAnim.Build.Symbol source_symbol,
            int priority = 0)
        {
            string targetId = HashCache.Get().Get(target_symbol);
            if (target_symbol == "snapto_cheek" && source_symbol == null)
            {
                return false;
            }

            return true;
        }
    }
}
