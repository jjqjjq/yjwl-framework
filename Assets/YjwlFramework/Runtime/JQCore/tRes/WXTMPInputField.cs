#if TextMeshPro
using UnityEngine;
using System.Collections;
using JQCore.tLog;
using TMPro;
#if SDK_WEIXIN
using WeChatWASM;
#endif
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JQFramework.JQFramework.tUGUI
{
    public class WXTMPInputField : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
    {
        public TMP_InputField input;
        private bool isShowKeyboard = false;
        
        public void OnPointerClick(PointerEventData eventData)
        {
#if !UNITY_EDITOR && SDK_WEIXIN
            JQLog.Log("OnPointerClick");
            ShowKeyboard();
#endif
        }

        public void OnPointerExit(PointerEventData eventData)
        {
#if !UNITY_EDITOR && SDK_WEIXIN
            JQLog.Log("OnPointerExit");
            if (!input.isFocused)
            {
                HideKeyboard();
            }
#endif
        }

#if SDK_WEIXIN
        public void OnInput(OnKeyboardInputListenerResult v)
        {
#if !UNITY_EDITOR && SDK_WEIXIN
            JQLog.Log("onInput");
            JQLog.Log(v.value);
            if (input.isFocused)
            {
                input.text = v.value;
            }
#endif
        }
        public void OnConfirm(OnKeyboardInputListenerResult v)
        {
#if !UNITY_EDITOR && SDK_WEIXIN
            // 输入法confirm回调
            JQLog.Log("onConfirm");
            JQLog.Log(v.value);
            HideKeyboard();
#endif
        }

        public void OnComplete(OnKeyboardInputListenerResult v)
        {
#if !UNITY_EDITOR && SDK_WEIXIN
            // 输入法complete回调
            JQLog.Log("OnComplete");
            JQLog.Log(v.value);
            HideKeyboard();
#endif
        }

        private void ShowKeyboard()
        {
            if (!isShowKeyboard)
            {
                WX.ShowKeyboard(new ShowKeyboardOption()
                {
                    defaultValue = "xxx",
                    maxLength = 20,
                    confirmType = "go"
                });

                //绑定回调
                WX.OnKeyboardConfirm(OnConfirm);
                WX.OnKeyboardComplete(OnComplete);
                WX.OnKeyboardInput(OnInput);
                isShowKeyboard = true;
            }
        }

        private void HideKeyboard()
        {
            if (isShowKeyboard)
            {
                WX.HideKeyboard(new HideKeyboardOption());
                //删除掉相关事件监听
                WX.OffKeyboardInput(OnInput);
                WX.OffKeyboardConfirm(OnConfirm);
                WX.OffKeyboardComplete(OnComplete);
                isShowKeyboard = false;
            }
        }
#endif
    }
}
#endif