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

            List<Personality> personalities = DuperyPatches.PersonalityManager.GetPersonalities();
            Debug.Log($"Found {personalities.Count} personalities in total.");

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
                Debug.Log($"Loaded {personality.Name}");
            }
        }
    }
}
