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
        public static string rollGender()
        {
            string[] genders = new string[3] { "Female", "NB", "Male" };
            int randomIndex = UnityEngine.Random.Range(0, genders.Length);
            return genders[randomIndex];
        }

        public static string rollPersonalityType()
        {
            string[] personalityTypes = new string[4] { "Doofy", "Cool", "Grumpy", "Sweet" };
            int randomIndex = UnityEngine.Random.Range(0, personalityTypes.Length);
            return personalityTypes[randomIndex];
        }

        public static string rollStressTrait()
        {
            int randomIndex = UnityEngine.Random.Range(0, DUPLICANTSTATS.STRESSTRAITS.Count);
            return DUPLICANTSTATS.STRESSTRAITS[randomIndex].id;
        }

        public static string rollJoyTrait()
        {
            int randomIndex = UnityEngine.Random.Range(0, DUPLICANTSTATS.JOYTRAITS.Count);
            return DUPLICANTSTATS.JOYTRAITS[randomIndex].id;
        }

        public static string rollStickerType()
        {
            string[] stickerTypes = new string[3] { "sticker", "glitter", "glowinthedark" };
            int randomIndex = UnityEngine.Random.Range(0, stickerTypes.Length);
            return stickerTypes[randomIndex];
        }

        public static int rollAccessory(AccessorySlot accessorySlot)
        {
            return UnityEngine.Random.Range(0, accessorySlot.accessories.Count) + 1;
        }

        public static Personality randomPersonality(string name, string description)
        {
            string gender = rollGender();
            string personalityType = rollPersonalityType();

            string stressTrait = rollStressTrait();
            string joyTrait = rollJoyTrait();
            string stickerType = "";
            if (joyTrait == "StickerBomber")
            {
                stickerType = rollStickerType();
            }

            int headShape = rollAccessory(Db.Get().AccessorySlots.HeadShape);
            int mouth = headShape;
            int eyes = rollAccessory(Db.Get().AccessorySlots.Eyes);
            int hair = rollAccessory(Db.Get().AccessorySlots.Hair);
            int body = rollAccessory(Db.Get().AccessorySlots.Body);

            Personality randomPersonality = new Personality(
                name.ToUpper(),
                name,
                gender.ToUpper(),
                personalityType,
                stressTrait,
                joyTrait,
                stickerType,
                "None",
                headShape,
                mouth,
                -1,
                eyes,
                hair,
                body,
                description
            );

            return randomPersonality;
        }
    }
}
