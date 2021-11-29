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
        public int HeadShape { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Mouth { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Eyes { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Hair { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Body { get; set; }

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
            //if (p.HeadShape != null) HeadShape = p.HeadShape;
            //if (p.Mouth != null) Mouth = p.Mouth;
            //if (p.Eyes != null) Eyes = p.Eyes;
            //if (p.Hair != null) Hair = p.Hair;
            //if (p.Body != null) Body = p.Body;
        }

        public Personality toPersonality(string nameStringKey)
        {
            // Meaningless attributes
            string congenitalTrait = "None";
            int neck = -1;

            // Localizable attributes
            StringEntry result;
            string name = Strings.TryGet(new StringKey(Name), out result) ? result.ToString() : Name;
            string description = Strings.TryGet(new StringKey(Description), out result) ? result.ToString() : Description;

            Personality personality = new Personality(
                nameStringKey.ToUpper(),
                name,
                this.Gender.ToUpper(),
                this.PersonalityType,
                this.StressTrait,
                this.JoyTrait,
                this.StickerType,
                congenitalTrait,
                this.HeadShape,
                this.Mouth,
                neck,
                this.Eyes,
                this.Hair,
                this.Body,
                description
            );

            return personality;
        }

        public static PersonalityOutline fromPersonality(Personality personality)
        {
            string name = string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.NAME", personality.nameStringKey.ToUpper());
            string description = string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.DESC", personality.nameStringKey.ToUpper());

            PersonalityOutline jsonPersonality = new PersonalityOutline
            {
                Name = name,
                Description = description,
                Gender = personality.genderStringKey,
                PersonalityType = personality.personalityType,
                StressTrait = personality.stresstrait,
                JoyTrait = personality.joyTrait,
                StickerType = personality.stickerType,
                HeadShape = personality.headShape,
                Mouth = personality.mouth,
                Eyes = personality.eyes,
                Hair = personality.hair,
                Body = personality.body
            };

            return jsonPersonality;
        }
    }
}
