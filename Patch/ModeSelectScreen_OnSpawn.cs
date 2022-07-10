using Dupery.Menu;
using System;
using System.Collections;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Dupery.Patch
{
    [HarmonyPatch(typeof(ModeSelectScreen))]
    [HarmonyPatch("OnSpawn")]
    class ModeSelectScreen_OnSpawn
    {
        [HarmonyPostfix]
        static void PostFix(KBatchedAnimController ___nosweatAnim, ModeSelectScreen __instance)
        {
            DuperyDebug.LogObjectTree(__instance.gameObject);

            //___nosweatAnim.SwapAnims();

            foreach (KAnimFile animFile in ___nosweatAnim.AnimFiles)
            {
                Logger.Log($"AnimFile: {animFile.name}");
                KAnim.Anim.FrameElement fe = animFile.GetData().GetAnimFrameElement(animFile.GetData().firstElementIndex);
                //fe.
            }
        }
    }
}
