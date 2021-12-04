using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dupery
{
    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch("Initialize")]
    internal class Db_Initialize
    {
        public static void Postfix()
        {
            // Import resources from other mods (which should now be loaded), if they are activated
            DuperyPatches.LoadResources();

            Debug.Log($"Preparing personality pool...");
            int totalPersonalityCount = DuperyPatches.PersonalityManager.CountPersonalities();
            List<Personality> personalities = DuperyPatches.PersonalityManager.GetPersonalities();
            Debug.Log($"Printing has been disabled for {totalPersonalityCount - personalities.Count} personalities");
            Debug.Log($"Using {personalities.Count}/{totalPersonalityCount} personalities");

            while (personalities.Count < PersonalityManager.MINIMUM_PERSONALITY_COUNT)
            {
                string name = string.Format("SUBSTIDUPE-{0}{1:00000}", personalities.Count + 1, UnityEngine.Random.Range(0, 100000));
                if (!DuperyPatches.Localizer.TryGet("STRINGS.BAD_DUPLICANT_DESCRIPTION", out string description))
                    description = STRINGS.BAD_DUPLICANT_DESCRIPTION;
                Personality substitutePersonality = PersonalityGenerator.randomPersonality(name, description);

                Debug.Log($"Not enough personalities, adding {name} to pool.");
                personalities.Add(substitutePersonality);
            }

            // Remove all personalities
            Db.Get().Personalities = new Personalities();
            int originalPersonalitiesCount = Db.Get().Personalities.Count;
            for (int i = 0; i < originalPersonalitiesCount; i++)
            {
                Db.Get().Personalities.Remove(Db.Get().Personalities[0]);
            }

            // Add fresh personalities
            foreach (Personality personality in personalities)
            {
                Db.Get().Personalities.Add(personality);
            }
        }
    }
}
