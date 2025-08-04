using JQCore.tRes;
using UnityEngine;

namespace JQFramework.tMVC.Base
{
    public abstract class ObjSubView : SubView
    {
        public void SetSubViewGo(BindObjLib bindObjLib)
        {
            _bindObjLib = bindObjLib;
            _bindObjLib.InitDic();
            OnBindAttr();
            OnSubViewLoadFinish();
            _subViewGo = bindObjLib.gameObject;
            _subViewTrans = _subViewGo.transform as RectTransform;
        }

        public override void OpenSubView(params object[] args)
        {
            _params = args;
            _isOpened = true;
            RealOpenView();
        }
    }
}