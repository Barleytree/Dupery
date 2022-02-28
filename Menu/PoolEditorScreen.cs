using KMod;
using ProcGen;
using ProcGenGame;
using UnityEngine;
using UnityEngine.UI;

namespace Dupery.Menu
{
    public class PoolEditorScreen : DuperyScreen
    {
        private KButton closeButton;

        protected override void SetupScreen()
        {
            GameObject background = null;
            foreach (Transform childTransform in this.transform)
            {
                if (childTransform.gameObject.name == "background" && childTransform.parent == this.transform)
                {
                    background = childTransform.gameObject;
                }
            }

            GameObject panel = MakeSprite("Panel", new Vector2(500, 399));
            panel = Instantiate(panel, background.transform);

            GameObject header = MakeSprite("Header", new Vector2(30, 39));
            Instantiate(header, panel.transform);

            GameObject label = MakeLabel("Label", new Vector2(200, 40), "Testing");
            Instantiate(label, background.transform);

            GameObject closeButton = MakeButton("CloseButton", new Vector2(20, 20));
            closeButton = Instantiate(closeButton, background.transform);

            GameObject closeButtonSprite = MakeSprite("CloseButtonSprite", new Vector2(20, 19));
            Instantiate(closeButtonSprite, closeButton.transform);

            this.closeButton = closeButton.GetComponent<KButton>();
            this.closeButton.onClick += new System.Action(((KScreen)this).Deactivate);
        }

        protected override void AfterSpawn()
        {
            Logger.Log("AFTERSPAWN");
        }

        public GameObject MakeUI(string name, Vector2 sizeDelta)
        {
            GameObject obj = new GameObject(name);

            RectTransform rt = obj.AddComponent<RectTransform>();
            rt.sizeDelta = sizeDelta;

            return obj;
        }

        public GameObject MakeCanvas(string name, Vector2 sizeDelta)
        {
            GameObject obj = MakeUI(name, sizeDelta);
            obj.AddComponent<CanvasRenderer>();

            return obj;
        }

        public GameObject MakeSprite(string name, Vector2 sizeDelta, Texture2D tex = null)
        {
            GameObject obj = MakeCanvas(name, sizeDelta);
            //var image = obj.AddComponent<Image>();
            if (tex != null)
            {
                var sr = obj.AddComponent<KImage>();
                sr.color = new Color(0.9f, 0.9f, 0.9f, 1.0f);
                sr.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            }

            return obj;
        }

        public GameObject MakeLabel(string name, Vector2 sizeDelta, string text)
        {
            GameObject obj = MakeCanvas(name, sizeDelta);
            obj.AddComponent<SetTextStyleSetting>();

            //var lt = obj.AddComponent<LocText>();
            //lt.text = "BLABLALA";
            //lt.fontSize = (float)14;

            return obj;
        }

        public GameObject MakeButton(string name, Vector2 sizeDelta, ColorStyleSetting style = null)
        {
            if (style == null)
                style = defaultButtonStyle;

            GameObject obj = MakeCanvas(name, sizeDelta);
            obj.AddComponent<Canvas>();
            obj.AddComponent<GraphicRaycaster>();

            var kbutton = obj.AddComponent<KButton>();
            var kimage = obj.AddComponent<KImage>();
            kimage.colorStyleSetting = style;
            kimage.ApplyColorStyleSetting();

            kbutton.additionalKImages = new KImage[0];

            return obj;
        }

        public GameObject MakeController(string name)
        {
            GameObject obj = new GameObject(name);

            var kc = obj.AddComponent<KBatchedAnimController>();
            var ka = kc.GetCurrentAnim();
            
            return obj;
        }
    }
}
