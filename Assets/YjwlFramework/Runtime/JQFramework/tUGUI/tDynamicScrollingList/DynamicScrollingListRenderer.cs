using System;
using System.Collections;
using System.Collections.Generic;
using JQCore.tRes;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace JQFramework.tUGUI.tDynamicScrollingList
{
    /// <summary>
    /// 动态无限列表
    /// </summary>

    public class DynamicScrollingListRenderer : MonoBehaviour
    {
        private static readonly ObjectPool<DynamicRect> _dynRectPool = new ObjectPool<DynamicRect>(createDynamicRect);

        private static DynamicRect createDynamicRect()
        {
            return new DynamicRect();
        }
        
        /// <summary>
        /// 单元格尺寸（宽，高）
        /// </summary>
        public Vector2 CellSize;
        /// <summary>
        /// 单元格间隙（水平，垂直）
        /// </summary>
        public Vector2 SpacingSize;
        /// <summary>
        /// 是否上下拖动加载
        /// </summary>
        public bool isVertical = true;
        /// <summary>
        /// 列数/行数
        /// </summary>
        public int RowColumnCount;
        /// <summary>
        /// prefab
        /// </summary>
        public GameObject RenderGO;
        /// <summary>
        /// mScrollRect
        /// </summary>
        public ScrollRect mScrollRect;
        /// <summary>
        /// 渲染格子数
        /// </summary>
        protected int mRendererCount;
        /// <summary>
        /// 父节点蒙版尺寸
        /// </summary>
        private Vector2 mMaskSize;
        /// <summary>
        /// 蒙版矩形
        /// </summary>
        private Rect mRectMask;
        /// <summary>
        /// 转换器
        /// </summary>
        protected RectTransform mRectTransformContainer;
        /// <summary>
        /// 渲染脚本集合
        /// </summary>
        protected List<DynamicScrollingItem> mList_items;
        /// <summary>
        /// 渲染格子字典
        /// </summary>
        private DynamicRect[] _dynRectArr;
        /// <summary>
        /// 数据总数量
        /// </summary>
        protected int mDataCount = 0;
        protected bool mHasInited = false;
        /// <summary>
        /// 初始化渲染脚本
        /// </summary>
        public virtual void InitRendererList(Action<DynamicScrollingItem> OnCreate, Action<DynamicScrollingItem> OnUpdate)
        {
            if (mHasInited) return;
            //转换器
            mRectTransformContainer = transform as RectTransform;
            //获得蒙版尺寸
            mMaskSize = mScrollRect.GetComponent<RectTransform>().rect.size;
            //通过蒙版尺寸和格子尺寸计算需要的渲染器个数
            mRendererCount = isVertical ? RowColumnCount * (Mathf.CeilToInt(mMaskSize.y / GetBlockSizeY()) + 1) : RowColumnCount * (Mathf.CeilToInt(mMaskSize.x / GetBlockSizeX()) + 1);
            //        HyDebug.Log("mRendererCount:"+mRendererCount);

            _UpdateDynmicRects(mRendererCount);
            mList_items = new List<DynamicScrollingItem>();
            for (int i = 0; i < mRendererCount; ++i)
            {
                GameObject child = GameObject.Instantiate(RenderGO);
                child.transform.SetParent(transform);
                child.transform.localRotation = Quaternion.identity;
                child.transform.localScale = Vector3.one;
                child.layer = gameObject.layer;
                DynamicScrollingItem dfItem = child.GetComponent<DynamicScrollingItem>();
                if (dfItem == null)
                {
                    dfItem = child.AddComponent<DynamicScrollingItem>();
                }
                BindObjLib bindObjLib = child.GetComponent<BindObjLib>();
                if (bindObjLib)
                {
                    dfItem.SetBindObjLib(bindObjLib);
                }

                if (OnCreate != null)
                {
                    OnCreate(dfItem);
                }
                dfItem.OnUpdateDataHandler = OnUpdate;

                dfItem.DRect = _dynRectArr[i];
                dfItem.gameObject.name = i.ToString();
                child.SetActive(false);
                mList_items.Add(dfItem);
                _UpdateChildTransformPos(child, i);
            }
            _SetListRenderSize(mRendererCount);
            mHasInited = true;
        }

        /// <summary>
        /// 设置渲染列表的尺寸
        /// 不需要public
        /// </summary>
        /// <param name="count"></param>
        void _SetListRenderSize(int count)
        {
            if (isVertical)
            {
                //            float blockSize = GetBlockSizeY();
                float contentSize = Mathf.CeilToInt(count * 1.0f / RowColumnCount) * GetBlockSizeY();
                mRectTransformContainer.sizeDelta = new Vector2(mRectTransformContainer.sizeDelta.x, contentSize);
                mRectMask = new Rect(0, -mMaskSize.y, mMaskSize.x, mMaskSize.y);
                mScrollRect.vertical = mRectTransformContainer.sizeDelta.y > mMaskSize.y;
                //            float contentPos = mRectTransformContainer.anchoredPosition.y;
                if (contentSize < mMaskSize.y) //整体需显示区域小于遮罩，则归零
                {
                    mRectTransformContainer.anchoredPosition = Vector2.zero;
                }

            }
            else
            {
                //            float blockSize = GetBlockSizeX();
                float contentSize = Mathf.CeilToInt(count * 1.0f / RowColumnCount) * GetBlockSizeX();
                mRectTransformContainer.sizeDelta = new Vector2(contentSize, mRectTransformContainer.sizeDelta.y);
                mRectMask = new Rect(0, 0, mMaskSize.x, mMaskSize.y);
                mScrollRect.horizontal = mRectTransformContainer.sizeDelta.x > mMaskSize.x;
                //            float contentPos = mRectTransformContainer.anchoredPosition.x;
                if (contentSize < mMaskSize.x) //整体需显示区域小于遮罩，则归零
                {
                    mRectTransformContainer.anchoredPosition = Vector2.zero;
                }
            }
        }

        /// <summary>
        /// 更新各个渲染格子的位置
        /// </summary>
        /// <param name="child"></param>
        /// <param name="index"></param>
        void _UpdateChildTransformPos(GameObject child, int index)
        {
            int row = isVertical ? index / RowColumnCount : index % RowColumnCount;//行数
            int column = isVertical ? index % RowColumnCount : index / RowColumnCount;//列数
            Vector2 v2Pos = new Vector2();
            v2Pos.x = column * GetBlockSizeX();
            v2Pos.y = -row * GetBlockSizeY();
            ((RectTransform)child.transform).anchoredPosition3D = Vector3.zero;
            ((RectTransform)child.transform).anchoredPosition = v2Pos;
        }

        /// <summary>
        /// 获得格子块尺寸
        /// </summary>
        /// <returns></returns>
        protected float GetBlockSizeY() { return CellSize.y + SpacingSize.y; }
        protected float GetBlockSizeX() { return CellSize.x + SpacingSize.x; }

        /// <summary>
        /// 更新动态渲染格
        /// </summary>
        void _UpdateDynmicRects(int count)
        {
            if (_dynRectArr == null)
            {
                _dynRectArr = new DynamicRect[count];
            }
            else
            {
                for (int i = 0; i < _dynRectArr.Length; i++)
                {
                    DynamicRect dynamicRect = _dynRectArr[i];
                    if (dynamicRect != null)
                    {
                        _dynRectPool.Release(dynamicRect);
                    }
                }
                Array.Clear(_dynRectArr, 0, _dynRectArr.Length);
                if (_dynRectArr.Length != count)
                {
                    Array.Resize(ref _dynRectArr, count);
                }
            }
            for (int i = 0; i < count; ++i)
            {
                int row = isVertical ? i / RowColumnCount : i % RowColumnCount;//行数
                int column = isVertical ? i % RowColumnCount : i / RowColumnCount;//列数
                float x = column * GetBlockSizeX() + (isVertical ? 0 : CellSize.x);
                float y = -row * GetBlockSizeY() - (isVertical ? CellSize.y : 0);
                DynamicRect dRect = _dynRectPool.Get();
                dRect.UpdateRect(x, y, CellSize.x, CellSize.y, i);
                _dynRectArr[i] = dRect;
            }
        }

        /// <summary>
        /// 设置格子总数
        /// </summary>
        /// <param name="datas"></param>
        public void SetData(int count)
        {
            _UpdateDynmicRects(count);
            _SetListRenderSize(count);
            mDataCount = count;
            ClearAllListRenderDr();
        }

        /// <summary>
        /// 清理可复用渲染格
        /// </summary>
        void ClearAllListRenderDr()
        {
            if (mList_items != null)
            {
                int len = mList_items.Count;
                for (int i = 0; i < len; ++i)
                {
                    DynamicScrollingItem item = mList_items[i];
                    item.DRect = null;
                }
            }
        }
        /// <summary>
        /// 数据总数
        /// </summary>
        public int GetDataCount() { return mDataCount; }

        /// <summary>
        /// 数据发生变化 供外部调用刷新列表
        /// </summary>
        public void RefreshData()
        {
            if (mDataCount == 0)
                throw new Exception("dataProviders 为空！请先使用SetDataProvider ");
            _UpdateDynmicRects(mDataCount);
            _SetListRenderSize(mDataCount);
            ClearAllListRenderDr();
            //UpdateRender();
        }

        #region 移动至数据
        /// <summary>
        /// 移动列表使之能定位到指定下标上
        /// </summary>
        public virtual void LocateRenderItemAtIndex(int index, float delay)
        {
            if (index < 0 || index > mDataCount - 1)
                throw new Exception("Locate Index Error " + index);
            index = Math.Min(index, mDataCount - mRendererCount + 2);
            index = Math.Max(0, index);
            Vector2 pos = mRectTransformContainer.anchoredPosition;
            Vector2 v2Pos;
            if (isVertical)
            {
                int row = index / RowColumnCount;//行数
                v2Pos = new Vector2(pos.x, row * GetBlockSizeY());
            }
            else
            {
                int column = isVertical ? index % RowColumnCount : index / RowColumnCount;//列数
                v2Pos = new Vector2(-column * GetBlockSizeX(), pos.y);
            }
            m_Coroutine = StartCoroutine(TweenMoveToPos(pos, v2Pos, delay));
        }
        protected IEnumerator TweenMoveToPos(Vector2 pos, Vector2 v2Pos, float delay)
        {
            bool running = true;
            float passedTime = 0f;
            while (running)
            {
                yield return new WaitForEndOfFrame();
                passedTime += Time.deltaTime;
                Vector2 vCur;
                if (passedTime >= delay)
                {
                    vCur = v2Pos;
                    running = false;
                    if (m_Coroutine != null)
                    {
                        StopCoroutine(m_Coroutine);
                        m_Coroutine = null;
                    }
                }
                else
                {
                    vCur = Vector2.Lerp(pos, v2Pos, passedTime / delay);
                }
                mRectTransformContainer.anchoredPosition = vCur;
            }

        }
        protected Coroutine m_Coroutine = null;
        #endregion

        private static Dictionary<int, DynamicRect> _inOverlaps = new Dictionary<int, DynamicRect>();

        protected void UpdateRender()
        {
            if (isVertical)
                mRectMask.y = -mMaskSize.y - mRectTransformContainer.anchoredPosition.y;
            else
                mRectMask.x = GetBlockSizeX() - mRectTransformContainer.anchoredPosition.x;
            _inOverlaps.Clear();
            for (int i = 0; i < _dynRectArr.Length; i++)
            {
                DynamicRect dynamicRect = _dynRectArr[i];
                if (dynamicRect != null)
                {
                    if (dynamicRect.Overlaps(mRectMask))
                    {
                        _inOverlaps.Add(dynamicRect.Index, dynamicRect);
                    }
                }
            }
            int len = mList_items.Count;
            for (int i = 0; i < len; ++i)
            {
                DynamicScrollingItem item = mList_items[i];
                if (item.DRect != null && !_inOverlaps.ContainsKey(item.DRect.Index))
                    item.DRect = null;
            }
            foreach (DynamicRect dR in _inOverlaps.Values)
            {
                if (GetDynmicItem(dR) == null)
                {
                    DynamicScrollingItem item = GetNullDynmicItem();
                    item.DRect = dR;
                    _UpdateChildTransformPos(item.gameObject, dR.Index);

                    if (mDataCount != 0 && dR.Index < mDataCount)
                    {
                        item.SetData();
                    }
                }
            }
            for (int i = 0; i < len; ++i)
            {
                DynamicScrollingItem item = mList_items[i];
                item.UpdateActive();
            }
        }

        /// <summary>
        /// 获得待渲染的渲染器
        /// </summary>
        /// <returns></returns>
        DynamicScrollingItem GetNullDynmicItem()
        {
            int len = mList_items.Count;
            for (int i = 0; i < len; ++i)
            {
                DynamicScrollingItem item = mList_items[i];
                if (item.DRect == null)
                    return item;
            }
            throw new Exception("Error");
        }

        /// <summary>
        /// 通过动态格子获得动态渲染器
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        DynamicScrollingItem GetDynmicItem(DynamicRect rect)
        {
            int len = mList_items.Count;
            for (int i = 0; i < len; ++i)
            {
                DynamicScrollingItem item = mList_items[i];
                if (item.DRect == null)
                    continue;
                if (rect.Index == item.DRect.Index)
                    return item;
            }
            return null;
        }

        public void DestroyAllItem()
        {
            int len = mList_items.Count;
            for (int i = 0; i < len; ++i)
            {
                DynamicScrollingItem item = mList_items[i];
                Destroy(item.gameObject);
            }
        }

        /// <summary>
        /// 通过动态格子获得动态渲染器
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public DynamicScrollingItem GetDynmicItemByIndex(int index)
        {
            DynamicScrollingItem item = null;
            int len = mList_items.Count;
            for (int i = 0; i < len; ++i)
            {
                if (mList_items[i].DRect == null)
                {
                    continue;
                }
                if (mList_items[i].DRect.Index == index)
                {
                    item = mList_items[i];
                    return item;
                }
            }
            return item;
        }

        void Update()
        {
            if (mHasInited)
            {
                UpdateRender();
            }
        }
    }
}
