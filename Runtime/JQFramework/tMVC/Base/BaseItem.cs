using JQCore.tRes;
using JQFramework.tUtil;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JQFramework.tMVC.Base
{
    public abstract class BaseItem
    {
        public GameObject gameObject;
        protected BindObjLib _bindObjLib = null;
        
        public void SetBindObjLib(BindObjLib bindObjLib)
        {
            gameObject = bindObjLib.gameObject;
            _bindObjLib = bindObjLib;
            _bindObjLib.InitDic();
            OnBindAttr();
        }
        
        public virtual void Dispose()
        {
            OnUnBindAttr();
            _bindObjLib = null;
            gameObject = null;
        }

        protected void AddBtnListener(Button btn, UnityAction unityAction, float secondClickTime = 0.1f, string sound = "0")
        {
            ViewUtil.AddBtnListener(btn, unityAction, secondClickTime, sound);
        }
        
        protected abstract void OnBindAttr();
        protected abstract void OnUnBindAttr();
    }
}