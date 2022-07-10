using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dupery
{
    class AccessoryManager
    {
        public AccessoryPool Pool { get { return this.accessoryPool; } }

        private AccessoryPool accessoryPool;

        public AccessoryManager()
        {
            accessoryPool = new AccessoryPool();
        }

        public string TryGetAccessoryId(string slotId, string accessoryKey)
        {
            return accessoryPool.GetId(slotId, accessoryKey);
        }

        public Sprite GetAcessorySprite(AccessorySlot slot, int accessoryKey)
        {
            Logger.Log($"SLOT {slot.Name} HAS {slot.accessories.Count} ACCESSORIES, TRYING {accessoryKey}");
            Accessory accessory = slot.accessories[accessoryKey - 1];
            KAnim.Build.Symbol symbol = accessory.symbol;

            Texture2D texture = accessory.symbol.build.GetTexture(0);

            var symbolFrame = symbol.GetFrame(0).symbolFrame;
            var imageIndex = symbol.GetFrame(0).buildImageIdx;

            bool centered = true;

            Debug.Assert((UnityEngine.Object)texture != (UnityEngine.Object)null, (object)("Invalid texture on " + accessory.IdHash));
            float x1 = symbolFrame.uvMin.x;
            float x2 = symbolFrame.uvMax.x;
            float y1 = symbolFrame.uvMax.y;
            float y2 = symbolFrame.uvMin.y;
            int num1 = (int)((double)texture.width * (double)Mathf.Abs(x2 - x1));
            int num2 = (int)((double)texture.height * (double)Mathf.Abs(y2 - y1));
            float num3 = Mathf.Abs(symbolFrame.bboxMax.x - symbolFrame.bboxMin.x);
            UnityEngine.Rect rect = new UnityEngine.Rect();
            rect.width = (float)num1;
            rect.height = (float)num2;
            rect.x = (float)(int)((double)texture.width * (double)x1);
            rect.y = (float)(int)((double)texture.height * (double)y1);
            float pixelsPerUnit = 100f;
            if (num1 != 0)
                pixelsPerUnit = (float)(100.0 / ((double)num3 / (double)num1));
            Sprite sprite = Sprite.Create(texture, rect, centered ? new Vector2(0.5f, 0.5f) : Vector2.zero, pixelsPerUnit, 0U, SpriteMeshType.FullRect);
            sprite.name = string.Format("{0}:{1}:{2}:{3}", (object)texture.name, (object)"bodyPart", (object)imageIndex.ToString(), (object)centered);

            Logger.Log($"SPRITE: {sprite.name}");

            return sprite;
        }

        public Texture2D GetTexture2(AccessorySlot slot, int accessoryKey)
        {
            Logger.Log($"SLOT {slot.Name} HAS {slot.accessories.Count} ACCESSORIES, TRYING {accessoryKey}");
            var accessory = slot.accessories[accessoryKey - 1];
            //var symbol = accessory.symbol;
            var texture = accessory.symbol.build.GetTexture(0);

            var build = accessory.symbol.build;
            foreach (KAnim.Build.Symbol symbol in build.symbols)
            {
                //Logger.Log($"CHECKING SYMBOL {symbol.hash}");
                if (symbol.hash.ToString().StartsWith("headshape_"))
                {
                    for (int i = 0; i < symbol.numFrames; i++)
                    {
                        var frame = symbol.GetFrame(i).symbolFrame;
                        var imageIndex = symbol.GetFrame(i).buildImageIdx;
                        if (frame != null)
                        {
                            Logger.Log($"{symbol.hash} ; {build.fileHash} ; [bboxMin {frame.bboxMin} | bboxMax: {frame.bboxMax}] ; [uvMin: {frame.uvMin} | uvMax {frame.uvMax}]");
                        }
                    }
                }
            }

            KAnimFileData kfd = null;

            //kfd.GetAnim(0).GetFrame(0).bbox();

            Logger.Log($"Loading sprites for {build.fileHash}");
            Sprite[] sprites = Resources.LoadAll<Sprite>(build.fileHash.ToString());
            //foreach (Sprite sprite in sprites)
            //{
            //    Logger.Log($"Sprite '{sprite.name}' ; [Bounds: {sprite.bounds.min} {sprite.bounds.max}]");
            //}

            //Assets.SpriteAssets
            //Assets.Sprites
            //Sprite sprite = Assets.GetSprite("headshape_004");
            //Logger.Log($"Sprite '{sprite.name}' ; [Bounds: {sprite.bounds.min} {sprite.bounds.max}]");
            //sprite = Assets.GetSprite("head_swap_0");
            //Logger.Log($"Sprite '{sprite.name}' ; [Bounds: {sprite.bounds.min} {sprite.bounds.max}]");

            //foreach (KAnim.Build.SymbolFrame frame in build.frames)
            //{
            //    Logger.Log($"{frame.fileNameHash} #{frame.sourceFrameNum} [BBox: {frame.bboxMax}, {frame.bboxMin}] [UV: {frame.uvMax}, {frame.uvMin}");
            //}

            return texture;
        }

        public int LoadAccessories(string animName, bool saveToCache = false)
        {
            ResourceSet accessories = Db.Get().Accessories;

            KAnimFile anim = Assets.GetAnim(animName);
            KAnim.Build build = anim.GetData().build;
            //anim.GetData().GetAnim(anim.GetData().firstAnimIndex).GetFrame

            Logger.Log($"Logging frames for {animName}");
            foreach (var frame in build.frames)
            {
                Vector2 bboxMax = frame.bboxMax;
                Vector2 bboxMin = frame.bboxMin;
                Logger.Log($"{frame.fileNameHash} {frame.sourceFrameNum} | bboxMax: {bboxMax} | bboxMin {bboxMin}");
            }

            int numLoaded = 0;
            int numCached = 0;
            for (int index = 0; index < build.symbols.Length; ++index)
            {
                string id = HashCache.Get().Get(build.symbols[index].hash);

                AccessorySlot slot;
                bool cachable = true;

                if (id.StartsWith("hair_"))
                {
                    slot = Db.Get().AccessorySlots.Hair;
                }
                else if (id.StartsWith("hat_hair_"))
                {
                    slot = Db.Get().AccessorySlots.HatHair;
                    cachable = false;
                }
                else if (id.StartsWith("body_"))
                {
                    slot = Db.Get().AccessorySlots.Body;
                }
                else if (id.StartsWith("arm_"))
                {
                    slot = Db.Get().AccessorySlots.Arm;
                }
                else
                {
                    continue;
                }

                Accessory accessory = new Accessory(id, accessories, slot, anim.batchTag, build.symbols[index]);
                slot.accessories.Add(accessory);
                Db.Get().ResourceTable.Add(accessory);

                if (cachable && saveToCache)
                {
                    accessoryPool.AddId(slot.Id, id, id);
                    numCached++;
                }

                numLoaded++;
            }

            if (numCached > 0)
                Logger.Log($"Added {numCached} new accessories IDs to the cache.");

            return numLoaded;
        }
    }
}
