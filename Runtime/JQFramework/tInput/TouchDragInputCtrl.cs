using System;
using System.Collections.Generic;
using JQCore.ECS;
using JQCore.tLog;
using JQFramework.tMgr;
using UnityEngine;

namespace JQFramework.tInput
{
    /// <summary>
    /// 过时，使用TouchDragInputCtrl
    /// </summary>
    public class TouchDragInputCtrl
    {
        protected int _normalLayerMask; //点瓶子
        private bool isDragging;
        protected Camera _camera;
        protected GameObject _dragTarget;
        protected Vector3 _dragStartPos;
        protected Vector2 _touchStartPos;
        protected Vector3 offset;
        protected Vector3 screenPoint; // 存储物体在屏幕上的位置
        protected Dictionary<GameObject, JQEntity> _colliderGo2EntitieDic = new Dictionary<GameObject, JQEntity>();

        public TouchDragInputCtrl(Camera camera)
        {
            _camera = camera;
            _normalLayerMask = LayerMgr.GetMaskValue(LayerMgr.NormalLayers);
        }

        public void AddColliderGoEntity(GameObject colliderGo, JQEntity cabinetEntity)
        {
            _colliderGo2EntitieDic.Add(colliderGo, cabinetEntity);
        }

        #region 主要方法

        protected JQEntity Raycast(Vector2 pos, int layerMask)
        {
            Ray ray = _camera.ScreenPointToRay(pos);
            RaycastHit hit;
            //点击瓶子广告
            if (Physics.Raycast(ray, out hit, 1000, layerMask))
            {
                GameObject colliderGo = hit.collider.gameObject;
                // JQLog.LogError(colliderGo.name);
                if (_colliderGo2EntitieDic.TryGetValue(colliderGo, out JQEntity clickEntity))
                {
                    return clickEntity;
                }
            }

            return null;
        }

        protected virtual bool IsDragSuccess(Vector2 pos)
        {
            return false;
        }

        protected virtual void OnDragStart(Vector2 pos)
        {
            if (_dragTarget != null)
            {
                _dragStartPos = _dragTarget.transform.position;
                screenPoint = _camera.WorldToScreenPoint(_dragTarget.transform.position);
                offset = _dragTarget.transform.position -
                         _camera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, screenPoint.z)); // 计算偏移量
            }

            _touchStartPos = _camera.ScreenToWorldPoint(pos);
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

        #endregion

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
            _colliderGo2EntitieDic.Clear();
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