using System;
using JQCore;
using JQCore.tLog;
using JQFramework.tUGUI;
using JQFramework.tUGUI.tDynamicScrollingList;
using JQCore.tUtil;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JQFramework.tUtil
{
    public static class UITools
    {
        /// <summary>
        /// 添加按钮长按功能
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="CallBack"></param>
        public static BtnOnLongPressedRepeat addBtnOnPressed(this Button btn, Action CallBack, float delay = 0.1f, float Spacing = 0.1f)
        {
            BtnOnLongPressedRepeat _btnOnPressed = btn.gameObject.AddMissingComponent<BtnOnLongPressedRepeat>();
            _btnOnPressed.callBack = CallBack;
            _btnOnPressed.delay = delay;
            _btnOnPressed.spacing = Spacing;
            return _btnOnPressed;
        }

        /// <summary>
        /// 长按开始结束事件
        /// </summary>
        /// <param name="btn">按钮</param>
        /// <param name="StartCallBack">开始点击事件</param>
        /// <param name="EndCallBack">结束点击事件</param>
        public static ButtonOnPressedCallBack ButtonOnPressedCallBack(this Button btn, Action<PointerEventData> StartCallBack, Action<PointerEventData> EndCallBack)
        {
            ButtonOnPressedCallBack _btnOnPressed = btn.gameObject.AddMissingComponent<ButtonOnPressedCallBack>();
            _btnOnPressed.StartCallBack = StartCallBack;
            _btnOnPressed.EndCallBack = EndCallBack;
            return _btnOnPressed;
        }


        public static void UnButtonOnPressedCallBack(this Button background)
        {
            ButtonOnPressedCallBack t = background.GetComponent<ButtonOnPressedCallBack>();
            if (t)
            {
                t.StartCallBack = null;
                t.EndCallBack = null;
                background.gameObject.RemoveComponent<ButtonOnPressedCallBack>();
            }
        }

        /// <summary>
        /// 添加滑动ScrollRect回弹功能
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="CallBack"></param>
        public static ScrollRectOnDrag AddScrollRectOnDrag(this ScrollRect _sc, uint part, float speed = 1)
        {
            ScrollRectOnDrag _scrollRectOnDrag = _sc.gameObject.AddMissingComponent<ScrollRectOnDrag>();
            _scrollRectOnDrag._sc = _sc;
            _scrollRectOnDrag.Part = part - 1;
            _scrollRectOnDrag.speed = speed;
            return _scrollRectOnDrag;
        }


        public static JQBanner AddBanner(this ScrollRect sr, GameObject itemPrefab, RectTransform itemParentTrans,
            GameObject togPrefab, RectTransform togParentTrans, int itemCount, Sys.VoidDelegateIntGameobject itemCreateActio)
        {
            JQBanner jqBanner = sr.gameObject.AddMissingComponent<JQBanner>();
            jqBanner.init(sr, itemPrefab, itemParentTrans, togPrefab, togParentTrans, itemCount, itemCreateActio);
            return jqBanner;
        }

        /// <summary>
        /// 添加滑动ScrollRect指定位置事件回调
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="CallBack"></param>
        public static ScrollRectOnDragCallBack AddScrollRectOnDragCallBack(this ScrollRect _sc, float targetPos, Action callBack)
        {
            ScrollRectOnDragCallBack _scrollRectOnDragCallBack = _sc.gameObject.AddMissingComponent<ScrollRectOnDragCallBack>();
            _scrollRectOnDragCallBack._sc = _sc;
            _scrollRectOnDragCallBack.targetPos = targetPos;
            _scrollRectOnDragCallBack.callBack = callBack;
            return _scrollRectOnDragCallBack;
        }

        /// <summary>
        /// 添加滑动ScrollRect实时回调
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="CallBack"></param>
        public static ScrollRectFullDragCallBack AddScrollRectFullDragCallBack(this ScrollRect _sc, Action onBeginDragCallBack, Action onDragCallBack, Action onEndDragCallBack)
        {
            ScrollRectFullDragCallBack _scrollRectFullDragCallBack = _sc.gameObject.AddMissingComponent<ScrollRectFullDragCallBack>();
            _scrollRectFullDragCallBack._sc = _sc;
            _scrollRectFullDragCallBack.onBeginDragCallBack = onBeginDragCallBack;
            _scrollRectFullDragCallBack.onDragCallBack = onDragCallBack;
            _scrollRectFullDragCallBack.onEndDragCallBack = onEndDragCallBack;
            return _scrollRectFullDragCallBack;
        }

        /// <summary>
        /// 添加滑动Image 事件回调
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="CallBack"></param>
        public static ImageOnDrag AddImageOnDragCallBack(this GameObject Target, int Index, int part, Sys.VoidDelegateOO callBack, int angle, bool ever = false, bool canDrag = true)
        {
            ImageOnDrag _imageOnDragCallBack = Target.AddMissingComponent<ImageOnDrag>();
            _imageOnDragCallBack.Target = Target;
            _imageOnDragCallBack.angleType = angle;
            _imageOnDragCallBack.curIndex = Index;
            _imageOnDragCallBack.canDrag = canDrag;
            _imageOnDragCallBack.everReturn = ever;
            _imageOnDragCallBack.part = part;
            _imageOnDragCallBack.action = callBack;
            _imageOnDragCallBack.OnEndDrag(null);
            return _imageOnDragCallBack;
        }

        /// <summary>
        /// 更新滚动列表的数据总数
        /// </summary>
        /// <param name="DSLR"></param>
        /// <param name="DataCount"></param>
        public static void SetDSLRDataCount(this DynamicScrollingListRenderer DSLR, int DataCount)
        {
            if (DataCount != 0)
            {
                DSLR.gameObject.SetActive_EX(true);
                DSLR.SetData(DataCount);
                DSLR.RefreshData();
            }
            else
            {
                DSLR.gameObject.SetActive_EX(false);
                JQLog.LogWarning("暂无设置数据");
            }
        }


        //文本逐字显示
        public static TextShowInTurn addTextShowInTurn(this Text text, string words)
        {
            TextShowInTurn t = text.gameObject.AddMissingComponent<TextShowInTurn>();
            t.myText = text;
            t.words = words;
            t.isActive = true;
            return t;
        }
    }
}