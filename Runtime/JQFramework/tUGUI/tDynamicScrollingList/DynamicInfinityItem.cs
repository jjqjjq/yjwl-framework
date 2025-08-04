using System;
using System.Collections;
using System.Collections.Generic;
using JQCore.tLog;
using JQCore.tRes;
using JQCore.tUtil;
using JQCore.Log;
using UnityEngine;
using UnityEngine.UI;

using Object = UnityEngine.Object;

namespace JQFramework.tUGUI.tDynamicScrollingList
{
    /// <summary>
    /// 动态格子矩形
    /// </summary>

    public class DynamicRect
    {
        /// <summary>
        /// 矩形数据
        /// </summary>
        private Rect mRect;
        /// <summary>
        /// 格子索引
        /// </summary>
        public int Index;
        public DynamicRect()
        {
            
        }

        public void UpdateRect(float x, float y, float width, float height, int index)
        {
            this.Index = index;
            mRect = new Rect(x, y, width, height);
        }

        /// <summary>
        /// 是否相交
        /// </summary>
        /// <param name="otherRect"></param>
        /// <returns></returns>
        public bool Overlaps(DynamicRect otherRect)
        {
            return mRect.Overlaps(otherRect.mRect);
        }

        /// <summary>
        /// 是否相交
        /// </summary>
        /// <param name="otherRect"></param>
        /// <returns></returns>
        public bool Overlaps(Rect otherRect)
        {
            bool xx = Overlaps(otherRect.xMin, otherRect.xMax, otherRect.yMin, otherRect.yMax, mRect.xMin, mRect.xMax, mRect.yMin, mRect.yMax);
            return xx;
        }

        private bool Overlaps(float xMin1, float xMax1, float yMin1, float yMax1, float xMin2, float xMax2, float yMin2, float yMax2)
        {
            return (double) xMax1 > (double) xMin2 && (double) xMin1 < (double) xMax2 && (double) yMax1 > (double) yMin2 && (double) yMin1 < (double) yMax2;
        }
        
        public override string ToString()
        {
            return string.Format("index:{0},x:{1},y:{2},w:{3},h:{4}", Index, mRect.x, mRect.y, mRect.width, mRect.height);
        }
    }

    /// <summary>
    /// 动态无限渲染器
    /// </summary>

    public class DynamicScrollingItem : MonoBehaviour
    {
        public Action<DynamicScrollingItem> OnUpdateDataHandler;
        public Action<DynamicScrollingItem> OnItemInitHandler;
        private BindObjLib _bindObjLib;
        
        private Dictionary<string, Object> _dic = new Dictionary<string, Object>();
        // private LuaTable _linkLuaTable = null;
        /// <summary>
        /// 动态矩形
        /// </summary>
        protected DynamicRect mDRect;

        public void SetBindObjLib(BindObjLib bindObjLib)
        {
            _bindObjLib = bindObjLib;
            _bindObjLib.InitDic();
        }

        void OnDestroy()
        {
            OnUpdateDataHandler = null;
            OnItemInitHandler = null;
            _dic.Clear();
            _dic = null;

            mDRect = null;
            _bindObjLib = null;

            // if (_linkLuaTable != null)
            // {
            //     _linkLuaTable.Dispose();
            //     _linkLuaTable = null;
            // }
        }

        public DynamicRect DRect
        {
            set
            {
                mDRect = value;
            }
            get { return mDRect; }
        }

        public int DataIndex
        {
            get { return DRect.Index; }
        }

        // public LuaTable GetLuaTable()
        // {
        //     if (_linkLuaTable == null)
        //     {
        //         _linkLuaTable = HyXLua.LuaEnv.NewTable();
        //     }
        //     return _linkLuaTable;
        // }

        public void UpdateActive()
        {
            bool active = mDRect != null;
            if (gameObject.activeSelf != active)
            {
                gameObject.SetActive_EX(active);
            }
        }

        public void addBehaviour(string key, Object obj)
        {
            //        Debug.Log("addBehaviour:" + keyIndex);
            // string key = LuaStrKeyMgr.Instance.getStrByIndex(keyIndex);
            if (obj == null)
            {
                JQLog.LogError("传入组件为空:"+key);
                return;
            }
            _dic[key] = obj;
        }

        public Object getObjFromLib(string key)
        {
            return _bindObjLib.GetObjByKey(key);
        }

        public Object getBehaviour(string key)
        {
//        Debug.Log("getBehaviour:"+keyIndex);
            // string key = LuaStrKeyMgr.Instance.getStrByIndex(keyIndex);
            Object obj = null;
            if (!_dic.TryGetValue(key, out obj))
            {
                JQLog.LogError("查无对应组件：" + key);
            }
            return obj;
        }

        void Start()
        {
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        public void SetData()
        {
            if (null != OnUpdateDataHandler)
            {
                OnUpdateDataHandler(this);
            }
        }
    }
}