/*----------------------------------------------------------------
// 文件名：CheckAppGroupInfo.cs
// 文件功能描述：
// 
// 创建者：JJQ
// 时间：2020/10/15 10:40:32
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using JQCore.tLog;
using JQEditor.Other;
using UnityEditor;
using UnityEngine;

namespace JQEditor.Check.tCheckApp
{
    public class CheckAppGroupInfo
    {
        public string Name => _name;

        public List<CheckAppInfo> List => _list;

        private string _name;
        private List<CheckAppInfo> _list = new List<CheckAppInfo>();
        public CheckAppGroupInfo(string name)
        {
            _name = name;
        }

        public void AddInfo(CheckAppInfo checkAppInfo)
        {
            _list.Add(checkAppInfo);
        }
        public void onGroupGUI()
        {
            GUILayout.Space(3f);
            EditorGUILayout.LabelField(_name, EditorStyle.headGuiStyle);
            GUILayout.Space(1f);
            GUILayout.BeginVertical("Box");
            for (int i = 0; i < _list.Count; i++)
            {
                CheckAppInfo info = _list[i];
                info.onGUI();
            }

            onGUI();
            GUILayout.EndVertical();
        }
        public void onGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("All", GUILayout.Width(300f), GUILayout.ExpandWidth(true)))
            {
                _currIndex = 0;
                checkNext();
                lastActionTime = DateTime.Now.ToString("MM-dd HH:mm");
                JQLog.Log("[FINISH]---" + _name);
            }
            EditorGUILayout.LabelField(lastActionTime, EditorStyle.timeStyle, GUILayout.MaxWidth(80f));
            GUILayout.EndHorizontal();
        }

        private int _currIndex;
        private void checkNext()
        {
            if (_currIndex < _list.Count)
            {
                CheckAppInfo info = _list[_currIndex];
                _currIndex++;
                info.execute(checkNext);
            }
            else
            {
                lastActionTime = DateTime.Now.ToString("MM-dd HH:mm");
                JQLog.Log("[FINISH]---" + _name);
            }
        }

        public void deletaTime()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                CheckAppInfo info = _list[i];
                info.deleteTime();
            }

            deleteTime();
        }

        public void deleteTime()
        {
            EditorPrefs.DeleteKey(Application.dataPath + "/" + _name + "/All");
        }

        private string lastActionTime
        {
            get
            {
                return EditorPrefs.GetString(Application.dataPath + "/" + _name + "/All");
            }
            set
            {
                EditorPrefs.SetString(Application.dataPath + "/" + _name + "/All", value);
            }
        }
    }
}
