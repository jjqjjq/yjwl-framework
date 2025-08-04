/*----------------------------------------------------------------
// 文件名：CheckCommonInfo.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/10/10 12:49:01
//----------------------------------------------------------------*/

using System;
using JQCore.tLog;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JQEditor.Build
{
    public class CheckCommonInfo<T> where T : Object
    {
        public delegate bool CheckCommonInfoAction(string path, T obj, object obj1);

        private readonly CheckCommonInfoAction _action;
        private string _curName;

        private readonly string[] _dataArray;
        private readonly string _extension;
        private int _index;
        private readonly string _name;
        private readonly object _obj1;
        private readonly bool _showUI;
        private readonly int _total;

        public CheckCommonInfo(string name, string searchPath, CheckCommonInfoAction action, object obj1, string extension = ".prefab", string filter = "t:gameObject", bool showUI = true)
        {
            _name = name;
            _showUI = showUI;
            //获取路径下所有的gameObject GUID，存入数组
            string[] lookFor = { searchPath };
            _dataArray = AssetDatabase.FindAssets(filter, lookFor);
            _total = _dataArray.Length;
            _index = 0;
            _extension = extension;
            _action = action;
            _obj1 = obj1;
        }

        public void SearchAndDo(Action endAction)
        {
            _index = 0;
            while (_index < _dataArray.Length)
            {
                var path = AssetDatabase.GUIDToAssetPath(_dataArray[_index]);
                ++_index;
                if (!path.Contains("Temp") && path.EndsWith(_extension))
                    try
                    {
                        var prefab = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
                        var obj = PrefabUtility.InstantiatePrefab(prefab) as T;
                        var hasOverride = _action(path, obj, _obj1);
                        //PrefabUtility.SetPropertyModifications(obj, PrefabUtility.GetPropertyModifications(obj));
                        if (hasOverride) PrefabUtility.ReplacePrefab(obj as GameObject, prefab, ReplacePrefabOptions.Default);
                        //                            PrefabUtility.SavePrefabAsset(prefab as GameObject);
                        Object.DestroyImmediate(obj);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message + "\n" + e.StackTrace);
                    }

                var isCancle = ShowProgressBar();
                if (isCancle || _index >= _total)
                {
                    DoEnd(endAction);
                    break;
                }
            }
        }

        private void DoEnd(Action endAction)
        {
            JQLog.Log("DoEnd:" + _name);
            if (_showUI) EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (endAction != null)
            {
                endAction();
                endAction = null;
            }
        }

        public void Search(Action endAction)
        {
            _index = 0;
            while (_index < _dataArray.Length)
            {
                var path = AssetDatabase.GUIDToAssetPath(_dataArray[_index]);
                ++_index;
                if (!path.Contains("Temp") && path.EndsWith(_extension))
                    try
                    {
                        var prefab = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
                        _curName = path;
                        _action(path, prefab, _obj1);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message + "\n" + e.StackTrace);
                    }

                var isCancle = ShowProgressBar();
                if (isCancle || _index >= _total)
                {
                    DoEnd(endAction);
                    break;
                }
            }
        }

        private bool ShowProgressBar()
        {
            if (!_showUI) return false;
            var count = _index % 3;
            var titleName = _name;
            for (var i = 0; i < count + 1; i++) titleName += ".";
            var isCancle = EditorUtility.DisplayCancelableProgressBar(titleName, $"{_index}/{_total}  {_curName}", (float)_index / _total);
            return isCancle;
        }
    }
}