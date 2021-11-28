using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dupery
{
    [HarmonyPatch(typeof(MinionIdentity))]
    [HarmonyPatch("OnSpawn")]
    internal class MinionIdentity_OnSpawn
    {
        [HarmonyPostfix]
        static void PostFix(ref MinionIdentity __instance)
        {
            SymbolOverrideController component2 = __instance.GetComponent<SymbolOverrideController>();
            Accessorizer component3 = __instance.gameObject.GetComponent<Accessorizer>();

            Accessory headAccessory = component3.GetAccessory(Db.Get().AccessorySlots.HeadShape);
            string cheekHash = HashCache.Get().Get(headAccessory.symbol.hash).Replace("headshape", "cheek");
            foreach (KAnim.Build.Symbol symbol in headAccessory.symbol.build.symbols)
            {
                if (HashCache.Get().Get(symbol.hash) == cheekHash)
                {
                    component2.AddSymbolOverride((HashedString)"snapto_cheek", Assets.GetAnim((HashedString)"head_swap_kanim").GetData().build.GetSymbol((KAnimHashedString)cheekHash), 1);
                }
            }
        }
    }
}
