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

            Logger.Log($"Preparing personality pool...");
            int totalPersonalityCount = DuperyPatches.PersonalityManager.CountPersonalities();
            List<Personality> personalities = DuperyPatches.PersonalityManager.GetPersonalities();
            Logger.Log($"Printing has been disabled for {totalPersonalityCount - personalities.Count} personalities");
            Logger.Log($"Using {personalities.Count}/{totalPersonalityCount} personalities");

            while (personalities.Count < PersonalityManager.MINIMUM_PERSONALITY_COUNT)
            {
                Personality substitutePersonality = PersonalityGenerator.RandomPersonality();

                Logger.Log($"Not enough personalities, adding {substitutePersonality.Name} to pool.");
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
