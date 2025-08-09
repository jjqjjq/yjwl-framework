using System;
using System.Collections.Generic;
using ET;
using HardCodeLab.TutorialMaster;
using JQCore;
using JQCore.ECS.Interface;
using JQCore.tLog;
using JQFramework.tUGUI;
using JQCore.tEnum;
using JQCore.tPool.Loader;
using JQCore.tRes;
using JQCore.tTime;
using JQFramework.tMVC.Base;
using JQCore.tUtil;
using JQFramework.tMgr;
using JQFramework.tMVC;
using JQFramework.UIModel;
#if TextMeshPro
using TMPro;
#endif
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JQFramework.tUtil
{
    public static class ViewUtil
    {
        //初始化分辨率
        public static void InitResolutionRatio()
        {
            if (Sys.GAME_WIDTH > Sys.GAME_HEIGHT) //横屏
            {
#if !UNITY_WEBGL
                Screen.orientation = ScreenOrientation.AutoRotation;
                SafeAreaUtil.currScreenOrientation = ScreenOrientation.AutoRotation;
#endif
                Screen.autorotateToPortrait = false;
                Screen.autorotateToPortraitUpsideDown = false;
                Screen.autorotateToLandscapeLeft = true;
                Screen.autorotateToLandscapeRight = true;
            }
            else //竖屏
            {
#if !UNITY_WEBGL
                Screen.orientation = ScreenOrientation.Portrait;
                SafeAreaUtil.currScreenOrientation = ScreenOrientation.Portrait;
#endif
            }

            var realWidthScale = 1f * Screen.width / Sys.GAME_WIDTH;
            var realHeightScale = 1f * Screen.height / Sys.GAME_HEIGHT;
            var realScale = Mathf.Max(realHeightScale, realWidthScale);
            if (realScale < 1)
            {
                JQLog.Log(
                    $"resizeBuff:  realWidthScale:{realWidthScale} --- realHeightScale:{realHeightScale}---屏幕整体缩放：{realScale}");
                ScalableBufferManager.ResizeBuffers(realScale, realScale);
            }

        }


        public static void SetGoArrActive(this GameObject[] self, int activeIndex)
        {
            for (int i = 0; i < self.Length; i++)
            {
                GameObject gameObject = self[i];
                gameObject.SetActive_EX(i == activeIndex);
            }
        }

        /// <summary>
        /// Gets the accurate position of a given RectTransform in relation of the canvas it resides in
        /// Unlike other means of obtaining the position, this method will give an accurate position regardless how nested it is or what canvas type it is.
        /// </summary>
        /// <param name="target">RectTransform from which position will be calculated.</param>
        /// <param name="canvas">The Canvas where RectTransform resides in.</param>
        /// <returns>Calculated vector representing the position of the UI Element.</returns>
        public static Vector2 GetAccuratePosition(Transform target, Canvas canvas)
        {
            var calculatedPosition = new Vector3();
            if (target is RectTransform)
            {
                RectTransform currentParentRectTrans = target as RectTransform;
                calculatedPosition = (new Vector2(0.5f, 0.5f) - currentParentRectTrans.pivot) * currentParentRectTrans.sizeDelta;
            }

            var currentParent = target;

            // iterate through all parents until reaching the canvas
            while (currentParent != canvas)
            {
                calculatedPosition += currentParent.localPosition;

                if (currentParent.parent == null)
                    break;

                var parentTransform = currentParent.parent.transform;
                if (parentTransform == null)
                    break;

                currentParent = parentTransform;
            }

            // iterate through all canvas parents (if any) and subtract the difference to get final result
            var posDiff = new Vector3();
            currentParent = canvas.transform;

            while (currentParent != null)
            {
                posDiff += currentParent.localPosition;
                if (currentParent.parent == null)
                    break;

                var parentTransform = currentParent.parent.transform;
                if (parentTransform == null)
                    break;

                currentParent = parentTransform;
            }

            calculatedPosition -= posDiff;

            return calculatedPosition;
        }

        public static void SetImgArrActive(this Image[] self, int activeIndex)
        {
            for (int i = 0; i < self.Length; i++)
            {
                Image image = self[i];
                if (i == activeIndex)
                {
                    image.color = Color.white;
                }
                else
                {
                    image.color = Color.gray;
                }
            }
        }


#if TextMeshPro
        public static void SetTxtArrActive(this TextMeshProUGUI[] self, int activeIndex)
        {
            for (int i = 0; i < self.Length; i++)
            {
                TextMeshProUGUI textMeshProUGUI = self[i];
                if (i == activeIndex)
                {
                    textMeshProUGUI.color = Color.white;
                }
                else
                {
                    textMeshProUGUI.color = Color.gray;
                }
            }
        }
#endif
        public static void SetTxtArrActive(this Text[] self, int activeIndex)
        {
            for (int i = 0; i < self.Length; i++)
            {
                Text text = self[i];
                if (i == activeIndex)
                {
                    text.color = Color.white;
                }
                else
                {
                    text.color = Color.gray;
                }
            }
        }

        public static void InitTutorial(TutorialMasterManager tutorialMasterManager, Canvas canvas)
        {
            foreach (Tutorial tutorial in tutorialMasterManager.Tutorials)
            {
                foreach (HardCodeLab.TutorialMaster.Stage stage in tutorial.Stages)
                {
                    foreach (PopupModuleConfig popupModuleConfig in stage.Modules.Popups)
                    {
                        popupModuleConfig.Settings.TargetCanvas = canvas;
                    }

                    foreach (ArrowModuleConfig arrowModuleConfig in stage.Modules.Arrows)
                    {
                        arrowModuleConfig.Settings.TargetCanvas = canvas;
                    }

                    foreach (HighlightModuleConfig highlightModuleConfig in stage.Modules.Highlighters)
                    {
                        highlightModuleConfig.Settings.TargetCanvas = canvas;
                    }

                    foreach (ImageModuleConfig imageModuleConfig in stage.Modules.Images)
                    {
                        imageModuleConfig.Settings.TargetCanvas = canvas;
                    }
                }
            }
        }

        public static T CreateOneItem<T>(PrefabLoader prefabLoader, List<T> itemList, Transform parentTrans)
            where T : BaseItem, new()
        {
            GameObject itemGo = prefabLoader.Spawn(true);
            if (itemGo != null)
            {
                itemGo.transform.SetParent_EX(parentTrans);
                itemGo.transform.SetLocalScale_EX(1f, 1f, 1f);
                itemGo.transform.SetLocalPos_EX(0, 0, 0);
                BindObjLib bindObjLib = itemGo.GetComponent<BindObjLib>();
                T item = new T();
                item.SetBindObjLib(bindObjLib);
                itemList.Add(item);
                return item;
            }

            return null;
        }

        public static void RemoveAllItem<T>(PrefabLoader prefabLoader, List<T> itemList) where T : BaseItem
        {
            if (itemList != null)
            {
                foreach (T item in itemList)
                {
                    prefabLoader.DeSpawn(item.gameObject, true);
                    item.Dispose();
                }

                itemList.Clear();
            }
        }

        public static Vector2 getScreenUISize()
        {
            if (Sys.GAME_HEIGHT > Sys.GAME_WIDTH) //竖屏
            {
                float realWidth = Sys.GAME_WIDTH;
                float realHeight = Sys.GAME_WIDTH * Screen.height / Screen.width;
                Vector2 screenSize = new Vector2(realWidth, realHeight);
                return screenSize;
            }
            else //横屏
            {
                float realHeight = Sys.GAME_HEIGHT;
                float realWidth = Sys.GAME_HEIGHT * Screen.width / Screen.height;
                Vector2 screenSize = new Vector2(realWidth, realHeight);
                return screenSize;
            }
        }

        private static void AddUIDrag(RawImage rawImage, GameObject gameObject, float rotateSpeed = 0)
        {
            rawImage.raycastTarget = true;
            UIDragEventTrigger triggerObj = UnityUtil.AddMissingComponent<UIDragEventTrigger>(rawImage.gameObject);
            triggerObj.SetTarget(gameObject);
            if (rotateSpeed != 0)
            {
                triggerObj.SetRotateSpeed(rotateSpeed);
            }
        }


        public static UIModelCtrl AddDisplay(this UIModelCtrlMgr mgr, string name, IDisplayComponent displayCom,
            RawImage rawImage, float cameraHeight, float cameraFarDis, float orthographicSize, bool closeMSAA,
            float3 rotate, float cameraAngle, int depthBuffer, float3 pos)
        {
            UIModelCtrl modelCtrl = mgr.Spwan();
            modelCtrl.setParam(name, rawImage, cameraHeight, cameraFarDis, orthographicSize, closeMSAA, cameraAngle,
                depthBuffer);
            if (displayCom != null)
            {
                modelCtrl.addSubObj(displayCom.transform, rotate, pos);
                displayCom.setUIModelCtrl(modelCtrl);
            }

            return modelCtrl;
        }

        public static T addDisplay<T>(BaseView view, IEntityVo entityVo, RawImage rawImage, float cameraHeight,
            float cameraFarDis,
            float orthographicSize, bool closeMSAA, float3 rotate = default, float cameraAngle = 0f,
            int depthBuffer = 16, bool notDrag = false, float rotateSpeed = 0f, float3 pos = default,
            bool hideUIDisplayShow = false) where T : IDisplayComponent, new()
        {
            GameObject raw = rawImage.gameObject;
            view.removeDisplayByParent(raw);
            T displayCom = new T();
            if (entityVo.uiCallBack != null)
            {
                displayCom.eventDispatcher.AddEventListener(FrameworkEvent.LoadFinish, entityVo.uiCallBack);
            }

            displayCom.ready(entityVo);

            if (!notDrag)
            {
                AddUIDrag(rawImage, displayCom.gameObject, rotateSpeed);
            }

            UIModelCtrl uiModel = UIModelCtrlMgr.Instance.AddDisplay(view.GetViewName(), displayCom, rawImage,
                cameraHeight, cameraFarDis, orthographicSize, closeMSAA, rotate, cameraAngle, depthBuffer, pos);
            UIModelInfo uiModelInfo = new UIModelInfo()
            {
                parent = raw,
                display = displayCom,
                uiModel = uiModel
            };
            view.AddUIModelInfo(uiModelInfo);
            return displayCom;
        }


        // public static void ShowMessageBox(string descStr, string leftBtnStr = "取消", Action leftBtnAction = null,
        //     string rightBtnStr = "确定", Action rightBtnAction = null)
        // {
        //     UIManager.OpenView<MessageBoxView>(descStr, leftBtnStr, leftBtnAction, rightBtnStr, rightBtnAction);
        // }
        //
        // //
        // public static void ShowLoadingMask(float time)
        // {
        //     string temp = time.ToString();
        //     UIManager.OpenView<LoadingMaskView>(temp);
        // }

        public static UIModelInfo AddModelCtrl(BaseView view, RawImage rawImage, float orthographicSize, bool closeMSAA,
            int depthBuffer)
        {
            GameObject raw = rawImage.gameObject;
            UIModelCtrl uiModel = UIModelCtrlMgr.Instance.AddDisplay(view.GetViewName(), null, rawImage, default,
                default, orthographicSize, closeMSAA, default, 0, depthBuffer, default);
            UIModelInfo uiModelInfo = new UIModelInfo()
            {
                parent = raw,
                display = null,
                uiModel = uiModel
            };
            view.AddUIModelInfo(uiModelInfo);
            return uiModelInfo;
        }

        public static void OpenViewByName(string viewName)
        {
            switch (viewName)
            {
                // case EView.Buy:
                //     OpenView<BuyView>();
                //     break;
                // case EView.NpcDialog:
                //     NpcConfig npcConfig = NpcConfigCategory.Instance.GetOne();
                //     OpenView<NpcDialogView>(npcConfig);
                //     break;
                // case EView.SceneMsgBoard:
                //     OpenView<SceneMsgBoardView>();
                //     break;
                // case EView.VideoUpload2:
                //     OpenView<VideoUpload2View>();
                //     break;
                // case EView.WebBrowser:
                //     OpenView<WebBrowserView>();
                //     break;
                default:
                    JQLog.LogError($"未添加的界面:{viewName}");
                    break;
            }
        }

        private static Dictionary<Button, float> _btnLastClickTimeDic = new Dictionary<Button, float>();

        public static void AddBtnListener(Button btn, UnityAction unityAction, float secondClickTime = 0.1f,
            string sound = "0")
        {
            btn.RemoveAllListeners_EX();
            btn.onClick.AddListener(() =>
            {
                //防止过快点击
                if (secondClickTime != 0)
                {
                    float nowTime = SysTime.time;
                    _btnLastClickTimeDic.TryGetValue(btn, out float lastClickTime);
                    if (lastClickTime != 0f && nowTime - lastClickTime < secondClickTime)
                    {
                        JQLog.LogWarning("点击过快");
                        return;
                    }

                    _btnLastClickTimeDic[btn] = nowTime;
                }

                //按钮音效
                if (sound != "0")
                {
                    SoundMgr.Instance.PostEvent($"play-SFX-{sound}");
                }
                else
                {
                    SoundMgr.Instance.PostEvent("play-SFX-ButtonClick");
                }

                unityAction();
            });
        }
    }
}