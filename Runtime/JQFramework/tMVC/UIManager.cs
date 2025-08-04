using System;
using System.Collections.Generic;
using JQCore;
using JQCore.tLog;
using JQCore.tSingleton;
using JQFramework.tMVC.Base;
using JQCore.tEnum;
using JQCore.tString;
using JQCore.tTime;
using JQCore.tUtil;
using JQFramework.tMgr;
using JQFramework.tMVC.Attributes;
using JQFramework.tMVC.sortingOrder;
using UnityEngine;

namespace JQFramework.tMVC
{
    public class UIManager : JQSingleton<UIManager>, ITick
    {
        private Dictionary<Type, string> _viewType2NameDic = new Dictionary<Type, string>();
        private Dictionary<string, RootView> _viewDic = new Dictionary<string, RootView>();

        public UIManager()
        {
            TickMgr.Instance.addTick(this);
        }

        public static void updateSafeArea()
        {
            (float safeLeftRate, float safeRightRate, float safeTopRate, float safeBottomRate) =
                Sys.sdkMgr.GetSafeAreaInfo();
            SafeAreaUtil.SetDeviceSafeArea(safeLeftRate, safeRightRate, safeTopRate, safeBottomRate);
            UpdatePhoneAdapter();
        }

        public static void UpdatePhoneAdapter()
        {
            Instance.UpdatePhoneAdapterr();
        }


        private void UpdatePhoneAdapterr()
        {
            // Apply displacement to all views under viewUI
            foreach (RootView viewDicValue in _viewDic.Values)
            {
                viewDicValue.AdaptPhone_UI();
            }
        }

        public override void Dispose()
        {
            foreach (RootView viewDicValue in _viewDic.Values)
            {
                viewDicValue.DestroyView();
            }

            _viewDic.Clear();
        }

        public static RootView GetRootView<T>() where T : RootView
        {
            return Instance.getRootVieww<T>();
        }

        public static RootView GetRootView(string viewName)
        {
            return Instance.GetRootVieww(viewName);
        }

        public RootView GetRootVieww(string viewName)
        {
            RootView view = null;
            _viewDic.TryGetValue(viewName, out view);
            return view;
        }

        private RootView getRootVieww<T>() where T : RootView
        {
            string viewName = getViewName<T>();
            RootView view = null;
            _viewDic.TryGetValue(viewName, out view);
            return view;
        }


        private static string getViewName<T>() where T : RootView
        {
            return Instance.getViewwName<T>();
        }

        private string getViewwName<T>() where T : RootView
        {
            Type viewType = typeof(T);
            string viewName = null;
            //如果还没有记录名称，则从
            if (!_viewType2NameDic.TryGetValue(viewType, out viewName))
            {
                ViewNameAttribute attribute =
                    (ViewNameAttribute)Attribute.GetCustomAttribute(viewType, typeof(ViewNameAttribute));
                viewName = attribute.Name;
                _viewType2NameDic[viewType] = viewName;
            }

            return viewName;
        }

        public static void OpenView<T>(params object[] args) where T : RootView, new()
        {
            Instance.OpenVieww<T>(args);
        }


        public static void ReadyLoad<T>()where T : RootView, new()
        {
            Instance.ReadyLoadd<T>();
        }

        public static bool IsViewOpened<T>() where T : RootView
        {
            return Instance.IsViewOpenedd<T>();
        }

        private bool IsViewOpenedd<T>() where T : RootView
        {
            string viewName = getViewwName<T>();
            RootView view = null;
            _viewDic.TryGetValue(viewName, out view);
            if (view == null)
            {
                return false;
            }

            return view.IsOpened;
        }

        public void ReadyLoadd<T>() where T : RootView, new()
        {
            try
            {
                string viewName = getViewwName<T>();
                JQLog.LogWarrningFormat("ReadyLoadd:{0}", viewName);

                RootView view = null;
                _viewDic.TryGetValue(viewName, out view);
                if (view == null)
                {
                    view = new T();
                    _viewDic[viewName] = view;
                }

                view.ReadyLoad();
            }
            catch (Exception e)
            {
                JQLog.LogError("ReadyLoadViewException:" + e.ToString() + e.StackTrace);
            }
        }

        private void OpenVieww<T>(params object[] args) where T : RootView, new()
        {
            try
            {
                string viewName = getViewwName<T>();
                JQLog.LogWarrningFormat("OpenView:{0} frame:{1}", viewName, Time.frameCount);

                RootView view = null;
                _viewDic.TryGetValue(viewName, out view);
                if (view == null)
                {
                    view = new T();
                    _viewDic[viewName] = view;
                }

                view.OpenView(args);
            }
            catch (Exception e)
            {
                JQLog.LogError("OpenViewException:" + e.ToString() + e.StackTrace);
            }
        }

        public static void SetViewVisible<T>(bool visible) where T : RootView
        {
            Instance.SetViewVisiblee<T>(visible);
        }

        private void SetViewVisiblee<T>(bool visible) where T : RootView
        {
            string viewName = getViewwName<T>();
            JQLog.LogWarrningFormat("SetViewVisible:{0}", viewName);
            RootView view = null;
            _viewDic.TryGetValue(viewName, out view);
            if (view != null)
            {
                view.SetViewVisible(visible);
            }
        }

        public static void CloseView<T>() where T : RootView
        {
            Instance.CloseVieww<T>();
        }

        private void CloseVieww<T>() where T : RootView
        {
            string viewName = getViewwName<T>();
            RootView view = null;
            _viewDic.TryGetValue(viewName, out view);
            if (view != null && view.IsOpened)
            {
                JQLog.LogWarrningFormat("CloseView:{0} frame:{1}", viewName, Time.frameCount);
                view.CloseView();
                Sys.gameDispatcher.TriggerEvent(FrameworkEvent.CloseView);
            }
        }

        private static bool closeing = false;

        public static void CloseAllView()
        {
            Instance.CloseAllVieww();
        }

        public void CloseAllVieww()
        {
            closeing = true;
            foreach (RootView rootView in _viewDic.Values)
            {
                switch (rootView.ViewType)
                {
                    case ViewType.Main:
                        // case ViewType.NoRestrain:
                        break;
                    default:
                        rootView.CloseView();
                        break;
                }
            }

            closeing = false;
        }

        //释放较久未使用的界面
        public static void DestoryOldViews()
        {
            //todo: DestoryOldViews
        }


        public bool CloseTopView()
        {
            int maxSiblingIndex = 0;
            RootView topView = null;
            foreach (RootView rootView in _viewDic.Values)
            {
                if (rootView.IsOpened && rootView.isStackView)
                {
                    int viewSortingOrder = ViewSortingOrderMgr.Instance.getViewSortingOrder(rootView);
                    if (viewSortingOrder > maxSiblingIndex)
                    {
                        maxSiblingIndex = viewSortingOrder;
                        topView = rootView;
                    }
                }
            }

            if (topView != null)
            {
                bool hadClose = ViewStackMgr.Instance.closeTopSubView(topView);
                if (!hadClose)
                {
                    //子界面都关闭了，关闭主界面
                    bool execCloseViewFunc = topView.OnEscCloseView();
                    if (execCloseViewFunc)
                    {
                        topView.CloseView();
                    }
                }

                return false;
            }

            return true;
        }

        public void SystemMessage(string msg)
        {
            // ChatCtrl.SendChatMessage(0, "【系统】", msg);
        }

        public void onTick()
        {
            if (SafeAreaUtil.IsOrientationChanged())
            {
                updateSafeArea();
                SafeAreaUtil.currScreenOrientation = Screen.orientation;
            }
        }
    }
}