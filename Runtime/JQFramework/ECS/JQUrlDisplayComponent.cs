using System;
using JQCore.ECS;
using JQCore.tPool.Loader;
using JQCore.tRes;
using JQFramework.tMgr;
using UnityEngine;

namespace JQCore.ECS
{
    public abstract class JQUrlDisplayComponent : JQComponent
    {
        private string _resUrl;
        private Transform _parent;
        private PrefabLoader _prefabLoader;
        
        protected GameObject _gameObject;
        protected Transform _transform;
        protected Vector3 _pos;
        protected BindObjLib _bindObjLib;

        public Transform Transform => _transform;
        public GameObject GameObject => _gameObject;

        protected JQUrlDisplayComponent(string name, int executePriority, bool needEvent) : base(name, executePriority, needEvent)
        {
        }

        /// <summary>
        /// url加载 异步
        /// </summary>
        /// <param name="resUrl"></param>
        /// <param name="onLoadAction"></param>
        protected void LoadRes(string resUrl, Transform parent, Vector3 pos)
        {
            _resUrl = resUrl;
            _parent = parent;
            _pos = pos;
            UrlPrefabLoaderUtil.LoadAsset(resUrl, onLoadCallback, false);
        }
        
        private void onLoadCallback(PrefabLoader prefabLoader)
        {
            _prefabLoader = prefabLoader;
            UrlPrefabLoaderUtil.CancelLoadAsset(_resUrl, onLoadCallback);
            _gameObject = prefabLoader.Spawn();
            _transform = _gameObject.transform;

            _transform.SetParent(_parent);
            // JQLog.LogError("xxx:"+cabinetEntityVo.pos);
            _transform.localPosition = _pos;
            _transform.localRotation = Quaternion.Euler(0, 0, 0);
            
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

        public override void Dispose()
        {
            OnUnBindAttr();
            if (_gameObject != null)
            {
                _prefabLoader.DeSpawn(_gameObject, true);
                _transform = null;
                _gameObject = null;
            }
            base.Dispose();
        }
    }
}