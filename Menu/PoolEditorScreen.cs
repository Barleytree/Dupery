using KMod;
using ProcGen;
using ProcGenGame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dupery.Menu
{
    public class PoolEditorScreen : DuperyScreen
    {
        private KButton closeButton;

        private List<PersonalityOutline> personalities;

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

            GameObject panel = InstantiateTexture("Panel", background, new Vector2(700, 580), Vector3.zero);

            GameObject title = InstantiateTexture("Title", panel, new Vector2(30, 39), new Vector3(-350, -290, 0));
            GameObject titleLabel = InstantiateLabel("titleLabel", title, new Vector2(200, 40), Vector3.zero, "STRINGS.UI.CLOSE");

            GameObject closeButton = InstantiateButton("CloseButton", title, new Vector2(20, 20), Vector3.zero);
            GameObject closeButtonSprite = InstantiateTexture("CloseButtonSprite", closeButton, new Vector2(20, 19), Vector3.zero);

            GameObject moreButton = InstantiateButton("MoreButton", panel, new Vector2(20, 20), Vector3.zero);
            GameObject moreButtonSprite = InstantiateTexture("MoreButtonSprite", moreButton, new Vector2(20, 19), Vector3.zero);

            DuperyPatches.LoadResources();
            LoadPersonalityOutlines();
            InstantiateDupeList(panel);

            this.closeButton = closeButton.GetComponent<KButton>();
            this.closeButton.onClick += new System.Action(((KScreen)this).Deactivate);
        }

        private void InstantiateDupeList(GameObject parent)
        {
            GameObject panel = InstantiateTexture("Dupes", parent, new Vector2(0, 0), Vector3.zero);
            var scrollRect = panel.AddComponent<KScrollRect>();
            GameObject viewPort = InstantiateUIObject("Viewport", panel, new Vector2(0, 0), Vector3.zero);
            viewPort.AddComponent<Canvas>();
            GameObject content = InstantiateTexture("Content", viewPort, new Vector2(700, 0), Vector3.zero);
            content.AddComponent<VerticalLayoutGroup>();
            var sizeFitter = content.AddComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            content.AddComponent<Canvas>();

            scrollRect.content = content.GetComponent<RectTransform>();

            //Sprite entrySprite = DuperyPatches.PersonalityManager.GetAcessorySprite(Db.Get().AccessorySlots.HeadShape, personalities[0].HeadShape);
            //GameObject entry = InstantiateSprite("Entry", content, entrySprite, Vector3.zero);
            //entry.AddComponent<HorizontalLayoutGroup>();
            //entry.AddComponent<HierarchyReferences>();
            //entry.AddComponent<DragMe>();
            //entry.AddComponent<Canvas>();
            //entry.AddComponent<GraphicRaycaster>();

            int verticalPosition = 400;
            int verticalOffset = 100;

            foreach (PersonalityOutline outline in personalities)
            {
                GameObject entry = InstantiateTexture("Content", content, new Vector2(0, 0), Vector3.zero);
                entry.transform.localPosition = new Vector3(-400, verticalPosition, 4);

                Sprite headSprite = DuperyPatches.PersonalityManager.GetAcessorySprite(Db.Get().AccessorySlots.HeadShape, outline.HeadShape);
                GameObject headPreview = InstantiateSprite("HeadPreview", entry, headSprite, Vector3.zero);
                //headPreview.transform.localPosition = new Vector3(-400, verticalPosition, 4);
                headPreview.AddComponent<HierarchyReferences>();
                //var dragoMe = headPreview.AddComponent<DragMe>();
                //dragoMe.
                headPreview.AddComponent<Canvas>();
                headPreview.AddComponent<GraphicRaycaster>();
                Sprite hairSprite = DuperyPatches.PersonalityManager.GetAcessorySprite(Db.Get().AccessorySlots.Hair, outline.Hair);
                GameObject hairPreview = InstantiateSprite("HairPreview", entry, hairSprite, Vector3.zero);
                hairPreview.transform.localPosition = new Vector3(0, 20, 0);

                verticalPosition = verticalPosition - verticalOffset;
            }
        }

        private void LoadPersonalityOutlines()
        {
            personalities = new List<PersonalityOutline>();
            var personalityMaps = new List<Dictionary<string, PersonalityOutline>>
            {
                DuperyPatches.PersonalityManager.StockPersonalities,
                //DuperyPatches.PersonalityManager.CustomPersonalities
            };
            //personalityMaps.AddRange(DuperyPatches.PersonalityManager.ImportedPersonalities.Values);

            foreach (Dictionary<string, PersonalityOutline> map in personalityMaps)
            {
                foreach (string nameStringKey in map.Keys)
                {
                    personalities.Add(map[nameStringKey]);
                }
            }
        }

        protected override void AfterSpawn()
        {
            DuperyDebug.LogObjectTree(gameObject);
        }
    }
}
