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
        public const string IMPORTED_PERSONALITIES_FILE_NAME = "duplicate.PERSONALITIES.json";

        private const string PERSONALITIES_FILE_NAME = "PERSONALITIES.json";
        private const int MINIMUM_PERSONALITY_COUNT = 4;

        private readonly string personalitiesFilePath;
        private readonly string[] fillerNames = new string[MINIMUM_PERSONALITY_COUNT] {
            "Urpudding", "Butterella", "Oozetato", "Jellyman" };
        private readonly string fillerDescription = "{0} appeared because there was nobody else to print!";

        private List<PersonalityOutline> storedPersonalities;
        private List<PersonalityOutline> importedPersonalities;
        private bool newFileNeeded = false;

        public PersonalityManager()
        {
            this.personalitiesFilePath = Path.Combine(DuperyPatches.DirectoryName, PERSONALITIES_FILE_NAME);

            this.storedPersonalities = new List<PersonalityOutline>();
            this.importedPersonalities = new List<PersonalityOutline>();

            TryFetchPersonalities();
        }

        public List<Personality> GetPersonalities()
        {
            // Fetch stored personalities now if it hasn't already happened
            if (this.newFileNeeded)
            {
                InitializeDefaultPersonalityFile();
                FetchPersonalities();

                this.newFileNeeded = false;
            }

            List<Personality> personalities = new List<Personality>();

            foreach (PersonalityOutline jsonPersonality in this.storedPersonalities)
                personalities.Add(jsonPersonality.toPersonality());

            foreach (PersonalityOutline jsonPersonality in this.importedPersonalities)
                personalities.Add(jsonPersonality.toPersonality());

            while (personalities.Count < MINIMUM_PERSONALITY_COUNT)
            {
                string name = this.fillerNames[personalities.Count];
                Personality fillerPersonality = PersonalityGenerator.randomPersonality(name, this.fillerDescription);

                Debug.Log($"Not enough personalities, adding {name} to personality pool.");
                personalities.Add(fillerPersonality);
            }

            return personalities;
        }

        public static IEnumerable<PersonalityOutline> ReadPersonalities(string personalitiesFilePath)
        {
            IEnumerable<PersonalityOutline> jsonPersonalities;
            using (StreamReader streamReader = new StreamReader(personalitiesFilePath))
                jsonPersonalities = JsonConvert.DeserializeObject<IEnumerable<PersonalityOutline>>(streamReader.ReadToEnd());

            return jsonPersonalities;
        }

        public static void WritePersonalities(string personalitiesFilePath, IEnumerable<PersonalityOutline> jsonPersonalities)
        {
            using (StreamWriter streamWriter = new StreamWriter(personalitiesFilePath))
            {
                string json = JsonConvert.SerializeObject(jsonPersonalities, Formatting.Indented);
                streamWriter.Write(json);
            }
        }

        private void TryFetchPersonalities()
        {
            if (!File.Exists(this.personalitiesFilePath))
            {
                Debug.Log($"{PERSONALITIES_FILE_NAME} not found, a fresh one will be generated.");
                this.newFileNeeded = true;
                return;
            }
            else
            {
                FetchPersonalities();
            }
        }

        private void FetchPersonalities()
        {
            Debug.Log($"Reading personalities from {PERSONALITIES_FILE_NAME}...");
            IEnumerable<PersonalityOutline> jsonPersonalities = ReadPersonalities(this.personalitiesFilePath);
            foreach (PersonalityOutline jsonPersonality in jsonPersonalities)
                this.storedPersonalities.Add(jsonPersonality);

            Debug.Log($"Personalities fetched.");
        }

        private void InitializeDefaultPersonalityFile()
        {
            List<PersonalityOutline> dbPersonalities = new List<PersonalityOutline>();

            int personalitiesCount = Db.Get().Personalities.Count;
            for (int i = 0; i < personalitiesCount; i++)
            {
                Personality dbPersonality = Db.Get().Personalities[i];
                dbPersonalities.Add(PersonalityOutline.fromPersonality(dbPersonality));
            }

            Debug.Log($"Writing initial {personalitiesCount} personalities file...");
            WritePersonalities(this.personalitiesFilePath, dbPersonalities);
            Debug.Log($"Default personalities file initialized.");
        }

        public void TryImportPersonalities(string importFilePath)
        {
            List<PersonalityOutline> jsonPersonalities = ReadPersonalities(importFilePath).ToList();
            foreach (PersonalityOutline jsonPersonality in jsonPersonalities)
            {
                this.importedPersonalities.Add(jsonPersonality);
            }

            Debug.Log($"{jsonPersonalities.Count} personalities imported successfully.");
        }
    }
}
