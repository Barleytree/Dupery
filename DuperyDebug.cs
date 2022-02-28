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

        public static void LogComponents(GameObject gameObject, int level=0)
        {
            String gap = new String(' ', level*2);
            Vector3 position = gameObject.transform.localPosition;
            Logger.Log($"{gap}[ {gameObject.name} ] ({position.x},{position.y},{position.z})");

            Component[] components = gameObject.GetComponents(typeof(Component));
            List<Component> otherComponents = new List<Component>();
            RectTransform rectTransform = null;
            Image image = null;

            foreach (Component component in components)
            {
                if (component.GetType() == typeof(RectTransform))
                    rectTransform = (RectTransform)component;
                else if (component.GetType() == typeof(Image))
                    image = (Image)component;
                else
                    otherComponents.Add(component);
            }

            if (rectTransform != null)
                Logger.Log($"{gap}RectTranform <{rectTransform.anchoredPosition.x},{rectTransform.anchoredPosition.y}> <{rectTransform.sizeDelta.x},{rectTransform.sizeDelta.y}>");
            if (image != null)
            {
                Sprite sprite = image.sprite;
                if (sprite != null)
                {
                    if (sprite.name == "cancel")
                    {
                        //LogTexture(sprite);
                    }

                    Rect r = sprite.rect;
                    Rect tr = sprite.textureRect;
                    Logger.Log($"{gap}Image <{sprite.name}> rect<{r.width},{r.height}> tex<{tr.x},{tr.y}>({tr.width},{tr.height}) [{image.color}]");
                }
                else
                    Logger.Log($"{gap}Image");
            }

            foreach (Component component in otherComponents)
            {
                Logger.Log($"{gap}{component.GetType()}");
            }

            foreach (Transform transform in gameObject.transform)
            {
                LogComponents(transform.gameObject, level=level+1);
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
            catch (UnityException _e)
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
    }
}
