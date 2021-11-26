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
        public string Name { get; set; } = "Nameless";
        [JsonProperty]
        public string Description { get; set; } = "This {0} is impeccably out of place.";
        [JsonProperty]
        public string Gender { get; set; } = "NB";
        [JsonProperty]
        public string PersonalityType { get; set; } = "Doofy"; // Doesn't seem to do anything
        [JsonProperty]
        public string StressTrait { get; set; } = "StressVomiter";
        [JsonProperty]
        public string JoyTrait { get; set; } = "StickerBomber";
        [JsonProperty]
        public string StickerType { get; set; } = "glitter";
        [JsonProperty]
        public int HeadShape { get; set; } = 1;
        [JsonProperty]
        public int Mouth { get; set; } = 1;
        [JsonProperty]
        public int Eyes { get; set; } = 1;
        [JsonProperty]
        public int Hair { get; set; } = 1;
        [JsonProperty]
        public int Body { get; set; } = 1;

        public PersonalityOutline() { }

        public Personality toPersonality()
        {
            // Meaningless attributes
            string congenitalTrait = "None";
            int neck = -1;

            Personality personality = new Personality(
                this.Name.ToUpper(),
                this.Name,
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
                this.Description
            );

            return personality;
        }

        public static PersonalityOutline fromPersonality(Personality personality)
        {
            PersonalityOutline jsonPersonality = new PersonalityOutline
            {
                Name = personality.Name,
                Description = personality.unformattedDescription,
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
