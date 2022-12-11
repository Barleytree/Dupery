using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Dupery
{
    class DuperyDebug
    {
        public static void LogProperties(object obj)
        {
            Logger.Log($"<{obj}>");
            foreach (System.ComponentModel.PropertyDescriptor descriptor in System.ComponentModel.TypeDescriptor.GetProperties(obj))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(obj);
                Logger.Log($"{name}={value}");
            }
        }

        public static void LogObjectTree(GameObject gameObject)
        {
            int depth = GetTransformDepth(gameObject.transform);

            LogComponents(gameObject, depth);
            foreach (Transform childTransform in gameObject.transform)
            {
                LogObjectTree(childTransform.gameObject);
            }
        }

        public static int GetTransformDepth(Transform transform)
        {
            int depth = 0;

            Transform parent = transform.parent;
            while (parent != null)
            {
                depth += 1;
                parent = parent.parent;
            }

            return depth;
        }

        public static void LogComponents(GameObject gameObject, int indent = 0)
        {
            String gap = new String(' ', indent * 2);
            Vector3 position = gameObject.transform.localPosition;
            String parentName = gameObject.transform.parent != null ? gameObject.transform.parent.name : "?";
            Logger.Log($"{gap}[ {gameObject.name} ] ({position.x},{position.y},{position.z})");

            Component[] components = gameObject.GetComponents(typeof(Component));
            List<Component> otherComponents = new List<Component>();
            RectTransform rectTransform = null;
            LocText locText = null;
            SetTextStyleSetting setTextStyleSetting = null;
            Image image = null;
            KScrollRect scrollRect = null;
            VerticalLayoutGroup vLayoutGroup = null;
            ContentSizeFitter sizeFitter = null;
            HierarchyReferences hierarchy = null;
            DragMe dragMe = null;

            foreach (Component component in components)
            {
                if (component.GetType() == typeof(RectTransform))
                    rectTransform = (RectTransform)component;
                else if (component.GetType() == typeof(LocText))
                    locText = (LocText)component;
                else if (component.GetType() == typeof(SetTextStyleSetting))
                    setTextStyleSetting = (SetTextStyleSetting)component;
                else if (component.GetType() == typeof(Image))
                    image = (Image)component;
                else if (component.GetType() == typeof(KScrollRect))
                    scrollRect = (KScrollRect)component;
                else if (component.GetType() == typeof(VerticalLayoutGroup))
                    vLayoutGroup = (VerticalLayoutGroup)component;
                else if (component.GetType() == typeof(ContentSizeFitter))
                    sizeFitter = (ContentSizeFitter)component;
                else if (component.GetType() == typeof(HierarchyReferences))
                    hierarchy = (HierarchyReferences)component;
                else if (component.GetType() == typeof(DragMe))
                    dragMe = (DragMe)component;
                else
                    otherComponents.Add(component);
            }

            if (rectTransform != null)
                Logger.Log($"{gap}RectTranform <{rectTransform.anchoredPosition.x},{rectTransform.anchoredPosition.y}> <{rectTransform.sizeDelta.x},{rectTransform.sizeDelta.y}>");

            if (locText != null)
                Logger.Log($"{gap}LocText ({locText.font}) {locText.text}");

            if (setTextStyleSetting != null)
                Logger.Log($"{gap}SetTextStyleSetting {setTextStyleSetting.ToString()}");

            if (scrollRect != null)
                Logger.Log($"{gap}ScrollRect -{scrollRect.content.name}- ({scrollRect.flexibleHeight},{scrollRect.flexibleWidth}) ({scrollRect.minHeight},{scrollRect.minWidth}) ({scrollRect.flexibleHeight},{scrollRect.flexibleWidth}) ({scrollRect.layoutPriority},{scrollRect.forceContentMatchWidth})");

            if (vLayoutGroup != null)
                Logger.Log($"{gap}vLayoutGroup ({vLayoutGroup.preferredHeight}, {vLayoutGroup.preferredWidth})");

            if (sizeFitter != null)
                Logger.Log($"{gap}sizeFitter ({sizeFitter.verticalFit}, {sizeFitter.horizontalFit})");

            if (hierarchy != null)
                Logger.Log($"{gap}hierarchy ({hierarchy.references})");

            if (dragMe != null)
                Logger.Log($"{gap}dragMe ({dragMe.listener})");

            if (image != null)
            {
                Texture texture = image.mainTexture;
                Sprite sprite = image.sprite;
                if (sprite != null)
                {
                    if (sprite.name == "cancel")
                    {
                        //LogTexture(sprite);
                    }

                    Rect r = sprite.rect;
                    Rect tr = sprite.textureRect;
                    Logger.Log($"{gap}Image <{sprite.name}> rect<{r.width},{r.height}> tex<{tr.x},{tr.y}>({tr.width},{tr.height}) [{image.color}] [{texture.name}]");
                }
                else
                    Logger.Log($"{gap}Image");
            }

            foreach (Component component in otherComponents)
            {
                Logger.Log($"{gap}{component.GetType()}");
            }
        }

        public static Texture2D TextureFromSprite(Sprite sprite)
        {
            if (sprite.rect.width != sprite.texture.width)
            {
                Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                             (int)sprite.textureRect.y,
                                                             (int)sprite.textureRect.width,
                                                             (int)sprite.textureRect.height);
                newText.SetPixels(newColors);
                newText.Apply();
                return newText;
            }
            else
                return sprite.texture;
        }

        public static void LogTexture(Sprite sprite)
        {
            string filePath = Path.Combine(DuperyPatches.DirectoryName, sprite.name);
            string data = $"{sprite.name} <{sprite.rect.width}, {sprite.rect.height}>\n";

            /*
            Color[] pixelColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                            (int)sprite.textureRect.y,
                                                            (int)sprite.textureRect.width,
                                                            (int)sprite.textureRect.height);
            */
            Color32[] pixelColors = GetPixelBlock(sprite.texture);

            foreach (Color c in pixelColors)
                data += c.ToHexString();

            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                streamWriter.Write(data);
            }
        }

        public static Color32[] GetPixelBlock(Texture2D tex)
        {
            Color32[] pixelBlock = null;
            try
            {
                pixelBlock = tex.GetPixels32();
            }
            catch (UnityException)
            {
                tex.filterMode = FilterMode.Point;
                RenderTexture rt = RenderTexture.GetTemporary(tex.width, tex.height);
                rt.filterMode = FilterMode.Point;
                RenderTexture.active = rt;
                Graphics.Blit(tex, rt);
                Texture2D newTex = new Texture2D(tex.width, tex.height);
                newTex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
                newTex.Apply();
                RenderTexture.active = null;
                tex = newTex;
                pixelBlock = tex.GetPixels32();
            }

            return pixelBlock;
        }

        private Sprite loadSlide(string file)
        {
            double realtimeSinceStartup = (double)Time.realtimeSinceStartup;
            Texture2D texture2D = new Texture2D(512, 768);
            texture2D.filterMode = FilterMode.Point;
            //texture2D.LoadImage(File.ReadAllBytes(file));
            return Sprite.Create(texture2D, new UnityEngine.Rect(Vector2.zero, new Vector2((float)texture2D.width, (float)texture2D.height)), new Vector2(0.5f, 0.5f), 100f, 0U, SpriteMeshType.FullRect);
        }
    }
}
