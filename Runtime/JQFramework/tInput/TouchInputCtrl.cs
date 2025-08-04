using System;
using JQCore.tLog;
using UnityEngine;

namespace JQFramework.tInput
{
    public class TouchInputCtrl
    {
        private bool isDragging;
        protected Camera _camera;
        protected GameObject _dragTarget;
        protected Vector3 _dragStartPos;
        protected Vector2 _touchStartPos;
        protected Vector3 offset;
        protected Vector3 screenPoint; // 存储物体在屏幕上的位置

        public TouchInputCtrl(Camera camera)
        {
            _camera = camera;
        }

        protected virtual bool IsDragSuccess(Vector2 pos)
        {
            return false;
        }

        protected virtual void OnDragStart(Vector2 pos)
        {
            _dragStartPos = _dragTarget.transform.position;
            screenPoint = _camera.WorldToScreenPoint(_dragTarget.transform.position);
            _touchStartPos = _camera.ScreenToWorldPoint(pos);
            offset = _dragTarget.transform.position -
                     _camera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, screenPoint.z)); // 计算偏移量
        }

        protected virtual void OnDraging(Vector2 pos)
        {
            Vector3 curScreenPoint = new Vector3(pos.x, pos.y, screenPoint.z); // 获取当前触摸位置的屏幕坐标
            Vector3 curPosition = _camera.ScreenToWorldPoint(curScreenPoint) + offset; // 将屏幕坐标转换为世界坐标并加上偏移量
            _dragTarget.transform.position = curPosition; // 更新物体的位置
        }

        protected virtual void OnDragEnd(Vector2 pos)
        {
            _dragTarget = null;
        }


        private bool UpdateMouse()
        {
            // 检测鼠标左键是否按下
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Input.mousePosition;
                bool canDrag = IsDragSuccess(mousePos);
                if (canDrag)
                {
                    OnDragStart(mousePos);
                    isDragging = true; // 设置拖拽标志位为真
                }

                return true;
            }

            // 检测鼠标左键是否持续按下并且物体正在被拖拽
            if (Input.GetMouseButton(0) && isDragging)
            {
                Vector3 mousePos = Input.mousePosition;
                OnDraging(mousePos);
                return true;
            }

            // 检测鼠标左键是否释放
            if (isDragging && Input.GetMouseButtonUp(0))
            {
                Vector3 mousePos = Input.mousePosition;
                OnDragEnd(mousePos);
                isDragging = false; // 设置拖拽标志位为假
                return true;
            }

            return false;
        }

        private bool UpdateTouch()
        {
            if (Input.touchCount == 0) return false;
            // 检测是否有触摸输入
            Touch touch = Input.GetTouch(0); // 获取第一个触摸点

            // 检测触摸开始
            if (!isDragging)
            {
                Vector2 touchPos = touch.position;
                bool canDrag = IsDragSuccess(touchPos);
                if (canDrag)
                {
                    OnDragStart(touchPos);
                    isDragging = true; // 设置拖拽标志位为真
                }
            }
            else
            {
                // 检测触摸移动并且物体正在被拖拽
                if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 touchPos = touch.position;
                    OnDraging(touchPos);
                }


                // 检测触摸结束
                if (touch.phase == TouchPhase.Ended)
                {
                    Vector2 touchPos = touch.position;
                    OnDragEnd(touchPos);
                    isDragging = false; // 设置拖拽标志位为假
                }
            }


            return true;
        }

        public virtual void Clear()
        {
            isDragging = false;
        }

        public virtual bool OnUpdate()
        {
#if UNITY_EDITOR
            return UpdateMouse();
#else
            return UpdateTouch();
#endif
        }

        public virtual bool OnFixUpdate()
        {
#if UNITY_EDITOR
            return UpdateMouse();
#else
            return UpdateTouch();
#endif
        }
    }
}