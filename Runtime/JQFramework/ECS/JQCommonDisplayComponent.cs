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

        public Transform Transform => _transform;
        public GameObject GameObject => _gameObject;

        protected JQCommonDisplayComponent(string name, int executePriority, bool needEvent) : base(name, executePriority, needEvent)
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
            _resName = resName;
            _gameObject = CommonResMgr.Instance.Spawn(_resName);
            _transform = _gameObject.transform;

            _transform.SetParent(parent);
            // JQLog.LogError("xxx:"+cabinetEntityVo.pos);
            _transform.localPosition = pos;
            _transform.localRotation = Quaternion.Euler(0, 0, 0);
            _pos = pos;
            
            BindObjLib bindObjLib = _gameObject.GetComponent<BindObjLib>();
            OnBindAttr(bindObjLib);
            UpdateName();
        }

        protected abstract void OnBindAttr(BindObjLib bindObjLib);
        
        public abstract void UpdateName();

        protected virtual void OnUnBindAttr()
        { 
            if (_gameObject != null)
            {
                CommonResMgr.Instance.DeSpawn(_resName, _gameObject);
                _transform = null;
                _gameObject = null;
            }
        }

        public override void Dispose()
        {
            OnUnBindAttr();
            base.Dispose();
        }
    }
}