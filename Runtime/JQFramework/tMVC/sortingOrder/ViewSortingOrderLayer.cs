using System.Collections.Generic;
using JQCore.tLog;
using JQCore.tMgr;
using JQFramework.tMVC.Base;
using JQCore.tRes;
using JQCore.tUtil;
using JQFramework.tMgr;
using UnityEngine;
using UnityEngine.UI;

namespace JQFramework.tMVC.sortingOrder
{
    public class ViewSortingOrderLayer
    {
        public const int VIEW_RANGE = 100;
        private Transform _layerTrans;
        private int _layerSortingOrder;

        public int LayerSortingOrder => _layerSortingOrder;

        private Canvas _layerCanvas;
        private Dictionary<string, Canvas> _viewCanvasDic;

        public ViewSortingOrderLayer(Transform layerTrans, int layerSortingOrder)
        {
            _layerTrans = layerTrans;
            _layerSortingOrder = layerSortingOrder;
            _layerCanvas = layerTrans.AddComponent_EX<Canvas>();
            _layerCanvas.overrideSorting = true;
            _layerCanvas.sortingOrder = layerSortingOrder;
            _viewCanvasDic = new Dictionary<string, Canvas>();
        }

        private int getTopViewOrder()
        {
            int topOrder = _layerSortingOrder;
            foreach (var keyValuePairs in _viewCanvasDic)
            {
                string key = keyValuePairs.Key;
                Canvas canvas = keyValuePairs.Value;
                if (canvas == null)
                {
                    JQLog.LogError($"Canvas is null:{key}");
                }
                else
                {
                    if (canvas.gameObject.activeSelf && canvas.sortingOrder > topOrder)
                    {
                        topOrder = canvas.sortingOrder;
                    }
                }
            }

            return topOrder;
        }

        public void addView(RootView rootView, int sortingOrder)
        {
            GameObject viewGo = rootView.viewGo;
            string viewName = rootView.GetViewName();
            //如果是FrontUI的界面必需指定sortingOrder
            if (sortingOrder == 0 && rootView.LayerTrans == HierarchyLayerMgr.FrontUI)
            {
                JQLog.LogError($"FrontUI的界面必需指定sortingOrder: GConst.ViewSortingOrder:{viewName}");
                return;
            }

            if (viewGo == null)
            {
                JQLog.LogError($"界面未加载完毕:{viewName}");
                return;
            }

            _viewCanvasDic.TryGetValue(viewName, out var canvas);
            if (canvas != null)
            {
                return;
            }

            int rootOrder;
            if (sortingOrder != 0)
            {
                rootOrder = _layerSortingOrder + sortingOrder;
            }
            else
            {
                int topViewOrder = getTopViewOrder();
                rootOrder = topViewOrder + VIEW_RANGE;
            }

            //把界面内部的所有子对象的sortingOrder都增加rootOrder个层次
            MVCUtil.setAllSubObjectOrder(viewGo, rootOrder, true);
            canvas = rootView.sortingCanvas;
            if (canvas == null)
            {
                rootView.sortingCanvas = UnityUtil.AddMissingComponent<Canvas>(viewGo);
                rootView.sortingGraphicRaycaster = UnityUtil.AddMissingComponent<GraphicRaycaster>(viewGo);
                canvas = rootView.sortingCanvas;
                canvas.overrideSorting = true;
            }

            canvas.sortingOrder = rootOrder;
            _viewCanvasDic[viewName] = canvas;
        }

        public void setSubObjOrder(RootView rootView, GameObject rootGo, int subOrder, bool isAdd)
        {
            string viewName = rootView.GetViewName();
            _viewCanvasDic.TryGetValue(viewName, out var canvas);
            if (canvas == null)
            {
                JQLog.LogError($"cant find viewGoName:{viewName} go:{rootGo}");
                return;
            }

            int rootOrder = canvas.sortingOrder;
            GameObject viewGo = rootView.viewGo;
            MVCUtil.setAllSubObjectOrder(viewGo, rootOrder + subOrder, isAdd, rootGo);
        }

        public void removeSubObjOrder(RootView rootView, GameObject rootGo, int subOrder, bool isAdd)
        {
            string viewName = rootView.GetViewName();
            _viewCanvasDic.TryGetValue(viewName, out var canvas);
            if (canvas == null)
            {
                JQLog.LogError($"cant find viewGoName:{viewName} go:{rootGo}");
                return;
            }

            int rootOrder = canvas.sortingOrder;
            GameObject viewGo = rootView.viewGo;
            MVCUtil.setAllSubObjectOrder(viewGo, -(rootOrder + subOrder), isAdd, rootGo);
        }

        public void SetSortingAsLastSibling(RootView rootView, int sortingOrder = 0)
        {
            GameObject viewGo = rootView.viewGo;
            string viewName = rootView.GetViewName();
            _viewCanvasDic.TryGetValue(viewName, out var canvas);
            if (canvas == null)
            {
                addView(rootView, sortingOrder);
                return;
            }

            int rootOrder;
            if (sortingOrder != 0)
            {
                rootOrder = _layerSortingOrder + sortingOrder;
            }
            else
            {
                int topViewOrder = getTopViewOrder();
                rootOrder = topViewOrder + VIEW_RANGE;
            }

            int oldOrder = canvas.sortingOrder;
            MVCUtil.setAllSubObjectOrder(viewGo, -oldOrder + rootOrder, true);

            canvas.sortingOrder = rootOrder;
        }

        public void removeView(RootView rootView)
        {
            GameObject viewGo = rootView.viewGo;
            string viewName = rootView.GetViewName();
            _viewCanvasDic.TryGetValue(viewName, out var canvas);
            if (canvas == null)
            {
                return;
            }

            int rootOrder = canvas.sortingOrder;
            MVCUtil.setAllSubObjectOrder(viewGo, -rootOrder, true);
            _viewCanvasDic.Remove(viewName);
        }

        public int getViewSortingOrder(RootView rootView)
        {
            if (rootView.viewGo == null)
            {
                JQLog.LogError("viewGo is null");
                return 0;
            }

            string viewName = rootView.GetViewName();
            _viewCanvasDic.TryGetValue(viewName, out var canvas);
            if (canvas)
            {
                return canvas.sortingOrder;
            }
            return 0;
        }
    }
}