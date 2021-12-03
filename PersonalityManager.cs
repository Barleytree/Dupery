using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dupery
{
    class PersonalityManager
    {
        public const string PERSONALITIES_FILE_NAME = "PERSONALITIES.json";
        public const string OVERRIDE_FILE_NAME = "OVERRIDE.json";
        public const string OVERRIDE_IMPORT_FILE_NAME = "OVERRIDE.{0}.json";

        public const int MINIMUM_PERSONALITY_COUNT = 4;

        private Dictionary<string, PersonalityOutline> stockPersonalities;
        private Dictionary<string, PersonalityOutline> customPersonalities;
        private Dictionary<string, Dictionary<string, PersonalityOutline>> importedPersonalities;

        public PersonalityManager()
        {
            // Load stock personalities
            stockPersonalities = new Dictionary<string, PersonalityOutline>();

            int personalitiesCount = Db.Get().Personalities.Count;
            for (int i = 0; i < personalitiesCount; i++)
            {
                Personality dbPersonality = Db.Get().Personalities[i];
                stockPersonalities[dbPersonality.nameStringKey] = PersonalityOutline.FromStockPersonality(dbPersonality);
            }

            string overrideFilePath = Path.Combine(DuperyPatches.DirectoryName, OVERRIDE_FILE_NAME);
            OverridePersonalities(overrideFilePath, ref stockPersonalities);

            Debug.Log($"Loaded the {stockPersonalities.Count} stock personalities.");

            // Load user created personalities
            string customPersonalitiesFilePath = Path.Combine(DuperyPatches.DirectoryName, PERSONALITIES_FILE_NAME);
            if (File.Exists(customPersonalitiesFilePath))
            {
                Debug.Log($"Reading custom personalities from {PERSONALITIES_FILE_NAME}...");
                customPersonalities = ReadPersonalities(customPersonalitiesFilePath);
                Debug.Log($"Loaded {customPersonalities.Count} user created personalities.");
            }
            else
            {
                Debug.Log($"{PERSONALITIES_FILE_NAME} not found, a fresh one will be generated.");
                customPersonalities = new Dictionary<string, PersonalityOutline>();
                customPersonalities["UNIQUEKEY"] = PersonalityGenerator.ExamplePersonality();
                WritePersonalities(customPersonalitiesFilePath, customPersonalities);
            }

            // Prepare for imported personalities
            this.importedPersonalities = new Dictionary<string, Dictionary<string, PersonalityOutline>>();
        }

        public List<Personality> GetPersonalities()
        {
            List<Personality> personalities = new List<Personality>();

            personalities.AddRange(FlattenPersonalities(stockPersonalities));
            personalities.AddRange(FlattenPersonalities(customPersonalities));

            foreach (string key in importedPersonalities.Keys)
            {
                Dictionary<string, PersonalityOutline> personalityMap = importedPersonalities[key];
                personalities.AddRange(FlattenPersonalities(personalityMap));
            }

            return personalities;
        }

        public int CountPersonalities()
        {
            int count = stockPersonalities.Count + customPersonalities.Count;
            foreach (Dictionary<string, PersonalityOutline> value in importedPersonalities.Values)
                count += value.Count;

            return count;
        }

        public void TryImportPersonalities(string importFilePath, string modId)
        {
            Dictionary<string, PersonalityOutline> modPersonalities = ReadPersonalities(importFilePath);

            string overrideFilePath = Path.Combine(DuperyPatches.DirectoryName, string.Format(OVERRIDE_IMPORT_FILE_NAME, modId));
            OverridePersonalities(overrideFilePath, ref modPersonalities);

            importedPersonalities[modId] = modPersonalities;
            Debug.Log($"{importedPersonalities.Count} personalities loaded successfully.");
        }

        public void OverridePersonalities(string overrideFilePath, ref Dictionary<string, PersonalityOutline> personalities)
        {
            Dictionary<string, PersonalityOutline> currentOverrides = null;
            if (File.Exists(overrideFilePath))
            {
                currentOverrides = ReadPersonalities(overrideFilePath);
            }

            Dictionary<string, PersonalityOutline> newOverrides = new Dictionary<string, PersonalityOutline>();
            foreach (string key in personalities.Keys)
            {
                PersonalityOutline overridingPersonality = null;
                if (currentOverrides != null)
                    currentOverrides.TryGetValue(key, out overridingPersonality);

                if (overridingPersonality != null)
                {
                    personalities[key].OverrideValues(overridingPersonality);
                    newOverrides[key] = overridingPersonality;
                }
                else
                {
                    newOverrides[key] = new PersonalityOutline { Printable = personalities[key].Printable };
                }
            }

            WritePersonalities(overrideFilePath, newOverrides);
        }

        public static Dictionary<string, PersonalityOutline> ReadPersonalities(string personalitiesFilePath)
        {
            Dictionary<string, PersonalityOutline> jsonPersonalities;
            using (StreamReader streamReader = new StreamReader(personalitiesFilePath))
                jsonPersonalities = JsonConvert.DeserializeObject<Dictionary<string, PersonalityOutline>>(streamReader.ReadToEnd());

            return jsonPersonalities;
        }

        public static void WritePersonalities(string personalitiesFilePath, Dictionary<string, PersonalityOutline> jsonPersonalities)
        {
            using (StreamWriter streamWriter = new StreamWriter(personalitiesFilePath))
            {
                string json = JsonConvert.SerializeObject(jsonPersonalities, Formatting.Indented);
                streamWriter.Write(json);
            }
        }

        private List<Personality> FlattenPersonalities(Dictionary<string, PersonalityOutline> personalities)
        {
            List<Personality> flattenedPersonalities = new List<Personality>();

            foreach (string key in personalities.Keys)
                if (personalities[key].Printable)
                    flattenedPersonalities.Add(personalities[key].ToPersonality(key));

            return flattenedPersonalities;
        }
    }
}
