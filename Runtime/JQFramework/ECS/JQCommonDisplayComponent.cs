using System;
using JQCore.ECS;
using JQCore.tPool.Loader;
using JQCore.tRes;
using JQFramework.tMgr;
using UnityEngine;

namespace JQCore.ECS
{
    public abstract class JQCommonDisplayComponent : JQComponent
    {
        private string _resName;
        private Action<UrlPrefabLoader> _onLoadAction;
        protected GameObject _gameObject;
        protected Transform _transform;
        protected Vector3 _pos;
        protected BindObjLib _bindObjLib;

        public Transform Transform => _transform;
        public GameObject GameObject => _gameObject;

        protected JQCommonDisplayComponent(string name, int executePriority, bool needEvent) : base(name, executePriority,
            needEvent)
        {
        }

        /// <summary>
        /// 直接从CommonResMgr拿 同步
        /// </summary>
        /// <param name="resName"></param>
        /// <param name="parent"></param>
        /// <param name="pos"></param>
        protected void LoadRes(string resName, Transform parent, Vector3 pos)
        {
            if (_gameObject != null) return;
            _resName = resName;
            _gameObject = CommonResMgr.Instance.Spawn(_resName);
            _transform = _gameObject.transform;

            _transform.SetParent(parent);
            // JQLog.LogError("xxx:"+cabinetEntityVo.pos);
            _transform.localPosition = pos;
            _transform.localRotation = Quaternion.Euler(0, 0, 0);
            _pos = pos;

            _bindObjLib = _gameObject.GetComponent<BindObjLib>();
            OnBindAttr();
            UpdateName();
            OnLoadFinish();
        }

        protected virtual void OnBindAttr()
        {
        }

        protected virtual void UpdateName()
        {
        }

        protected virtual void OnLoadFinish()
        {
        }

        protected virtual void OnUnBindAttr()
        {
        }

        // public override void onReset()
        // {
        //     OnUnBindAttr();
        //     if (_gameObject != null)
        //     {
        //         CommonResMgr.Instance.DeSpawnPerformance(_resName, _gameObject);
        //         _transform = null;
        //         _gameObject = null;
        //     }
        //     base.onReset();
        // }

        public override void Dispose()
        {
            OnUnBindAttr();
            if (_gameObject != null)
            {
#if UNITY_EDITOR
                CommonResMgr.Instance.DeSpawn(_resName, _gameObject);
#else
                CommonResMgr.Instance.DeSpawnPerformance(_resName, _gameObject);
#endif
                _transform = null;
                _gameObject = null;
            }

            base.Dispose();
        }
    }
}