using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;

namespace Dupery
{
    class PersonalityGenerator
    {
        public static string RollGender()
        {
            string[] genders = new string[3] { "Female", "NB", "Male" };
            int randomIndex = UnityEngine.Random.Range(0, genders.Length);
            return genders[randomIndex];
        }

        public static string RollPersonalityType()
        {
            string[] personalityTypes = new string[4] { "Doofy", "Cool", "Grumpy", "Sweet" };
            int randomIndex = UnityEngine.Random.Range(0, personalityTypes.Length);
            return personalityTypes[randomIndex];
        }

        public static string RollStressTrait()
        {
            int randomIndex = UnityEngine.Random.Range(0, DUPLICANTSTATS.STRESSTRAITS.Count);
            return DUPLICANTSTATS.STRESSTRAITS[randomIndex].id;
        }

        public static string RollJoyTrait()
        {
            int randomIndex = UnityEngine.Random.Range(0, DUPLICANTSTATS.JOYTRAITS.Count);
            return DUPLICANTSTATS.JOYTRAITS[randomIndex].id;
        }

        public static string RollStickerType()
        {
            string[] stickerTypes = new string[3] { "sticker", "glitter", "glowinthedark" };
            int randomIndex = UnityEngine.Random.Range(0, stickerTypes.Length);
            return stickerTypes[randomIndex];
        }

        public static int RollAccessory(AccessorySlot accessorySlot)
        {
            return UnityEngine.Random.Range(0, accessorySlot.accessories.Count) + 1;
        }

        public static PersonalityOutline ExamplePersonality()
        {
            if (!DuperyPatches.Localizer.TryGet("STRINGS.EXAMPLE_DUPLICANT_NAME", out string name))
                name = STRINGS.EXAMPLE_DUPLICANT_NAME;
            if (!DuperyPatches.Localizer.TryGet("STRINGS.EXAMPLE_DUPLICANT_DESCRIPTION", out string description))
                description = STRINGS.EXAMPLE_DUPLICANT_DESCRIPTION;

            return new PersonalityOutline()
            {
                Printable = false,
                Name = name,
                Description = description,
                Gender = "NB",
                StressTrait = "BingeEater",
                JoyTrait = "SuperProductive",
                HeadShape = "1",
                Eyes = "1",
                Hair = "1",
                Body = "1",
            };
        }

        public static Personality randomPersonality(string name, string description)
        {
            PersonalityOutline outline = new PersonalityOutline()
            {
                Name = name,
                Description = description
            };
            return outline.ToPersonality(name.ToUpper());
        }
    }
}
