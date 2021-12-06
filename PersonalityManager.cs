using KMod;
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
            try
            {
                OverridePersonalities(overrideFilePath, ref stockPersonalities);
            }
            catch (PersonalityLoadException)
            {
                Logger.LogError($"Failed to load {OVERRIDE_FILE_NAME}. Please fix any JSON syntax errors or delete the file.");
            }

            Logger.Log($"Loaded the {stockPersonalities.Count} stock personalities.");

            // Load user created personalities
            string customPersonalitiesFilePath = Path.Combine(DuperyPatches.DirectoryName, PERSONALITIES_FILE_NAME);
            customPersonalities = new Dictionary<string, PersonalityOutline>();
            if (File.Exists(customPersonalitiesFilePath))
            {
                try
                {
                    Logger.Log($"Reading custom personalities from {PERSONALITIES_FILE_NAME}...");
                    customPersonalities = ReadPersonalities(customPersonalitiesFilePath);
                }
                catch (PersonalityLoadException)
                {
                    Logger.LogError($"Failed to load {PERSONALITIES_FILE_NAME}. Please fix any JSON syntax errors or delete the file.");
                }

                if (customPersonalities != null)
                    Logger.Log($"Loaded {customPersonalities.Count} user created personalities.");
            }
            else
            {
                Logger.Log($"{PERSONALITIES_FILE_NAME} not found, a fresh one will be generated.");
                customPersonalities["EXAMPLENAME"] = PersonalityGenerator.ExamplePersonality();
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

        public bool TryImportPersonalities(string importFilePath, Mod mod)
        {
            Dictionary<string, PersonalityOutline> modPersonalities;
            try
            {
                modPersonalities = ReadPersonalities(importFilePath);
            }
            catch (PersonalityLoadException)
            {
                Logger.LogError($"Failed to load {PERSONALITIES_FILE_NAME} file from mod <{mod.title}>. Please fix any JSON syntax errors or delete the file.");
                return false;
            }

            string overrideFilePath = Path.Combine(DuperyPatches.DirectoryName, string.Format(OVERRIDE_IMPORT_FILE_NAME, mod.staticID));
            if (File.Exists(overrideFilePath))
            {
                try
                {
                    OverridePersonalities(overrideFilePath, ref modPersonalities);
                }
                catch (PersonalityLoadException)
                {
                    Logger.LogError($"Failed to load {string.Format(OVERRIDE_IMPORT_FILE_NAME, mod.staticID)}. Please fix any JSON syntax errors or delete the file.");
                }
            }

            foreach (string key in modPersonalities.Keys)
                modPersonalities[key].SetSourceModId(mod.staticID);

            importedPersonalities[mod.staticID] = modPersonalities;
            Logger.Log($"{importedPersonalities.Count} personalities imported from <{mod.title}>.");

            return true;
        }

        public void OverridePersonalities(string overrideFilePath, ref Dictionary<string, PersonalityOutline> personalities)
        {
            Dictionary<string, PersonalityOutline> currentOverrides = ReadPersonalities(overrideFilePath);
            
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

            try
            {
                using (StreamReader streamReader = new StreamReader(personalitiesFilePath))
                    jsonPersonalities = JsonConvert.DeserializeObject<Dictionary<string, PersonalityOutline>>(streamReader.ReadToEnd());
            }
            catch (Exception)
            {
                throw new PersonalityLoadException();
            }

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
