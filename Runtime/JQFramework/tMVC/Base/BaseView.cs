using System;
using System.Collections.Generic;
using JQCore.tTime;
using JQCore.tLog;
using JQCore.DynamicTexture;
using JQCore.tMgr;
using JQCore.tUtil;
using JQFramework.tMgr;
using JQFramework.UIModel;
#if TextMeshPro
using TMPro;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JQFramework.tMVC.Base
{
    public abstract class BaseView : IDynamicTextureOwner
    {
        public bool isStackView = false; //是否是堆叠界面,在具体的构造函数内赋值
        protected bool _isOpened;

        public bool IsOpened => _isOpened;

        private List<UIModelInfo> _uiModelInfoList = new List<UIModelInfo>();
        protected Dictionary<Button, float> _btnLastClickTimeDic = new Dictionary<Button, float>();

        protected string _viewName;
        public abstract string GetViewName();

        public void AddUIModelInfo(UIModelInfo uiModelInfo)
        {
            if (_uiModelInfoList == null)
            {
                _uiModelInfoList = new List<UIModelInfo>();
            }

            _uiModelInfoList.Add(uiModelInfo);
        }

        public void RemoveUIModelInfo(UIModelInfo uiModelInfo)
        {
            if (_uiModelInfoList == null) return;
            _uiModelInfoList.Remove(uiModelInfo);
        }

        public void removeDisplayByParent(GameObject raw)
        {
            foreach (UIModelInfo uiModelInfo in _uiModelInfoList)
            {
                if (uiModelInfo.parent == raw)
                {
                    uiModelInfo.Dispose();
                    _uiModelInfoList.Remove(uiModelInfo);
                    return;
                }
            }
        }

        protected void removeAllDisplay()
        {
            if (_uiModelInfoList == null) return;
            for (int i = 0; i < _uiModelInfoList.Count; i++)
            {
                UIModelInfo uiModelInfo = _uiModelInfoList[i];
                uiModelInfo.Dispose();
            }

            _uiModelInfoList.Clear();
        }


        public void addDynamicLoader(Image image, string fileName, Action<Image> callbackFunc = null)
        {
            DynamicImageLoaderMgr.Instance.addDynamicLoader(this, image, fileName, callbackFunc);
        }

        public void addDynamicLoaderFullPath(Image image, string fullPath, Action<Image> callbackFunc = null)
        {
            DynamicImageLoaderMgr.Instance.addDynamicLoaderFullPath(this, image, fullPath, callbackFunc);
        }

        #region InputField 输入变化事件

#if TextMeshPro
        protected void AddInputListener(TMP_InputField tmpInputField, UnityAction<string> unityAction)
        {
            tmpInputField.onValueChanged.RemoveAllListeners();
            tmpInputField.onValueChanged.AddListener(unityAction);
        }
#else

        protected void AddInputListener(InputField inputField, UnityAction<string> unityAction)
        {
            inputField.onValueChanged.RemoveAllListeners();
            inputField.onValueChanged.AddListener(unityAction);
        }
#endif

        #endregion

        #region Button 按钮点击统一事件

        protected void AddBtnListener(Button btn, UnityAction unityAction, float secondClickTime = 0.1f, string sound = "0")
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

        #endregion
    }
}