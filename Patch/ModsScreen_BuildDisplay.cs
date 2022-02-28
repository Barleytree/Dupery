using Dupery.Menu;
using System;
using System.Collections;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Dupery.Menu;

namespace Dupery.Patch
{
    [HarmonyPatch(typeof(ModsScreen))]
    [HarmonyPatch("BuildDisplay")]
    class ModsScreen_BuildDisplay
    {
        [HarmonyPostfix]
        static void PostFix(Transform ___entryParent, ModsScreen __instance, IList ___displayedMods)
        {
            
        }
    }

    public struct DisplayedMod
    {
        public RectTransform rect_transform;
        public int mod_index;
    }
}
