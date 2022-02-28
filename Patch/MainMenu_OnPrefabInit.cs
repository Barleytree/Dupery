using Dupery.Menu;
using HarmonyLib;
using UnityEngine;

namespace Dupery.Patch
{
    [HarmonyPatch(typeof(MainMenu))]
    [HarmonyPatch("OnPrefabInit")]
    class MainMenu_OnPrefabInit
    {
        private static GameObject screenObject;

        [HarmonyPostfix]
        static void PostFix(ref MainMenu __instance, ref ColorStyleSetting ___normalButtonStyle, KButton ___buttonPrefab, GameObject ___buttonParent)
        {
            DuperyScreen.defaultButtonStyle = ___normalButtonStyle;

            screenObject = PrepareMenu();
            MakeButton(new ButtonInfo("Pool Editor", new System.Action(PoolEditor), 14, ___normalButtonStyle), ___buttonPrefab, ___buttonParent);
        }

        private static GameObject PrepareMenu()
        {
            GameObject screenObj = new GameObject("PoolEditorMenu");
            PoolEditorScreen poolEditorScreen = screenObj.AddComponent<PoolEditorScreen>();
            return poolEditorScreen.gameObject;
        }

        private static KButton MakeButton(ButtonInfo info, KButton buttonPrefab, GameObject buttonParent)
        {
            KButton kbutton = Util.KInstantiateUI<KButton>(buttonPrefab.gameObject, buttonParent, true);
            kbutton.onClick += info.action;
            KImage component = kbutton.GetComponent<KImage>();
            component.colorStyleSetting = info.style;
            component.ApplyColorStyleSetting();
            LocText componentInChildren = kbutton.GetComponentInChildren<LocText>();
            componentInChildren.text = (string)info.text;
            componentInChildren.fontSize = (float)info.fontSize;

            return kbutton;
        }

        private static void PoolEditor()
        {
            GameObject screenObj = new GameObject("PoolEditorMenu");
            PoolEditorScreen poolEditorScreen = screenObj.AddComponent<PoolEditorScreen>();
            poolEditorScreen.prefabReady = true;
            Util.KInstantiateUI<PoolEditorScreen>(screenObj, MainMenu.Instance.transform.parent.gameObject);
        }

        private struct ButtonInfo
        {
            public LocString text;
            public System.Action action;
            public int fontSize;
            public ColorStyleSetting style;

            public ButtonInfo(LocString text, System.Action action, int font_size, ColorStyleSetting style)
            {
                this.text = text;
                this.action = action;
                this.fontSize = font_size;
                this.style = style;
            }
        }
    }
}
