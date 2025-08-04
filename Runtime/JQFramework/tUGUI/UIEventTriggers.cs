using System;
using System.Collections.Generic;
using JQCore;
using JQCore.tUtil;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JQFramework.tUGUI
{
    /// <summary>
    /// UGUI模型旋转鼠标拖动响应
    /// </summary>
    public class UIDragEventTrigger : EventTrigger
    {
        private GameObject m_Display;
        private Vector2 m_StartMousePos;
        private Quaternion m_StartModelQuat;
        private float speed_Num = 2.0f;

        public void SetTarget(GameObject display)
        {
            this.m_Display = display;
        }

        public void SetRotateSpeed(float num)
        {
            this.speed_Num = num;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (m_Display != null)
            {
                m_StartMousePos = eventData.position;
                m_StartModelQuat = m_Display.transform.localRotation;
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (m_Display != null)
            {
                Vector3 dragVec = eventData.position - m_StartMousePos;
                m_Display.transform.localRotation =
                    m_StartModelQuat * Quaternion.AngleAxis(-dragVec.x / speed_Num, Vector3.up);
            }
        }
    }

    /// <summary>
    /// UGUI普通拖动功能相关封装
    /// </summary>
    class UIDragPosEventTrigger : EventTrigger
    {
        public Vector2 m_StartMousePos;
        public Vector2 m_StartAnchoredPos;
        public Vector2 m_EndMousePos;
        public Vector2 m_EndAnchoredPos;
        bool m_isLimit = false;
        Vector2 m_LimitPosMin, m_LimitPosMax;
        RectTransform m_rectTrans;
        bool m_allowVerticalMove, m_allowHorizonalMove;

        Action m_BeginDragCallback = null;
        Action m_DoingDragCallback = null;
        Action m_EndDragCallback = null;

        void Awake()
        {
            m_rectTrans = this.gameObject.GetComponent<RectTransform>();
            GetComponent<Graphic>().raycastTarget = true;
            m_allowVerticalMove = true;
            m_allowHorizonalMove = true;
        }

        public void SetPosLimit(bool isLimit, int xMin, int xMax, int yMin, int yMax)
        {
            m_isLimit = isLimit;
            m_LimitPosMin = new Vector2(xMin, yMin);
            m_LimitPosMax = new Vector2(xMax, yMax);
        }

        public void SetMoveDirection(bool allowVertical, bool allowHorizonal)
        {
            m_allowVerticalMove = allowVertical;
            m_allowHorizonalMove = allowHorizonal;
        }

        public void SetCallBacks(Action cbBegin, Action cbDoing, Action cbEnd)
        {
            m_BeginDragCallback = cbBegin;
            m_DoingDragCallback = cbDoing;
            m_EndDragCallback = cbEnd;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            m_StartMousePos = eventData.position;
            m_StartAnchoredPos = m_rectTrans.anchoredPosition;

            if (m_BeginDragCallback != null)
            {
                m_BeginDragCallback();
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            Vector2 deltaPos = eventData.position - m_StartMousePos;
            Vector2 targetPos = m_StartAnchoredPos + deltaPos;
            if (m_allowVerticalMove)
            {
                targetPos.y = Mathf.Clamp(targetPos.y, m_LimitPosMin.y, m_LimitPosMax.y);
            }
            else
            {
                targetPos.y = m_StartAnchoredPos.y;
            }

            if (m_allowHorizonalMove)
            {
                targetPos.x = Mathf.Clamp(targetPos.x, m_LimitPosMin.x, m_LimitPosMax.x);
            }
            else
            {
                targetPos.x = m_StartAnchoredPos.x;
            }

            m_rectTrans.anchoredPosition = targetPos;

            if (m_DoingDragCallback != null)
            {
                m_DoingDragCallback();
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            m_EndMousePos = eventData.position;
            m_EndAnchoredPos = m_rectTrans.anchoredPosition;
            if (m_EndDragCallback != null)
            {
                m_EndDragCallback();
            }
        }
    }


    /// <summary>
    /// UGUI拖动热区功能相关封装
    /// </summary>
    public class UIHotAreaEventTrigger : EventTrigger
    {
        bool m_IsDragging = false;
        List<Graphic> m_HotAreaList = new List<Graphic>();
        Vector3 m_origPos, m_eventDataPos;
        int m_origSiblingIndex;
        public Action<Graphic> m_HotAreaCallback;
        public Action m_BeginDragCallback = null;
        public Action m_DoingDragCallback = null;
        public Action m_EndDragCallback = null;
        public Graphic myArea;


        public void RegisterHotArea(Graphic hotArea)
        {
            hotArea.raycastTarget = true;
            m_HotAreaList.Add(hotArea);
        }

        public void RemoveHotArea(Graphic hotArea)
        {
            m_HotAreaList.Remove(hotArea);
        }

        public void RemoveAllHotArea()
        {
            m_HotAreaList.Clear();
        }

        public void setOrigPos(Vector3 vec3)
        {
            m_origPos = vec3;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            m_IsDragging = true;

            if (myArea != null)
            {
                myArea.raycastTarget = false;
            }

            m_origPos = this.transform.position;
            m_origSiblingIndex = this.transform.GetSiblingIndex();
            this.transform.SetAsLastSibling();
            RectTransformUtility.ScreenPointToWorldPointInRectangle(this.transform as RectTransform, eventData.position,
                eventData.pressEventCamera, out m_eventDataPos);
            this.transform.position = m_eventDataPos;
            Vector3 tmpVec = this.transform.localPosition;
            this.transform.localPosition = tmpVec;


            if (m_BeginDragCallback != null)
            {
                m_BeginDragCallback();
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(this.transform as RectTransform, eventData.position,
                eventData.pressEventCamera, out m_eventDataPos);
            this.transform.position = m_eventDataPos;
            Vector3 tmpVec = this.transform.localPosition;
            this.transform.localPosition = tmpVec;
            m_DoingDragCallback?.Invoke();
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            m_IsDragging = false;

            if (myArea != null)
            {
                myArea.raycastTarget = true;
            }

            this.transform.position = m_origPos;
            this.transform.SetSiblingIndex(m_origSiblingIndex);
            // 射线判定与回调调用
            GameObject goRayCast = eventData.pointerCurrentRaycast.gameObject;
            if (goRayCast != null)
            {
                for (int i = 0; i < m_HotAreaList.Count; i++)
                {
                    Graphic mGraphic = m_HotAreaList[i];
                    if (goRayCast == mGraphic.gameObject)
                    {
                        m_HotAreaCallback?.Invoke(mGraphic);
                        break;
                    }
                }
            }

            m_EndDragCallback?.Invoke();
        }
    }

    /// <summary>
    /// UGUI拖动热区功能相关封装
    /// </summary>
    class UIDragRayCastEventTrigger : EventTrigger
    {
        bool m_IsDragging = false;
        Action m_DragStarCallback;
        Action<object> m_HotAreaCallback;
        Action<object> m_DragEndCallback;
        List<Graphic> m_HotAreaList = new List<Graphic>();
        Vector3 m_origPos, m_eventDataPos;
        Graphic myArea;
        int m_origSiblingIndex;

        public void SetCallback(Action cb, Action<object> cb1, Action<object> cb2)
        {
            m_DragStarCallback = cb;
            m_HotAreaCallback = cb1;
            m_DragEndCallback = cb2;
        }

        public void RegisterHotArea(Graphic hotArea)
        {
            hotArea.raycastTarget = true;
            m_HotAreaList.Add(hotArea);
        }

        public void RemoveHotArea(Graphic hotArea)
        {
            m_HotAreaList.Remove(hotArea);
        }

        public void RemoveAllHotArea()
        {
            m_HotAreaList.Clear();
        }

        public void setOrigPos(Vector3 vec3)
        {
            myArea = this.gameObject.GetComponent<Graphic>();
            m_origPos = vec3;
        }

        public void resetPos()
        {
            this.transform.localPosition = m_origPos;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            m_IsDragging = true;
            myArea.raycastTarget = false;
            m_origSiblingIndex = this.transform.GetSiblingIndex();
            this.transform.SetAsLastSibling();
            RectTransformUtility.ScreenPointToWorldPointInRectangle(this.transform as RectTransform, eventData.position,
                eventData.pressEventCamera, out m_eventDataPos);
            this.transform.position = m_eventDataPos;
            Vector3 tmpVec = this.transform.localPosition;
            this.transform.localPosition = tmpVec;
            m_DragStarCallback();
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (m_IsDragging)
            {
                RectTransformUtility.ScreenPointToWorldPointInRectangle(this.transform as RectTransform,
                    eventData.position, eventData.pressEventCamera, out m_eventDataPos);
                this.transform.position = m_eventDataPos;
                Vector3 tmpVec = this.transform.localPosition;
                this.transform.localPosition = tmpVec;
                this.transform.SetSiblingIndex(m_origSiblingIndex);
                // 射线判定与回调调用
                GameObject goRayCast = eventData.pointerCurrentRaycast.gameObject;
                if (goRayCast != null)
                {
                    for (int i = 0; i < m_HotAreaList.Count; i++)
                    {
                        if (goRayCast == m_HotAreaList[i].gameObject)
                        {
                            if (m_HotAreaCallback != null)
                            {
                                m_IsDragging = false;
                                m_HotAreaCallback(i + 1);
                            }

                            break;
                        }
                    }
                }
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (m_DragEndCallback != null)
            {
                m_DragEndCallback(m_IsDragging);
            }

            m_IsDragging = false;
            myArea.raycastTarget = true;
        }
    }

    /// <summary>
    /// 按钮长按接口，可重复多次触发（快速升级之类的功能）
    /// </summary>
    public class BtnOnLongPressedRepeat : EventTrigger
    {
        /// <summary> 触发间距 </summary>
        public float spacing = 0.1f;

        /// <summary> 延时执行 </summary>
        public float delay = 0.1f;

        public bool isDown = false;
        private float lastIsDownTime1;
        private float lastIsDownTime2;
        public Action callBack;

        void Update()
        {
            if (!isDown) return;
            if (Time.time - lastIsDownTime1 > delay)
            {
                if (Time.time - lastIsDownTime2 > spacing)
                {
                    lastIsDownTime2 = Time.time;
                    if (callBack != null && isDown)
                    {
                        callBack();
                    }
                }
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            isDown = true;
            lastIsDownTime1 = Time.time;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (callBack != null && isDown)
                callBack();
            isDown = false;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            isDown = false;
        }
    }

    /// <summary>
    /// 按钮长按接口，仅触发一次（长按出TIPS）
    /// </summary>
    public class BtnOnLongPressed : EventTrigger
    {
        private bool _hadTrigger = false;

        /// <summary> 延时执行 </summary>
        public float delay = 0.1f;

        public bool isDown = false;
        private float lastIsDownTime1;
        private float lastIsDownTime2;
        public Action onTriggerAction;
        public Action onPointerUpAction;

        void Update()
        {
            if (!isDown) return;
            if (_hadTrigger) return;

            if (Time.time - lastIsDownTime1 > delay)
            {
                onTriggerAction?.Invoke();
                _hadTrigger = true;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            isDown = true;
            _hadTrigger = false;
            lastIsDownTime1 = Time.time;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            isDown = false;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            isDown = false;
            onPointerUpAction?.Invoke();
        }
    }

    /// <summary>
    /// 卡牌抽奖奖励显示表现
    /// </summary>
    /// 
    public class ImageOnDrag : EventTrigger
    {
        public GameObject Target; //旋转目标
        public Sys.VoidDelegateOO action; //切换目标回调
        public int part = 0; //分多少份
        public int curIndex = 4; //当前显示的下标
        public int angleType = 0;
        public bool canDrag = true; //是否可拖动
        public bool everReturn = false;
        private Vector2 m_StartMousePos; //鼠标开始点击位置
        private Quaternion m_StartModelQuat;
        private float targetAngle; //最终角度
        public bool autoMove = false;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (!canDrag)
            {
                return;
            }

            m_StartMousePos = eventData.position;
            m_StartModelQuat = Target.transform.localRotation;
            autoMove = false;
        }

        public void MoveTo(int index)
        {
            curIndex = index;
            targetAngle = (int)(360 / part * (curIndex - 1));
            autoMove = true;
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (!canDrag)
            {
                return;
            }

            Vector3 dragVec = eventData.position - m_StartMousePos;
            if (angleType == 1)
            {
                Target.transform.localRotation =
                    m_StartModelQuat * Quaternion.AngleAxis(-dragVec.x / 10.0f, Vector3.forward);
            }
            else if (angleType == 2)
            {
                Target.transform.localRotation =
                    m_StartModelQuat * Quaternion.AngleAxis(-dragVec.x / 10.0f, Vector3.up);
            }
            else
            {
                Target.transform.localRotation =
                    m_StartModelQuat * Quaternion.AngleAxis(-dragVec.x / 10.0f, Vector3.right);
            }

            int index = getIndex(true);
            if (curIndex != index || everReturn)
            {
                float curAngle = 0;
                if (angleType == 1)
                {
                    curAngle = Target.transform.eulerAngles.z;
                }
                else if (angleType == 2)
                {
                    curAngle = Target.transform.eulerAngles.y;
                }
                else
                {
                    curAngle = Target.transform.eulerAngles.x;
                }

                curIndex = index;
                action(curIndex, curAngle);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (!canDrag)
            {
                return;
            }

            curIndex = getIndex(true);
            autoMove = true;
        }

        private int getIndex(bool resetTarget)
        {
            if (resetTarget)
            {
                targetAngle = 0;
            }

            int index = 1;
            float curAngle = 0;
            if (angleType == 1)
            {
                curAngle = Target.transform.eulerAngles.z;
            }
            else if (angleType == 2)
            {
                curAngle = Target.transform.eulerAngles.y;
            }
            else
            {
                curAngle = Target.transform.eulerAngles.x;
            }

            for (float i = 1; i <= part; i++)
            {
                float angle = 360 / part;
                if (angle / -2 + angle * (i - 1) <= curAngle && curAngle <= angle / 2 + angle * (i - 1))
                {
                    index = (int)i;
                    if (resetTarget)
                    {
                        targetAngle = (int)(angle * (i - 1));
                    }
                }
            }

            return index;
        }

        // Update is called once per frame
        void Update()
        {
            if (autoMove)
            {
                if (angleType == 1)
                {
                    Target.transform.SetLocalRotation_EX(0, 0,
                        Mathf.LerpAngle(Target.transform.eulerAngles.z, targetAngle, 10 * Time.deltaTime));
                }
                else if (angleType == 2)
                {
                    float rotate = Mathf.LerpAngle(Target.transform.eulerAngles.y, targetAngle, 10 * Time.deltaTime);
                    Target.transform.SetLocalRotation_EX(0, rotate, 0);
                }
                else
                {
                    Target.transform.SetLocalRotation_EX(
                        Mathf.LerpAngle(Target.transform.eulerAngles.x, targetAngle, 10 * Time.deltaTime), 0, 0);
                }

                int index = getIndex(false);
                if (curIndex != index || everReturn)
                {
                    float curAngle = 0;
                    if (angleType == 1)
                    {
                        curAngle = Target.transform.eulerAngles.z;
                    }
                    else if (angleType == 2)
                    {
                        curAngle = Target.transform.eulerAngles.y;
                    }
                    else
                    {
                        curAngle = Target.transform.eulerAngles.x;
                    }

                    curIndex = index;
                    action(curIndex, curAngle);
                }
            }
        }

        void LateUpdate()
        {
            float angle = 0;
            if (angleType == 1)
            {
                angle = Target.transform.eulerAngles.z;
            }
            else if (angleType == 2)
            {
                angle = Target.transform.eulerAngles.y;
            }
            else
            {
                angle = Target.transform.eulerAngles.x;
            }

            if (autoMove && Mathf.Abs(angle - targetAngle) < 1)
            {
                if (action != null)
                {
                    if (angleType == 1)
                    {
                        Target.transform.SetLocalRotation_EX(0, 0, targetAngle);
                    }
                    else if (angleType == 2)
                    {
                        Target.transform.SetLocalRotation_EX(0, targetAngle, 0);
                    }
                    else
                    {
                        Target.transform.SetLocalRotation_EX(targetAngle, 0, 0);
                    }

                    action(curIndex, angle);
                    autoMove = false;
                }
            }
        }
    }

    /// <summary>
    /// ScrollRect拖拽 分区域会弹表现
    /// </summary>
    public class ScrollRectOnDrag : EventTrigger
    {
        public ScrollRect _sc;
        public float Part;
        public float speed = 1;
        public int curIndex = 0;
        private bool autoMove = false; //开始自动弹回
        private float targetPos = 0;
        public bool draging = false;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            draging = true;
            autoMove = false;
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            draging = false;
            float pos = _sc.horizontalNormalizedPosition;
            curIndex = 0;
            for (int i = 0; i <= Part; i++)
            {
                if ((i - 0.5f) / Part <= pos && (i + 0.5f) / Part > pos)
                {
                    curIndex = i;
                    break;
                }
            }

            targetPos = curIndex / Part;
            autoMove = true;
        }

        public void MoveToThatPart(int partIndex)
        {
            targetPos = partIndex / Part;
            autoMove = true;
        }

        public int onRightMove()
        {
            if (curIndex < Part)
            {
                curIndex = curIndex + 1;
                targetPos = curIndex / Part;
                autoMove = true;
            }

            return curIndex;
        }

        public int onLeftMove()
        {
            if (curIndex > 0)
            {
                curIndex = curIndex - 1;
                targetPos = curIndex / Part;
                autoMove = true;
            }

            return curIndex;
        }


        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (autoMove)
            {
                _sc.horizontalNormalizedPosition =
                    Mathf.Lerp(_sc.horizontalNormalizedPosition, targetPos, speed * Time.deltaTime);
            }
        }

        void LateUpdate()
        {
            if (Mathf.Abs(_sc.horizontalNormalizedPosition - targetPos) < 0.001)
                autoMove = false;
        }
    }

    /// <summary>
    /// ScrollRect 拖动指定位置后执行回调
    /// </summary>
    public class ScrollRectOnDragCallBack : EventTrigger
    {
        public ScrollRect _sc;
        public float targetPos = 0;
        private bool draging = false; //是否在拖动
        public Action callBack = null; //拖动目标位置执行回调

        public override void OnBeginDrag(PointerEventData eventData)
        {
            draging = true;
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            draging = false;
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (draging)
            {
                if (_sc.verticalNormalizedPosition <= targetPos)
                {
                    if (callBack != null)
                    {
                        callBack();
                        draging = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// ScrollRect 拖动实时回调, 三Drag事件转Lua层处理
    /// </summary>
    public class ScrollRectFullDragCallBack : EventTrigger
    {
        public ScrollRect _sc;
        public Action onBeginDragCallBack = null;
        public Action onDragCallBack = null;
        public Action onEndDragCallBack = null;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (onBeginDragCallBack != null)
            {
                onBeginDragCallBack();
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (onDragCallBack != null)
            {
                onDragCallBack();
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (onEndDragCallBack != null)
            {
                onEndDragCallBack();
            }
        }
    }
}