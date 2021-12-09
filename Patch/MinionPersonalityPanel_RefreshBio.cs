using Database;
using HarmonyLib;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dupery.Patch
{
    [HarmonyPatch(typeof(MinionPersonalityPanel))]
    [HarmonyPatch("RefreshBio")]
    internal class MinionPersonalityPanel_RefreshBio
    {
        [HarmonyPostfix]
        static void RefreshBio(DetailsPanelDrawer ___bioDrawer, GameObject ___selectedTarget)
        {
            MinionIdentity component1 = ___selectedTarget.GetComponent<MinionIdentity>();

            ___bioDrawer.BeginDrawing()
                .NewLabel((string)DUPLICANTS.NAMETITLE + component1.name)
                .NewLabel((string)DUPLICANTS.ARRIVALTIME + GameUtil.GetFormattedCycles((float)(((double)GameClock.Instance.GetCycle() - (double)component1.arrivalTime) * 600.0), "F0", true))
                .Tooltip(string.Format((string)DUPLICANTS.ARRIVALTIME_TOOLTIP, (object)(float)((double)component1.arrivalTime + 1.0), (object)component1.name))
                .NewLabel((string)DUPLICANTS.GENDERTITLE + string.Format((string)Strings.Get(string.Format("STRINGS.DUPLICANTS.GENDER.{0}.NAME", (object)component1.genderStringKey.ToUpper())), (object)component1.gender))
                .NewLabel(string.Format(DuperyPatches.PersonalityManager.FindDescription(component1.nameStringKey), (object)component1.name))
                .Tooltip(string.Format((string)Strings.Get(string.Format("STRINGS.DUPLICANTS.DESC_TOOLTIP", (object)component1.nameStringKey.ToUpper())), (object)component1.name));
            MinionResume component2 = ___selectedTarget.GetComponent<MinionResume>();
            if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && component2.AptitudeBySkillGroup.Count > 0)
            {
                ___bioDrawer.NewLabel((string)UI.DETAILTABS.PERSONALITY.RESUME.APTITUDES.NAME + "\n").Tooltip(string.Format((string)UI.DETAILTABS.PERSONALITY.RESUME.APTITUDES.TOOLTIP, (object)___selectedTarget.name));
                foreach (KeyValuePair<HashedString, float> keyValuePair in component2.AptitudeBySkillGroup)
                {
                    if ((double)keyValuePair.Value != 0.0)
                    {
                        SkillGroup skillGroup = Db.Get().SkillGroups.TryGet(keyValuePair.Key);
                        if (skillGroup != null)
                            ___bioDrawer.NewLabel("  • " + skillGroup.Name).Tooltip(string.Format((string)DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION, (object)skillGroup.Name, (object)keyValuePair.Value));
                    }
                }
            }
            ___bioDrawer.EndDrawing();
        }
    }
}
