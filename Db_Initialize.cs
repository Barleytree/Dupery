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
            var personalityMaps = new List<Dictionary<string, PersonalityOutline>>
            {
                DuperyPatches.PersonalityManager.StockPersonalities,
                DuperyPatches.PersonalityManager.CustomPersonalities
            };
            personalityMaps.AddRange(DuperyPatches.PersonalityManager.ImportedPersonalities.Values);

            // Convert the PersonalityOutline objects to Personality objects
            var personalities = new List<Personality>();
            var poolNames = new List<string>();
            int rejectCount = 0;
            foreach (Dictionary<string, PersonalityOutline> map in personalityMaps)
            {
                foreach (string nameStringKey in map.Keys)
                {
                    PersonalityOutline outline = map[nameStringKey];
                    Personality personality = outline.ToPersonality(nameStringKey);

                    string name = $"{personality.Name}";
                    string sourceModId = outline.GetSourceModId();
                    if (sourceModId != null)
                        name = $"{name} [{sourceModId}]";
                    if (outline.IsModified())
                        name = $"{name} [MODIFIED]";
                    if (outline.Randomize)
                        name = $"{name} [RANDOMIZED]";

                    if (outline.Printable)
                    {
                        personalities.Add(personality);
                        poolNames.Add(name);
                    }
                    else
                    {
                        rejectCount++;
                    }
                }
            }

            // Just logging stuff
            string poolReport = string.Join("\n", poolNames);
            Logger.Log($"Pool contains {poolNames.Count} personalities:\n{poolReport}");
            if (rejectCount > 0)
                Logger.Log($"{rejectCount} personalities have the property \"Printable = false\" and wont be used.");

            // Add random personalities if there aren't enough in the pool
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
