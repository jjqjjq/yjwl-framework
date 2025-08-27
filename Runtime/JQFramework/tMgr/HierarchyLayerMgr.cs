
using System.Collections.Generic;
using JQCore.tUtil;
using UnityEngine;
using UnityEngine.UI;

namespace JQFramework.tMgr
{
    public static class HierarchyLayerMgr
    {
        #region Transforms

        public static Transform rootScene;
        public static Transform cameras;
        public static Transform map;
        public static RectTransform canvasTransform;
        public static Canvas canvas;
        public static RawImage sceneRawImage;
        public static float resolutionW;
        public static float resolutionH;

        public static Transform scene;
        public static Transform effect;
        public static Transform uiModel;
        public static Transform uiMesh;

        public static Transform Bubble;

        public static Transform sceneUI;
        public static Transform SceneHudUI;
        public static Transform SceneHeroHudUI;
        public static Transform SceneNumberUI;
        public static Transform SceneEffectUI;


        public static Transform MainUI;
        public static Transform BaseUI;
        public static Transform ViewUI;
        public static Transform FrontUI;
        public static Transform FrontFrontUI;

        #endregion

        
        private static Dictionary<string, Transform> _children = new Dictionary<string, Transform>();
        private static float offsetX_left;
        private static float offsetX_right;

        public static Transform GetChild(string name)
        {
            _children.TryGetValue(name, out Transform child);
            return child;
        }

        public static void init(string[] extraGo = null)
        {
            
            // UnityUtil.StaticFind_EX("Others/HyAudio").AddComponent_EX<HyAudioInitializer>();
            
            #if !UNITY_EDITOR
           
            #endif
           
            rootScene = UnityUtil.StaticFind_EX("Driver/SceneRoot").transform;
            map = initChild(rootScene, "Map");

            if(extraGo != null)
            for (int i = 0; i < extraGo.Length; i++)
            {
                string name = extraGo[i];
                _children[name] = initChild(rootScene, name);
            }

            cameras = UnityUtil.StaticFind_EX("Driver/Cameras").transform;
#if UNITY_IOS
            wechatObj=UnityUtil.StaticFind_EX("Others/WeChatComponent").AddComponent_EX<WeChatComponent>();
#endif
            canvasTransform = (RectTransform)UnityUtil.StaticFind_EX("Driver/Canvas").transform;
            canvas = canvasTransform.GetComponent<Canvas>();
            // sceneRawImage = canvas.Find_EX("SceneRawImage").GetComponent<RawImage>();
            
            resolutionW = canvasTransform.sizeDelta.x;
            resolutionH = canvasTransform.sizeDelta.y;

            scene = initChild(rootScene, "Scene");
            effect = initChild(rootScene, "Effect");
            uiModel = initChild(scene, "UIModel");
            //  Light  _light = uiModel.gameObject.AddComponent<Light>();
            // _light.type = LightType.Directional;
            // _light.color = new Color(238f / 255f, 217f / 255f, 206f / 255f, 255f / 255f);
            // _light.intensity = 1.87f;
            // _light.shadows = LightShadows.Hard;
            // _light.cullingMask = LayerMgr.GetMaskValue(LayerMgr.UIWorld);
            
            uiMesh = initChild(scene, "UIMesh");

            #region group

            Bubble = initChild(scene, "Bubble");
            Canvas bubbleCanvas = Bubble.gameObject.AddMissingComponent<Canvas>();
            Bubble = bubbleCanvas.transform;

            #endregion

            sceneUI = initChild(canvasTransform, "SceneUI", true);

            #region uiGroup

            SceneHudUI = initChild(sceneUI, "SceneHudUI", true, 1000);
            SceneHeroHudUI = initChild(sceneUI, "SceneHeroHudUI", true, 1100);
            SceneNumberUI = initChild(sceneUI, "SceneNumberUI", true, 2000);
            // SceneCardUI = initChild(sceneUI, "SceneCardUI", true, 3000);
            SceneEffectUI = initChild(sceneUI, "SceneEffectUI", true, 4000);

            #endregion

            MainUI = initChild(canvasTransform, "MainUI", true);
            BaseUI = initChild(canvasTransform, "BaseUI", true);
            ViewUI = initChild(canvasTransform, "ViewUI", true);
            FrontUI = initChild(canvasTransform, "FrontUI", true);
            FrontFrontUI = initChild(canvasTransform, "FrontUI1", true);
            
            UnityUtil.SetLayer(SceneNumberUI.gameObject, ELayer.UI);
            UnityUtil.SetLayer(SceneHudUI.gameObject, ELayer.UI);
            UnityUtil.SetLayer(SceneHeroHudUI.gameObject, ELayer.UI);
            // UnityUtil.SetLayer(SceneCardUI.gameObject, ELayer.UI);
        }

        private static Transform initChild(Transform parent, string name, bool isUGUI = false, int rootOrder = 0)
        {
            Transform transform = UnityUtil.NewGameObject(name).transform;
            transform.SetParent_EX(parent);
            transform.SetLocalScale_EX(1, 1, 1);
            if (isUGUI)
            {
                RectTransform rectTransform = transform.AddComponent_EX<RectTransform>();
                transform = rectTransform;
                rectTransform.SetSizeDelta_EX(0, 0);
                rectTransform.SetAnchorMax_EX(1, 1);

                //界面适配数据
                rectTransform.SetAnchorMin_EX(0, 0);
                rectTransform.SetAnchorMax_EX(1, 1);
                rectTransform.SetPivot_EX(0.5f, 0.5f);

                rectTransform.SetOffsetMin_EX(0, 0);
                rectTransform.SetOffsetMax_EX(0, 0);

                if (rootOrder != 0)
                {
                    Canvas canvas = transform.AddComponent_EX<Canvas>();
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = rootOrder;
                }
            }
            else
            {
                transform.SetLocalPos_EX(0, 0, 0);
            }

            return transform;
        }
    }
}