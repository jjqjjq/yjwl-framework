using JQCore;
using JQCore.tEnum;
using JQCore.tMgr;
using JQCore.tPool.Loader;
using JQCore.tRes;
using JQCore.tUtil;
using JQFramework.tMgr;
using UnityEngine;

namespace JQFramework.tMVC.Base
{
    /// <summary>
    /// 独立Prefab的子界面
    /// </summary>
    public abstract class UrlSubView : SubView
    {
        protected string _url = null;
        private string _fullUrl = null;
        protected Transform _viewParent = null;

        public void SetSubViewParent(Transform viewParent)
        {
            _viewParent = viewParent;
        }

        public override void OpenSubView(params object[] args)
        {
            _params = args;
            _isOpened = true;

            //已经打开
            if (_isOpened && _subViewGo != null)
            {
                OnSubViewRefresh();
                Sys.gameDispatcher.TriggerEvent(FrameworkEvent.OpenOverView, this);
                return;
            }

            //未加载
            if (_subViewGo == null)
            {
                UrlPrefabLoaderUtil.LoadAsset(getFullUrl, OnLoadFinish);
            }
            else
            {
                RealOpenView();
            }
        }

        private void OnLoadFinish(UrlPrefabLoader urlPrefabLoader)
        {
            if (!_isOpened) return;
            _subViewGo = urlPrefabLoader.Spawn();
            UnityUtil.SetLayer(_subViewGo, ELayer.UI);

            _subViewTrans = _subViewGo.transform as RectTransform;
            _subViewTrans.SetParent_EX(_viewParent);
            _subViewTrans.SetOffsetMin_EX(0, 0);
            _subViewTrans.SetOffsetMax_EX(0, 0);
            _subViewTrans.SetLocalScale_EX(1, 1, 1);

            _subViewGo.SetActive_EX(true);

            _bindObjLib = _subViewGo.GetComponent<BindObjLib>();
            _bindObjLib.InitDic();

            OnBindAttr();
            OnSubViewLoadFinish();

            RealOpenView();
        }

        public override void CloseSubView()
        {
            base.CloseSubView();
            //移除界面Res
            UrlPrefabLoaderUtil.CancelLoadAsset(getFullUrl, OnLoadFinish);
        }

        public override void DestroySubView()
        {
            if (_subViewGo != null)
            {
                UrlPrefabLoaderUtil.DeSpawnPerformance(getFullUrl, _subViewGo);
            }
            base.DestroySubView();
        }

        private string getFullUrl
        {
            get
            {
                if (_fullUrl == null)
                {
                    _fullUrl = Sys.stringBuilder.Append("UI/Views/").Append(_url).Append(".prefab").ToString();
                }

                return _fullUrl;
            }
        }
    }
}