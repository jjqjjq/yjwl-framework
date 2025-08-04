using System;
using JQCore;
using JQCore.tLog;
using JQCore.DynamicTexture;
using JQCore.tEnum;
using JQCore.tRes;
using JQCore.tUtil;
using JQFramework.tMVC.Attributes;
using UnityEngine;

namespace JQFramework.tMVC.Base
{
    public abstract class SubView:BaseView
    {
        
        
        protected object[] _params = null;
        public RootView viewParent;
        protected GameObject _subViewGo;
        protected RectTransform _subViewTrans;

        protected BindObjLib _bindObjLib = null;


        private void addStack()
        {
            if (!isStackView) return;
            ViewStackMgr.Instance.addSubViewStack(viewParent, this);
        }

        private void removeStack()
        {
            if (!isStackView) return;
            ViewStackMgr.Instance.removeSubViewStack(viewParent, this);
        }
        
        
        protected void RealOpenView()
        {
            addStack();
            _subViewGo.SetActive_EX(true);
            try
            {
                OnAddEvent();
            }
            catch (Exception e)
            {
                JQLog.LogException(e);
            }

            OnSubViewOpen();
            Sys.gameDispatcher.TriggerEvent(FrameworkEvent.OpenOverView);
        }

        public virtual void CloseSubView()
        {
            removeStack();
            _isOpened = false;
            //移除计时器
            Sys.timerMgr.removeByCaller(this);
            Sys.highTimerMgr.removeByCaller(this);
            
            if (_subViewGo != null)
            {
                //动态图片加载器取消加载
                DynamicImageLoaderMgr.Instance.removeDynamicLoaderByParent(this);
                
                removeAllDisplay();
                OnSubViewClose();
                OnRemoveEvent();

                _subViewGo.SetActive_EX(false);
            }
        }

        public virtual void DestroySubView()
        {
            if (_subViewGo == null) return;
            OnRemoveEvent();
            OnSubViewDestory();
            
            OnUnBindAttr();
            _btnLastClickTimeDic.Clear();
            _subViewGo = null;
            _subViewTrans = null;
        }



        #region 抽象方法

        //添加
        protected abstract void OnAddEvent();

        //移除
        protected abstract void OnRemoveEvent();

        //初始化绑定
        protected abstract void OnBindAttr();

        //取消绑定
        protected abstract void OnUnBindAttr();
        

        //界面加载完毕（除了bind之外其他的特殊操作）
        protected abstract void OnSubViewLoadFinish();

        //界面卸载
        public abstract void OnSubViewDestory();

        //界面打开（加载完毕后）
        protected abstract void OnSubViewOpen();

        //界面刷新（界面打开时）
        protected abstract void OnSubViewRefresh();

        //界面关闭处理
        public abstract void OnSubViewClose();

        public abstract void OpenSubView(params object[] args);
        #endregion
        
        
        
        
        public override string GetViewName()
        {
            Type viewType = this.GetType();
            SubViewNameAttribute attribute =
                (SubViewNameAttribute)Attribute.GetCustomAttribute(viewType, typeof(SubViewNameAttribute));
            string viewName = attribute.Name;
            return viewName;
        }
    }
}