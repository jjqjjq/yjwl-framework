using System;
using System.Collections.Generic;
using JQCore.tLog;
#if UNITY_EDITOR
using JQCore.tFileSystem;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace JQCore.tRes
{
    public class BindObjLib : MonoBehaviour
    {
        [SerializeField] public BindObj[] objs;

        //param说明
        //Button：点击频率，点击音效
        [Serializable]
        public class BindObj
        {
            public string key;
            public Object obj;
            public string param1; //Button:点击频率
            public string param2; //Button:点击音效
            public string param3;
            public string param4;
        }

        private Dictionary<string, Object> _objectDic;

        public Object GetObjByKey(string key, bool isLog = true)
        {
            if (_objectDic == null)
            {
                InitDic();
                // JQLog.LogError("ObjectDic为空，请先调用InitDic()方法");
            }

            _objectDic.TryGetValue(key, out Object obj);
            if (obj == null && isLog)
            {
                JQLog.LogError("BindObjLib not exist key:" + key);
            }

            return obj;
        }

        public T GetObjByKey<T>(string key, bool isLog = true) where T : class
        {
            return GetObjByKey(key, isLog) as T;
        }

        public void InitDic()
        {
            if (_objectDic != null) return;
            _objectDic = new Dictionary<string, Object>();
            for (int i = 0; i < objs.Length; i++)
            {
                BindObj bindObj = objs[i];
                bool success = _objectDic.TryAdd(bindObj.key, bindObj.obj);
                if (!success)
                {
                    JQLog.LogError($"存在重复的key goName:{gameObject.name} key:{bindObj.key}");
                }
            }
        }

#if UNITY_EDITOR

        public bool IsKeyExist(Object obj, string key)
        {
            if (objs == null)
            {
                objs = new BindObj[0];
            }

            for (int i = 0; i < objs.Length; i++)
            {
                BindObj bindObj = objs[i];
                if (obj == bindObj.obj && key == bindObj.key)
                {
                    return true;
                }
            }

            return false;
        }

        private BindObj getObjByComponent(Object obj)
        {
            if (objs == null)
            {
                objs = new BindObj[0];
            }

            for (int i = 0; i < objs.Length; i++)
            {
                BindObj bindObj = objs[i];
                if (obj == bindObj.obj)
                {
                    return bindObj;
                }
            }

            return null;
        }


        private string lowerFirstWord(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }

        private string CreateOneKey(Object obj, Transform transform, bool addTypeName)
        {
            string key = transform.name;

            if (addTypeName)
            {
                if (obj is GameObject)
                {
                    key = transform.name + "Go";
                }
                else if (obj is Transform)
                {
                    key = transform.name + "Trans";
                }
                else if (obj is Button)
                {
                    key = transform.name + "Btn";
                }
                else if (obj is Image)
                {
                    key = transform.name + "Img";
                }
                else if (obj is Text)
                {
                    key = transform.name + "Text";
                }
                else
                {
                    key = transform.name + obj.GetType().Name;
                }
            }

            if (!IsKeyExist(obj, key))
            {
                return key;
            }

            for (int i = 0; i < 9; i++)
            {
                key = $"{transform.name}_{i}";
                if (!IsKeyExist(obj, key))
                {
                    return key;
                }
            }

            if (transform.parent)
            {
                key = transform.parent.name + key;
            }

            if (!IsKeyExist(obj, key))
            {
                return key;
            }

            if (transform.parent.parent)
            {
                key = transform.parent.parent.name + key;
            }

            if (!IsKeyExist(obj, key))
            {
                return key;
            }

            if (transform.parent.parent.parent)
            {
                key = transform.parent.parent.parent.name + key;
            }

            if (!IsKeyExist(obj, key))
            {
                return key;
            }

            Debug.LogError("添加了重复的key：" + key);
            return key;
        }

        private void ClearNullObj()
        {
            if (objs == null)
            {
                objs = new BindObj[0];
            }

            List<BindObj> list = new List<BindObj>(objs);
            for (int i = objs.Length - 1; i >= 0; i--)
            {
                BindObj oneBindObj = objs[i];
                if (oneBindObj.obj == null)
                {
                    list.RemoveAt(i);
                }
            }

            objs = list.ToArray();
        }

        public void AddObjEditor(Object obj, Transform objTrans, bool addTypeName)
        {
            ClearNullObj();

            string key = CreateOneKey(obj, objTrans, addTypeName);
            key = lowerFirstWord(key);
            BindObj bindObj = getObjByComponent(obj);
            if (bindObj != null)
            {
                bindObj.key = key;
                ApplyPrefab(gameObject);
                return;
            }


            List<BindObj> bindObjs = new List<BindObj>(objs);
            bindObjs.Add(new BindObj() { key = key, obj = obj });
            objs = bindObjs.ToArray();
            ApplyPrefab(gameObject);
        }

        public static void ApplyPrefab(GameObject gameObject)
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage == null)
            {
                GameObject prefabGo = PrefabUtility.GetNearestPrefabInstanceRoot(gameObject);
                if (prefabGo != null)
                {
                    PrefabUtility.ApplyPrefabInstance(prefabGo, InteractionMode.AutomatedAction);
                }
            }
            else
            {
                PrefabUtility.SaveAsPrefabAsset(prefabStage.prefabContentsRoot, prefabStage.assetPath);
            }
        }

        [ContextMenu("同步代码（仅打印）")]
        public void PrintViewScript()
        {
            string viewName = gameObject.name;
            Debug.Log(viewName);
            ClearNullObj();
            MVCUtil.CreateViewExtraCls(this, false, false, true);
        }


        [ContextMenu("同步代码")]
        public void SynViewScript()
        {
            string viewName = gameObject.name;
            Debug.Log(viewName);
            ClearNullObj();
            MVCUtil.CreateViewExtraCls(this, false, false);
        }

        [ContextMenu("新建代码")]
        public void CreateViewScript()
        {
            string viewName = gameObject.name;
            Debug.Log(viewName);
            ClearNullObj();
            MVCUtil.CreateViewExtraCls(this, true, false);
        }

        [ContextMenu("同步代码-SubView")]
        public void SynSubViewScript()
        {
            string viewName = gameObject.name;
            Debug.Log(viewName);
            ClearNullObj();
            MVCUtil.CreateViewExtraCls(this, false, true);
        }

        [ContextMenu("新建代码-SubView")]
        public void CreateSubViewScript()
        {
            string viewName = gameObject.name;
            Debug.Log(viewName);
            ClearNullObj();
            MVCUtil.CreateViewExtraCls(this, true, true);
        }
#endif
    }
}