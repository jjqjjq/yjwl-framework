using System;
using System.Collections.Generic;
using JQCore.tLog;
using JQCore.tSingleton;
using JQFramework.tMVC.Base;

namespace JQFramework.tMVC
{
    
    
    public class ViewStackMgr:JQSingleton<ViewStackMgr>
    {
        
        private Dictionary<RootView, List<SubView>> _viewDic = new Dictionary<RootView, List<SubView>>();

        public override void Dispose()
        {
            _viewDic.Clear();
        }

        private List<SubView> getViewStack(RootView rootView)
        {
            _viewDic.TryGetValue(rootView, out var subViewList);
            if(subViewList == null)
            {
                subViewList = new List<SubView>();
                _viewDic[rootView] = subViewList;
            }
            return subViewList;
        }
        
        public void addSubViewStack(RootView rootView, SubView subView)
        {
            List<SubView> subViewList = getViewStack(rootView);
            subViewList.Remove(subView);
            subViewList.Add(subView);
        }

        public void removeSubViewStack(RootView rootView, SubView subView)
        {
            List<SubView> subViewList = getViewStack(rootView);
            subViewList.Remove(subView);
        }
        
        public void clearSubViewStack(RootView rootView)
        {
            List<SubView> subViewList = getViewStack(rootView);
            subViewList.Clear();
        }
        
        public bool closeTopSubView(RootView rootView)
        {
            List<SubView> subViewList = getViewStack(rootView);
            if (subViewList.Count > 0)
            {
                SubView subView = subViewList[subViewList.Count - 1];
                if (subView.IsOpened)
                {
                    subViewList.Remove(subView);
                    subView.CloseSubView();
                    return true;
                }
                else
                {
                    JQLog.LogError($"界面关闭了却还在队列中:{rootView.GetType().Name},{subView.GetType().Name}");
                    return false;
                }
            }
            return false;
        }
        
    }
}