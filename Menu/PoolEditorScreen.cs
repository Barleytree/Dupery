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

            GameObject panel = InstantiateTexture("Panel", background, new Vector2(700, 580), Vector3.zero);
            Image panelImage = panel.AddComponent<Image>();

            GameObject title = InstantiateTexture("Title", panel, new Vector2(30, 39), new Vector3(-350, -290, 0));

            GameObject titleLabel = InstantiateLabel("titleLabel", title, new Vector2(200, 40), Vector3.zero, "STRINGS.UI.CLOSE");

            GameObject closeButton = InstantiateButton("CloseButton", title, new Vector2(20, 20), Vector3.zero);

            GameObject closeButtonSprite = InstantiateTexture("CloseButtonSprite", closeButton, new Vector2(20, 19), Vector3.zero);

            GameObject moreButton = InstantiateButton("MoreButton", panel, new Vector2(20, 20), Vector3.zero);

            GameObject moreButtonSprite = InstantiateTexture("MoreButtonSprite", moreButton, new Vector2(20, 19), Vector3.zero);

            Personality personality = Db.Get().Personalities[0];
            InstantiateDupeInfo(personality, panel);

            this.closeButton = closeButton.GetComponent<KButton>();
            this.closeButton.onClick += new System.Action(((KScreen)this).Deactivate);
        }

        private void InstantiateDupeInfo(Personality personality, GameObject parent)
        {
            Sprite headSprite = DuperyPatches.AccessoryManager.GetAcessorySprite(Db.Get().AccessorySlots.HeadShape, personality.headShape);
            GameObject headPreview = InstantiateSprite("HeadPreview", parent, headSprite, Vector3.zero);
            headPreview.transform.localPosition = new Vector3(400, 0, 4);

            Sprite hairSprite = DuperyPatches.AccessoryManager.GetAcessorySprite(Db.Get().AccessorySlots.Hair, personality.hair);
            GameObject hairPreview = InstantiateSprite("HairPreview", parent, hairSprite, Vector3.zero);
            hairPreview.transform.localPosition = new Vector3(0, 0, 4);
        }

        protected override void AfterSpawn()
        {
            DuperyDebug.LogObjectTree(gameObject);
        }
    }
}
