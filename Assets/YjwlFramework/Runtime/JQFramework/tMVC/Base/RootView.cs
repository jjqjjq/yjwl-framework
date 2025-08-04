using System;
using System.Collections.Generic;
using DG.Tweening;
using JQFramework.tMVC.sortingOrder;
using JQCore;
using JQCore.tLog;
using JQCore.DynamicTexture;
using JQCore.tEnum;
using JQCore.tMgr;
using JQCore.tPool.Loader;
using JQCore.tRes;
using JQCore.tUtil;
using JQCore.Log;
using JQFramework.tMgr;
using JQFramework.tMVC.Attributes;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace JQFramework.tMVC.Base
{
    public enum ViewType
    {
        Normal = 0, //UI类型-普通
        Main = 1, //主界面
        NoRestrain = 2 //不受约束UI(需要手动关闭/不统计打开界面个数里)
    }
    
    public abstract class RootView : BaseView
    {
        #region UI投射到场景Mesh上

        // private UIMeshCtrl _uiMeshCtrl;
        //
        // public void AddUIMesh(MeshRenderer meshRenderer, int width, int height)
        // {
        //     if (_uiMeshCtrl == null)
        //     {
        //         _uiMeshCtrl = UIMeshCtrlMgr.Instance.AddDisplay("SceneMsgBoardView", this, meshRenderer, width, height,
        //             1.01f, 5, 1.1f, false, new float3(12, 180, 0));
        //     }
        //     else
        //     {
        //         _uiMeshCtrl.addOtherMeshRenderer(meshRenderer);
        //     }
        // }

        #endregion

        protected object[] _params = null;
        protected GameObject _viewGo;
        protected ViewType _viewType = ViewType.Normal;
        protected bool _openTween = true;

        public GameObject viewGo
        {
            get { return _viewGo; }
        }

        protected RectTransform _viewTweenRectTrans;
        protected RectTransform _panelFunctionRectTrans;
        protected RectTransform _viewTrans;
        private string _fullUrl = null;


        private Dictionary<Type, string> _subViewType2NameDic = new Dictionary<Type, string>();
        private Dictionary<string, SubView> _subViewDic = new Dictionary<string, SubView>();

        protected BindObjLib _bindObjLib = null;


        protected string _url = null;

        public Canvas sortingCanvas = null;
        public GraphicRaycaster sortingGraphicRaycaster = null;
        protected int _sortingOrder = 0; //0则使用自动排序

        protected Transform _layerTrans = null; //layerType

        public Transform LayerTrans
        {
            get { return _layerTrans; }
        }

        /// <summary>
        /// 添加新的子界面 独立prefab,需要加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected void AddUrlSubView<T>(Transform subViewParentTrans) where T : UrlSubView, new()
        {
            SubView subView = AddSubView<T>();
            UrlSubView urlSubView = subView as UrlSubView;
            urlSubView.SetSubViewParent(subViewParentTrans);
        }

        public void OpenOnlyOneSubView<T>(params object[] args) where T : SubView, new()
        {
            foreach (SubView subView in _subViewDic.Values)
            {
                if (!(subView is T))
                {
                    subView.CloseSubView();
                }
            }

            OpenSubView<T>(args);
        }

        public void CloseAllSubView()
        {
            foreach (SubView subView in _subViewDic.Values)
            {
                subView.CloseSubView();
            }
        }

        public void SetViewVisible(bool visible)
        {
            if (visible)
            {
                _viewTrans.SetLocalScale_EX(1, 1, 1);
            }
            else
            {
                _viewTrans.SetLocalScale_EX(0, 0, 0);
            }
        }

        /// <summary>
        /// 添加新的子界面 非独立prefab
        /// </summary>
        /// <param name="gameObject"></param>
        /// <typeparam name="T"></typeparam>
        protected void AddObjSubView<T>(BindObjLib bindObjLib) where T : ObjSubView, new()
        {
            SubView subView = AddSubView<T>();
            ObjSubView objSubView = subView as ObjSubView;
            objSubView.SetSubViewGo(bindObjLib);
        }

        private SubView AddSubView<T>() where T : SubView, new()
        {
            string viewName = getSubViewwName<T>();
            JQLog.LogWarrningFormat("AddSubView:{0}", viewName);

            SubView subView = null;
            _subViewDic.TryGetValue(viewName, out subView);
            if (subView == null)
            {
                subView = new T();
                subView.viewParent = this;
                _subViewDic[viewName] = subView;
            }

            return subView;
        }

        public void OpenSubView<T>(params object[] args) where T : SubView, new()
        {
            string viewName = getSubViewwName<T>();
            JQLog.LogWarrningFormat("OpenSubView:{0}", viewName);
            SubView subView = null;
            _subViewDic.TryGetValue(viewName, out subView);


            Sys.gameDispatcher.TriggerEvent(FrameworkEvent.OpenView);
            subView.OpenSubView(args);
        }

        private string getSubViewwName<T>() where T : SubView
        {
            Type viewType = typeof(T);
            string viewName = null;
            //如果还没有记录名称，则从
            if (!_subViewType2NameDic.TryGetValue(viewType, out viewName))
            {
                SubViewNameAttribute attribute =
                    (SubViewNameAttribute)Attribute.GetCustomAttribute(viewType, typeof(SubViewNameAttribute));
                viewName = attribute.Name;
                _subViewType2NameDic[viewType] = viewName;
            }

            return viewName;
        }


        private string fullUrl
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

        public void ReadyLoad()
        {
            if (_viewGo == null)
            {
                UrlPrefabLoaderUtil.LoadAsset(fullUrl, null, true);
            }
        }
        
        public void OpenView(params object[] args)
        {
            Sys.gameDispatcher.TriggerEvent(FrameworkEvent.OpenView);
            _params = args;
            //已经打开
            if (_isOpened && _viewGo != null)
            {
                UpdateSortingOrder();
                OnViewRefresh();
                Sys.gameDispatcher.TriggerEvent(FrameworkEvent.OpenOverRootView, this);
                return;
            }

            // Log.Error("IsOpened true:"+GetViewName());
            _isOpened = true;
            //未加载
            if (_viewGo == null)
            {
                UrlPrefabLoaderUtil.LoadAsset(fullUrl, OnLoadFinish);
            }
            else
            {
                RealOpenView();
            }
        }

        private void UpdateSortingOrder()
        {
            ViewSortingOrderMgr.Instance.removeView(this);
            ViewSortingOrderMgr.Instance.addView(this, _sortingOrder);
            _viewTrans.SetAsLastSibling();
        }

        private void OnLoadFinish(UrlPrefabLoader urlPrefabLoader)
        {
            if (!_isOpened) return;
            JQLog.LogWarrningFormat("View OnLoadFinish:{0} frame:{1}", _url, Time.frameCount);
            
            Profiler.BeginSample($"RootView.OnLoadFinish:{_url}");
            
            
            Profiler.BeginSample($"OnLoadFinish1");
            _viewGo = urlPrefabLoader.Spawn();
            UnityUtil.SetLayer(_viewGo, ELayer.UI);
            Profiler.EndSample();

            Profiler.BeginSample($"OnLoadFinish2");
            _viewTrans = _viewGo.transform as RectTransform;
            _viewTrans.SetParent_EX(_layerTrans);
            _viewTrans.SetOffsetMin_EX(0, 0);
            _viewTrans.SetOffsetMax_EX(0, 0);
            _viewTrans.SetLocalScale_EX(1, 1, 1);
            Profiler.EndSample();


            Profiler.BeginSample($"OnLoadFinish3");
            _bindObjLib = _viewGo.GetComponent<BindObjLib>();
            _bindObjLib.InitDic();
            Profiler.EndSample();

            Profiler.BeginSample($"OnLoadFinish4");
            //屏幕刘海屏适配
            _panelFunctionRectTrans = _bindObjLib.GetObjByKey<RectTransform>("panel_function", false);
            AdaptPhone_UI();
            Profiler.EndSample();
            
            Profiler.BeginSample($"OnLoadFinish5");
            OnBindAttr();
            OnInitSubView();
            OnViewLoadFinish();
            Profiler.EndSample();

            Profiler.BeginSample($"OnLoadFinish6");
            RealOpenView();
            Profiler.EndSample();
            
            Profiler.EndSample();
        }

        private void RealOpenView()
        {
            
            Profiler.BeginSample($"RealOpenView.Other");
            _viewGo.SetActive_EX(true);
            if (_openTween)
            {
                RectTransform tweenRect = _viewTweenRectTrans ?? _panelFunctionRectTrans;
                tweenRect.localScale = Vector3.zero;
                tweenRect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            }
            Profiler.EndSample();
            
            
            Profiler.BeginSample($"RealOpenView.UpdateSortingOrder");
            //TODO: 屏幕刘海屏适配 s3 AdaptPhone_UI
            UpdateSortingOrder();
            Profiler.EndSample();
            try
            {
                
                Profiler.BeginSample($"RealOpenView.OnAddEvent");
                OnAddEvent();
                Profiler.EndSample();
            }
            catch (Exception e)
            {
                JQLog.LogException(e);
            }

            Profiler.BeginSample($"RealOpenView.OnViewOpen");
            OnViewOpen();
            Profiler.EndSample();
            
            Profiler.BeginSample($"RealOpenView.TriggerEvent");
            Sys.gameDispatcher.TriggerEvent(FrameworkEvent.OpenOverRootView, this);
            Profiler.EndSample();
        }

        /// <summary>
        /// 返回键处理
        /// </summary>
        /// <returns>true 为执行CloseView</returns>
        public virtual bool OnEscCloseView()
        {
            return true;
        }

        public ViewType ViewType => _viewType;

        public void CloseView()
        {
            foreach (SubView subView in _subViewDic.Values)
            {
                subView.CloseSubView();
            }


            // Log.Error("IsOpened false:"+GetViewName());
            _isOpened = false;
            //移除计时器
            Sys.timerMgr.removeByCaller(this);
            Sys.highTimerMgr.removeByCaller(this);

            //移除界面Res
            UrlPrefabLoaderUtil.CancelLoadAsset(fullUrl, OnLoadFinish);


            if (_viewGo != null)
            {
                //TODO：动态图片加载器取消加载

                //video卸载
                // VideoMgr.Instance.releaseVideo(this);

                //动态图片加载器取消加载
                DynamicImageLoaderMgr.Instance.removeDynamicLoaderByParent(this);

                removeAllDisplay();
                OnViewClose();
                OnRemoveEvent();

                //TODO:ViewSortingOrderMgr.removeView

                _viewGo.SetActive_EX(false);
            }
        }

        public void AdaptPhone_UI()
        {
            if (_panelFunctionRectTrans != null)
            {
                SafeAreaUtil.AdaptPhone_UI(_panelFunctionRectTrans);
            }
        }

        public void DestroyView()
        {
            if (_viewGo == null) return;
            OnRemoveEvent();
            OnViewDestory();

            foreach (SubView subBaseView in _subViewDic.Values)
            {
                subBaseView.DestroySubView();
            }

            if (ViewStackMgr.Instance != null)
            {
                ViewStackMgr.Instance.clearSubViewStack(this);
            }

            OnUnBindAttr();
            UrlPrefabLoaderUtil.DeSpawnPerformance(fullUrl, _viewGo);
            _btnLastClickTimeDic.Clear();
            _viewGo = null;
            _viewTrans = null;
        }


        //界面内部使用
        protected void closeNextFrame()
        {
            Sys.timerMgr.addOnce(0f, this, CloseView);
        }

        #region 抽象方法

        //初始化子界面（说明子界面是哪些）
        protected abstract void OnInitSubView();

        //添加
        protected abstract void OnAddEvent();

        //移除
        protected abstract void OnRemoveEvent();

        //初始化绑定
        protected abstract void OnBindAttr();

        //取消绑定
        protected abstract void OnUnBindAttr();

        //界面加载完毕（除了bind之外其他的特殊操作）
        protected abstract void OnViewLoadFinish();

        //界面卸载
        protected abstract void OnViewDestory();

        //界面打开（加载完毕后）
        protected abstract void OnViewOpen();

        //界面刷新（界面打开时）
        protected abstract void OnViewRefresh();

        //界面关闭处理
        protected abstract void OnViewClose();

        #endregion


        public override string GetViewName()
        {
            if (_viewName != null) return _viewName;
            Type viewType = this.GetType();
            ViewNameAttribute attribute =
                (ViewNameAttribute)Attribute.GetCustomAttribute(viewType, typeof(ViewNameAttribute));
            string viewName = attribute.Name;
            _viewName = viewName;
            return viewName;
        }
    }
}