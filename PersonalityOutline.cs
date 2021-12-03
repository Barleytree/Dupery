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
        public bool Printable { get; set; } = true;

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

            Printable = p.Printable;
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

        public Personality ToPersonality(string nameStringKey)
        {
            // Meaningless attributes
            string congenitalTrait = "None";
            int neck = -1;

            // Fill missing values
            string name = Name != null ? Name : "The Nameless One";
            string description = Description != null ? Description : "{0} defies description.";
            string gender = Gender != null ? Gender : PersonalityGenerator.RollGender();
            string personalityType = PersonalityType != null ? PersonalityType : PersonalityGenerator.RollPersonalityType();
            string stressTrait = StressTrait != null ? StressTrait : PersonalityGenerator.RollStressTrait();
            string joyTrait = JoyTrait != null ? JoyTrait : PersonalityGenerator.RollJoyTrait();
            string stickerType = "";
            if (joyTrait == "StickerBomber")
            {
                stickerType = StickerType;
                if (stickerType == null | stickerType == "")
                    stickerType = PersonalityGenerator.RollStickerType();
            }

            // Localizable attributes
            StringEntry result;
            name = Strings.TryGet(new StringKey(name), out result) ? result.ToString() : name;
            description = Strings.TryGet(new StringKey(description), out result) ? result.ToString() : description;

            // Uncustomisable accessories
            int headShape = ChooseAccessoryNumber(Db.Get().AccessorySlots.HeadShape, HeadShape);
            int mouth = Mouth == null ? headShape : ChooseAccessoryNumber(Db.Get().AccessorySlots.Mouth, Mouth);
            int eyes = ChooseAccessoryNumber(Db.Get().AccessorySlots.Eyes, Eyes);

            // Customisable accessories
            int hair = ChooseAccessoryNumber(Db.Get().AccessorySlots.Hair, Hair);
            int body = ChooseAccessoryNumber(Db.Get().AccessorySlots.Body, Body);

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

        public static PersonalityOutline FromStockPersonality(Personality personality)
        {
            string name = string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.NAME", personality.nameStringKey.ToUpper());
            string description = string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.DESC", personality.nameStringKey.ToUpper());

            PersonalityOutline jsonPersonality = new PersonalityOutline
            {
                Printable = true,
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

        private static int ChooseAccessoryNumber(AccessorySlot slot, string value)
        {
            int accessoryNumber;

            if (value == null)
            {
                accessoryNumber = PersonalityGenerator.RollAccessory(slot);
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
