using System.Collections.Generic;
using JQCore.tSingleton;
using JQFramework.tMgr;
using JQFramework.tMVC.Base;
using UnityEngine;

namespace JQFramework.tMVC.sortingOrder
{
    public class ViewSortingOrderMgr:JQSingleton<ViewSortingOrderMgr>
    {
        public const int LAYER_RANGE = 5000;

        private Dictionary<Transform, ViewSortingOrderLayer> _layerDic;

        public override void Dispose()
        {
            _layerDic.Clear();
        }

        public ViewSortingOrderMgr()
        {
            _layerDic = new Dictionary<Transform, ViewSortingOrderLayer>();
            addLayer(HierarchyLayerMgr.MainUI);
            addLayer(HierarchyLayerMgr.BaseUI);
            addLayer(HierarchyLayerMgr.ViewUI);
            addLayer(HierarchyLayerMgr.FrontUI);
            addLayer(HierarchyLayerMgr.FrontFrontUI);
        }

        private void addLayer(Transform layerTrans)
        {
            int sortingOrder = (_layerDic.Count + 1) * LAYER_RANGE;
            _layerDic[layerTrans] = new ViewSortingOrderLayer(layerTrans, sortingOrder);
        }

        public void addView(RootView rootView, int sortingOrder)
        {
            ViewSortingOrderLayer layer = _layerDic[rootView.LayerTrans];
            layer.addView(rootView, sortingOrder);
        }

        public void removeView(RootView rootView)
        {
            ViewSortingOrderLayer layer = _layerDic[rootView.LayerTrans];
            layer.removeView(rootView);
        }

        public void SetSortingAsLastSibling(RootView rootView, int sortingOrder)
        {
            ViewSortingOrderLayer layer = _layerDic[rootView.LayerTrans];
            layer.SetSortingAsLastSibling(rootView, sortingOrder);
        }

        public int getViewSortingOrder(RootView rootView)
        {
            ViewSortingOrderLayer layer = _layerDic[rootView.LayerTrans];
            return layer.getViewSortingOrder(rootView);
        }

        public void setSubObjOrderByCls<T>(GameObject rootGo, int subOrder, bool isAdd) where T :RootView
        {
            RootView rootView = UIManager.GetRootView<T>();
            if (rootView.viewGo)
            {
                setSubObjOrder(rootView, rootGo, subOrder, isAdd);
            }
        }
        
        public void removeSubObjOrderByCls<T>(GameObject rootGo, int subOrder, bool isAdd) where T :RootView
        {
            RootView rootView = UIManager.GetRootView<T>();
            if (rootView.viewGo)
            {
                removeSubObjOrder(rootView, rootGo, subOrder, isAdd);
            }
        }
        
        private void setSubObjOrder(RootView rootView, GameObject rootGo, int subOrder, bool isAdd)
        {
            ViewSortingOrderLayer layer = _layerDic[rootView.LayerTrans];
            layer.setSubObjOrder(rootView, rootGo, subOrder, isAdd);
        }
        
        private void removeSubObjOrder(RootView rootView, GameObject rootGo, int subOrder, bool isAdd)
        {
            ViewSortingOrderLayer layer = _layerDic[rootView.LayerTrans];
            layer.removeSubObjOrder(rootView, rootGo, subOrder, isAdd);
        }

        public int getLayerOrder(Transform layerTran)
        {
            ViewSortingOrderLayer layer = _layerDic[layerTran];
            return layer.LayerSortingOrder;
        }
    }
}