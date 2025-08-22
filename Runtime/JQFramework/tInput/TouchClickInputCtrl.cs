using System;
using JQCore.tLog;
using UnityEngine;

namespace JQFramework.tInput
{
    public class TouchClickInputCtrl
    {
        private bool isDragging;
        private Camera _sceneCamera;

        public TouchClickInputCtrl(Camera sceneCamera)
        {
            _sceneCamera = sceneCamera;
        }

        protected virtual bool IsClickSuccess(Vector2 pos)
        {
            return false;
        }

        protected virtual void OnClick(Vector2 pos)
        {
        }

        private bool UpdateMouse()
        {
            // 检测鼠标左键是否按下
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Input.mousePosition;
                bool canDrag = IsClickSuccess(mousePos);
                if (canDrag)
                {
                    OnClick(mousePos);
                }

                return true;
            }

            return false;
        }

        private bool UpdateTouch()
        {
            if (Input.touchCount == 0) return false;
            // 检测是否有触摸输入
            Touch touch = Input.GetTouch(0); // 获取第一个触摸点
            // 只在TouchPhase.Began时处理点击
            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchPos = touch.position;
                bool canDrag = IsClickSuccess(touchPos);
                if (canDrag)
                {
                    OnClick(touchPos);
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