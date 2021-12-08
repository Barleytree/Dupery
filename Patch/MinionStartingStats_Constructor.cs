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
    [HarmonyPatch(MethodType.Constructor)]
    [HarmonyPatch(new System.Type[] { typeof(bool), typeof(string), typeof(string) })]
    internal class MinionStartingStats_Constructor
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            // Instructions for Db.Get().Personalities.Count
            List<CodeInstruction> betterInstructions = new List<CodeInstruction>();
            betterInstructions.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Db), "Get")));
            betterInstructions.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Db), "Personalities")));
            betterInstructions.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(ResourceSet), "get_Count")));

            Logger.Log($"Attempting to force personality randomiser to pick from all available personalities...");
            int successes = 0;
            for (int i = 0; i < code.Count - 1; i++)
            {
                if (code[i].opcode == OpCodes.Ldc_I4_S && code[i + 5].operand.ToString() == "Database.Personalities Personalities")
                {
                    if (code[i].operand.ToString() == "29")
                    {
                        Logger.Log("Removing starting dupe restriction: all personalities will be available when creating a new colony.");
                    }

                    code.RemoveAt(i);
                    code.InsertRange(i, betterInstructions);
                    successes++;
                }

                if (successes == 2)
                {
                    Logger.Log($"Successfully transpiled the personality randomiser!");
                    break;
                }
            }

            if (successes != 2)
            {
                Logger.Log($"Failed to correctly transpile the personality randomiser, some personalities may be unavailable!");
            }

            return code;
        }
    }
}
