using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dupery
{
    class PersonalityOutline
    {
        [JsonProperty]
        public bool Enabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Gender { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PersonalityType { get; set; } // Doesn't seem to do anything
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StressTrait { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string JoyTrait { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StickerType { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HeadShape { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Mouth { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Eyes { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Hair { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Body { get; set; }

        public PersonalityOutline() { }

        public void OverrideValues(PersonalityOutline overridingPersonality)
        {
            PersonalityOutline p = overridingPersonality;

            Enabled = p.Enabled;
            if (p.Name != null) Name = p.Name;
            if (p.Description != null) Description = p.Description;
            if (p.Gender != null) Gender = p.Gender;
            if (p.PersonalityType != null) PersonalityType = p.PersonalityType;
            if (p.StressTrait != null) StressTrait = p.StressTrait;
            if (p.JoyTrait != null) JoyTrait = p.JoyTrait;
            if (p.StickerType != null) StickerType = p.StickerType;
            if (p.HeadShape != null) HeadShape = p.HeadShape;
            if (p.Mouth != null) Mouth = p.Mouth;
            if (p.Eyes != null) Eyes = p.Eyes;
            if (p.Hair != null) Hair = p.Hair;
            if (p.Body != null) Body = p.Body;
        }

        public Personality toPersonality(string nameStringKey)
        {
            // Meaningless attributes
            string congenitalTrait = "None";
            int neck = -1;

            // Fill missing values
            string name = Name != null ? Name : "The Nameless One";
            string description = Description != null ? Description : "{0} defies description.";
            string gender = Gender != null ? Gender : PersonalityGenerator.rollGender();
            string personalityType = PersonalityType != null ? PersonalityType : PersonalityGenerator.rollPersonalityType();
            string stressTrait = StressTrait != null ? StressTrait : PersonalityGenerator.rollStressTrait();
            string joyTrait = JoyTrait != null ? JoyTrait : PersonalityGenerator.rollJoyTrait();
            string stickerType = "";
            if (joyTrait == "StickerBomber")
            {
                stickerType = StickerType;
                if (stickerType == null | stickerType == "")
                    stickerType = PersonalityGenerator.rollStickerType();
            }

            // Localizable attributes
            StringEntry result;
            name = Strings.TryGet(new StringKey(name), out result) ? result.ToString() : name;
            description = Strings.TryGet(new StringKey(description), out result) ? result.ToString() : description;

            // Uncustomisable accessories
            int headShape = chooseAccessoryNumber(Db.Get().AccessorySlots.HeadShape, HeadShape);
            int mouth = Mouth == null ? headShape : chooseAccessoryNumber(Db.Get().AccessorySlots.Mouth, Mouth);
            int eyes = chooseAccessoryNumber(Db.Get().AccessorySlots.Eyes, Eyes);

            // Customisable accessories
            int hair = chooseAccessoryNumber(Db.Get().AccessorySlots.Hair, Hair);
            int body = chooseAccessoryNumber(Db.Get().AccessorySlots.Body, Body);

            Personality personality = new Personality(
                nameStringKey.ToUpper(),
                name,
                gender.ToUpper(),
                personalityType,
                stressTrait,
                joyTrait,
                stickerType,
                congenitalTrait,
                headShape,
                mouth,
                neck,
                eyes,
                hair,
                body,
                description
            );

            return personality;
        }

        public static PersonalityOutline fromStockPersonality(Personality personality)
        {
            string name = string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.NAME", personality.nameStringKey.ToUpper());
            string description = string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.DESC", personality.nameStringKey.ToUpper());

            PersonalityOutline jsonPersonality = new PersonalityOutline
            {
                Enabled = true,
                Name = name,
                Description = description,
                Gender = personality.genderStringKey,
                PersonalityType = personality.personalityType,
                StressTrait = personality.stresstrait,
                JoyTrait = personality.joyTrait,
                StickerType = personality.stickerType,
                HeadShape = personality.headShape.ToString(),
                Mouth = personality.mouth.ToString(),
                Eyes = personality.eyes.ToString(),
                Hair = personality.hair.ToString(),
                Body = personality.body.ToString()
            };

            return jsonPersonality;
        }

        private static int chooseAccessoryNumber(AccessorySlot slot, string value)
        {
            int accessoryNumber;

            if (value == null)
            {
                accessoryNumber = PersonalityGenerator.rollAccessory(slot);
            }
            else
            {
                int.TryParse(value, out accessoryNumber);
                accessoryNumber = accessoryNumber > 0 ? accessoryNumber : DuperyPatches.AccessoryManager.GetAccessoryNumber(slot, value);
            }

            return accessoryNumber;
        }
    }
}
