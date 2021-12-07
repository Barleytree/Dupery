﻿using Newtonsoft.Json;
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

        // Extra not-serlialized properties
        private string sourceModId;
        private bool isModified;

        public PersonalityOutline() { }

        public void OverrideValues(PersonalityOutline overridingPersonality)
        {
            PersonalityOutline p = overridingPersonality;

            if (p == this)
                isModified = true;

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
            nameStringKey = nameStringKey.ToUpper();

            // Meaningless attributes
            string congenitalTrait = "None";
            int neck = -1;

            // Name can't be null
            string name = Name;
            if (name == null)
                if (!DuperyPatches.Localizer.TryGet("STRINGS.MISSING_DUPLICANT_NAME", out name))
                    name = STRINGS.MISSING_DUPLICANT_NAME;

            // Description can't be null
            string description = Description;
            if (description == null)
                if (!DuperyPatches.Localizer.TryGet("STRINGS.MISSING_DUPLICANT_DESCRIPTION", out description))
                    description = STRINGS.MISSING_DUPLICANT_DESCRIPTION;

            // Fill in other missing values using randomness
            string gender = Gender != null ? Gender : PersonalityGenerator.RollGender();
            gender = gender.ToUpper();
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
            if (name != null)
                name = Strings.TryGet(new StringKey(name), out result) ? result.ToString() : name;
            if (description != null)
                description = Strings.TryGet(new StringKey(description), out result) ? result.ToString() : description;

            string localizedName = null;
            string localizedDescription = null;
            if (sourceModId != null)
            {
                DuperyPatches.ModLocalizers[sourceModId].TryGet($"{nameStringKey}.NAME", out localizedName);
                DuperyPatches.ModLocalizers[sourceModId].TryGet($"{nameStringKey}.DESCRIPTION", out localizedDescription);
            }

            name = localizedName != null ? localizedName : name;
            description = localizedDescription != null ? localizedDescription : description;

            // Uncustomisable accessories
            int headShape = ChooseAccessoryNumber(Db.Get().AccessorySlots.HeadShape, HeadShape);
            int mouth = Mouth == null ? headShape : ChooseAccessoryNumber(Db.Get().AccessorySlots.Mouth, Mouth);
            int eyes = ChooseAccessoryNumber(Db.Get().AccessorySlots.Eyes, Eyes);

            // Customisable accessories
            int hair = ChooseAccessoryNumber(Db.Get().AccessorySlots.Hair, Hair);
            int body = ChooseAccessoryNumber(Db.Get().AccessorySlots.Body, Body);

            Personality personality = new Personality(
                nameStringKey,
                name,
                gender,
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

        public void SetSourceModId(string sourceModId)
        {
            this.sourceModId = sourceModId;
        }

        public string GetSourceModId()
        {
            return this.sourceModId;
        }

        public bool IsModified()
        {
            return this.isModified;
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
