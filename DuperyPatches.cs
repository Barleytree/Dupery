using HarmonyLib;
using Klei;
using KMod;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Dupery
{
    internal class DuperyPatches : UserMod2
    {
        public static string ModStaticID;
        public static IReadOnlyList<Mod> Mods;

        public static CheekyLocalizer Localizer;
        public static Dictionary<string, CheekyLocalizer> ModLocalizers;

        public static AccessoryManager AccessoryManager;
        public static PersonalityManager PersonalityManager;

        public static string DirectoryName;

        public override void OnLoad(Harmony harmony)
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            DirectoryName = Path.GetDirectoryName(assemblyLocation);

            base.OnLoad(harmony);
        }

        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<Mod> mods)
        {
            Mods = mods;
            ModStaticID = this.mod.staticID;

            base.OnAllModsLoaded(harmony, mods);
        }

        public static void LoadResources()
        {
            Localizer = new CheekyLocalizer(DirectoryName);
            ModLocalizers = new Dictionary<string, CheekyLocalizer>();

            AccessoryManager = new AccessoryManager();
            PersonalityManager = new PersonalityManager();

            Logger.Log("Searching for personalities and accessories provided by other mods.");

            foreach (Mod mod in Mods)
            {
                if (!mod.IsActive())
                    continue;

                if (mod.staticID == ModStaticID)
                    continue;

                if (mod.content_source == null)
                    continue;

                ModLocalizers[mod.staticID] = new CheekyLocalizer(mod.content_source.GetRoot());

                List<string> animNames = GetAnimNames(mod);
                if (animNames != null && animNames.Count > 0)
                {
                    Logger.Log($"Found anims belonging to mod <{mod.title}>, searching for accessories.");

                    int totalImported = 0;
                    foreach (string animName in animNames)
                    {
                        Logger.Log($"Checking {animName}...");
                        totalImported += AccessoryManager.LoadAccessories(animName, true);
                    }

                    Logger.Log($"{totalImported} accessories imported successfully.");
                }

                string personalitiesFilePath = Path.Combine(mod.content_source.GetRoot(), PersonalityManager.PERSONALITIES_FILE_NAME);
                if (File.Exists(personalitiesFilePath))
                {
                    Logger.Log($"Found {PersonalityManager.PERSONALITIES_FILE_NAME} file belonging to mod <{mod.title}>, attempting to import personalities...");
                    PersonalityManager.TryImportPersonalities(personalitiesFilePath, mod.staticID);
                }
            }
        }

        private static List<string> GetAnimNames(Mod mod)
        {
            List<string> animNames = new List<string>();

            string path = FileSystem.Normalize(System.IO.Path.Combine(mod.ContentPath, "anim"));
            if (!System.IO.Directory.Exists(path))
                return null;

            foreach (DirectoryInfo directory1 in new DirectoryInfo(path).GetDirectories())
            {
                foreach (DirectoryInfo directory2 in directory1.GetDirectories())
                {
                    string name = directory2.Name + "_kanim";
                    foreach (KAnimFile kAnimFile in Assets.ModLoadedKAnims)
                    {
                        if (kAnimFile.name == name)
                        {
                            animNames.Add(name);
                        }
                    }
                }
            }

            return animNames;
        }
    }
}
