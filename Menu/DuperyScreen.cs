using KMod;
using ProcGen;
using ProcGenGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dupery.Menu
{
    public abstract class DuperyScreen : KModalScreen
    {
        public static ColorStyleSetting defaultButtonStyle;

        public bool prefabReady = false;

        public GameObject MakeUIObject(string name, Vector2 sizeDelta)
        {
            GameObject obj = new GameObject(name);

            RectTransform rt = obj.AddComponent<RectTransform>();
            rt.sizeDelta = sizeDelta;

            return obj;
        }

        public GameObject MakeCanvasObject(string name, Vector2 sizeDelta)
        {
            GameObject obj = MakeUIObject(name, sizeDelta);
            obj.AddComponent<CanvasRenderer>();

            return obj;
        }

        public GameObject InstantiateTexture(string name, GameObject parent, Vector2 sizeDelta, Vector3 position, Texture2D tex = null)
        {
            GameObject obj = MakeCanvasObject(name, sizeDelta);
            if (tex != null)
            {
                var sr = obj.AddComponent<KImage>();
                sr.color = new Color(0.9f, 0.9f, 0.9f, 1.0f);
                sr.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            }

            obj = Instantiate(obj, parent.transform);
            obj.transform.localPosition = position;
            return obj;
        }

        public GameObject InstantiateSprite(string name, GameObject parent, Sprite sprite, Vector3 position)
        {
            GameObject obj = MakeCanvasObject(name, sprite.rect.size);

            var sr = obj.AddComponent<KImage>();
            sr.color = new Color(0.9f, 0.9f, 0.9f, 1.0f);
            sr.sprite = sprite;

            obj = Instantiate(obj, parent.transform);
            obj.transform.localPosition = position;
            return obj;
        }

        public GameObject InstantiateLabel(string name, GameObject parent, Vector2 sizeDelta, Vector3 position, string textKey)
        {
            GameObject obj = MakeCanvasObject(name, sizeDelta);
            obj.AddComponent<SetTextStyleSetting>();

            var textMesh = obj.AddComponent<TextMeshProUGUI>();
            textMesh.text = "BLKBEBJEIBVAINVEIANEINNEINBVE";

            //var locText = obj.AddComponent<LocText>();
            //locText.text = Strings.Get().String;
            //var stringKey = new StringKey(textKey);
            //stringKey.Hash = textKey.GetHashCode();
            //Logger.Log($"{textKey} {locText.text}");
            //locText.key = textKey;

            //Logger.Log(locText.key);
            //locText.text = Strings.Get(new StringKey(locText.key)).String;

            //locText.font = Localization.FontAsset;
            //locText.fontStyle = TMPro.FontStyles.Bold;
            //locText.fontSize = (float)14;

            //var lt = obj.AddComponent<LocText>();
            //lt.text = "BLABLALA";
            //lt.font = Localization.FontAsset;
            //lt.fontStyle = TMPro.FontStyles.Bold;
            //lt.fontSize = (float)14;

            obj = Instantiate(obj, parent.transform);
            obj.transform.localPosition = position;
            return obj;
        }

        public GameObject InstantiateButton(string name, GameObject parent, Vector2 sizeDelta, Vector3 position, ColorStyleSetting style = null)
        {
            if (style == null)
                style = defaultButtonStyle;

            GameObject obj = MakeCanvasObject(name, sizeDelta);
            obj.AddComponent<Canvas>();
            obj.AddComponent<GraphicRaycaster>();

            var kbutton = obj.AddComponent<KButton>();
            var kimage = obj.AddComponent<KImage>();
            kimage.colorStyleSetting = style;
            kimage.ApplyColorStyleSetting();

            kbutton.additionalKImages = new KImage[0];

            obj = Instantiate(obj, parent.transform);
            obj.transform.localPosition = position;
            return obj;
        }

        protected override void OnPrefabInit()
        {
            if (!prefabReady) return;
            base.OnPrefabInit();
            SetupScreen();
        }

        protected override void OnSpawn()
        {
            if (!prefabReady) return;
            base.OnSpawn();
            AfterSpawn();
        }

        protected override void OnCmpEnable()
        {
            if (!prefabReady) return;
            base.OnCmpEnable();
        }

        protected override void OnShow(bool show)
        {
            if (!prefabReady) return;
            base.OnShow(show);
        }

        protected abstract void SetupScreen();

        protected abstract void AfterSpawn();

        private void SetStyle()
        {
            defaultButtonStyle = new ColorStyleSetting();
            defaultButtonStyle.activeColor = new Color(0.503f, 0.544f, 0.699f, 1.000f);
            defaultButtonStyle.disabledActiveColor = new Color(0.625f, 0.616f, 0.588f, 1.000f);
            defaultButtonStyle.disabledColor = new Color(0.416f, 0.412f, 0.400f, 1.000f);
            defaultButtonStyle.disabledhoverColor = new Color(0.500f, 0.490f, 0.460f, 1.000f);
            defaultButtonStyle.hoverColor = new Color(0.346f, 0.374f, 0.485f, 1.000f);
            defaultButtonStyle.inactiveColor = new Color(0.243f, 0.263f, 0.341f, 1.000f);
        }
    }
}
