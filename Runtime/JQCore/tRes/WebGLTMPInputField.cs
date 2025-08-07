#if TextMeshPro
using UnityEngine;
using System.Collections;
using JQCore.tLog;
using TMPro;
#if SDK_DOUYIN
using TTSDK;
#endif
#if SDK_WEIXIN
using WeChatWASM;
#endif
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JQFramework.tUGUI
{
    public class WebGLTMPInputField : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
    {
        public TMP_InputField input;
        private bool isShowKeyboard = false;
        
        public void OnPointerClick(PointerEventData eventData)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            JQLog.Log("OnPointerClick");
            ShowKeyboard();
#endif
        }

        public void OnPointerExit(PointerEventData eventData)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            JQLog.Log("OnPointerExit");
            if (!input.isFocused)
            {
                HideKeyboard();
            }
#endif
        }

#if UNITY_WEBGL && !UNITY_EDITOR
#if SDK_DOUYIN
        private void ShowKeyboard()
        {
            if (!isShowKeyboard)
            {
                TT.ShowKeyboard(new TTKeyboard.ShowKeyboardOptions()
                {
                    defaultValue = "",
                    confirmType = "go",
                    confirmHold = false,
                    maxLength = 20
                });
                TT.OnKeyboardConfirm += OnConfirm;
                TT.OnKeyboardInput += OnInput;
                TT.OnKeyboardComplete += OnComplete;
                isShowKeyboard = true;
            }
        }

        private void HideKeyboard()
        {
            if (isShowKeyboard)
            {
                TT.HideKeyboard();
                //删除掉相关事件监听
                TT.OnKeyboardConfirm -= OnConfirm;
                TT.OnKeyboardInput -= OnInput;
                TT.OnKeyboardComplete -= OnComplete;
                isShowKeyboard = false;
            }
        }


        private void OnInput(string v)
        {
            JQLog.Log("onInput:" + v);
            if (input.isFocused)
            {
                input.text = v;
            }
        }

        private void OnConfirm(string v)
        {
            // 输入法confirm回调
            JQLog.Log("onConfirm:" + v);
            HideKeyboard();
        }

        private void OnComplete(string v)
        {
            // 输入法complete回调
            JQLog.Log("OnComplete:" + v);
            HideKeyboard();
        }
#endif

#if SDK_WEIXIN
        private void ShowKeyboard()
        {
            if (!isShowKeyboard)
            {
                WX.ShowKeyboard(new ShowKeyboardOption()
                {
                    defaultValue = "",
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


        private void OnInput(OnKeyboardInputListenerResult v)
        {
            JQLog.Log("onInput:" + v.value);
            if (input.isFocused)
            {
                input.text = v.value;
            }
        }

        private void OnConfirm(OnKeyboardInputListenerResult v)
        {
            // 输入法confirm回调
            JQLog.Log("onConfirm:" + v.value);
            HideKeyboard();
        }

        private void OnComplete(OnKeyboardInputListenerResult v)
        {
            // 输入法complete回调
            JQLog.Log("OnComplete:" + v.value);
            HideKeyboard();
        }
#endif
#endif
    }
}
#endif