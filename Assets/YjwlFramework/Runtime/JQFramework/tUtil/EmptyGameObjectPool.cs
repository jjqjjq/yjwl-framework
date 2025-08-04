using System.Collections.Generic;
using JQCore;
using JQCore.tLog;
using JQCore.tCfg;
using JQCore.tUtil;
using UnityEngine;

namespace JQFramework.tUtil
{
    public class EmptyGameObjectPool
    {
        private int _gameObjectIndex = 0;
        private Queue<GameObject> _gameObjectPool = new Queue<GameObject>();

        public delegate GameObject BuildGoFun(int index);

        private BuildGoFun _buildGoFun;
        
        public EmptyGameObjectPool( BuildGoFun buildGoFun)
        {
            _buildGoFun = buildGoFun;
        }

        public GameObject SpawnGameObject(string name)
        {
            GameObject go = null;
            if (_gameObjectPool.Count == 0)
            {
                go = _buildGoFun(_gameObjectIndex);
                _gameObjectIndex++;
            }
            else
            {
                go = _gameObjectPool.Dequeue();
            }
            if (go == null)
            {
                JQLog.LogError("对象被销毁了："+name);
                go = SpawnGameObject(name);
            }

            if (Sys.isEditor && AppConfig.IsDebug)
            {
                go.name = name;
            }

            return go;
        }

        public void DespawnGameObject(GameObject gameObject)
        {
            Transform transform = gameObject.transform;
            transform.SetLocalPos_EX(0, 0, -10000);
            transform.SetLocalScale_EX(1, 1, 1);
            _gameObjectPool.Enqueue(gameObject);
            if (Sys.isEditor && AppConfig.IsDebug)
            {
                gameObject.name = "emptyGo";
            }
        }
    }
}