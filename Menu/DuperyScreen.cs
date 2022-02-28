using KMod;
using ProcGen;
using ProcGenGame;
using UnityEngine;
using UnityEngine.UI;

namespace Dupery.Menu
{
    public abstract class DuperyScreen : KModalScreen
    {
        public static ColorStyleSetting defaultButtonStyle;

        public bool prefabReady = false;

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
