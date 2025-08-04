using System;
using JQEditor.Other;
using UnityEditor;
using UnityEngine;

namespace JQEditor.Check.tCheckApp
{
    public class CheckAppInfo
    {
        private string _groupName;
        private string _name;
        private Action<string, Action> _checkAction;

        public CheckAppInfo(CheckAppGroupInfo checkGroup, string name, Action<string, Action> checkAction)
        {
            _name = name;
            _checkAction = checkAction;
            _groupName = checkGroup.Name;
        }

        public void execute(Action endAction)
        {
            _checkAction(_name, endAction);
            lastActionTime = DateTime.Now.ToString("MM-dd HH:mm");
            Debug.Log("[FINISH]---" + _name);
        }

        public void onGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(_name, GUILayout.Width(300f), GUILayout.ExpandWidth(true)))
            {
                execute(null);
            }
            EditorGUILayout.LabelField(lastActionTime, EditorStyle.timeStyle, GUILayout.MaxWidth(80f));
            GUILayout.EndHorizontal();
        }

        public void deleteTime()
        {
            EditorPrefs.DeleteKey(Application.dataPath + "/" + _groupName + "/" + _name);
        }

        private string lastActionTime
        {
            get
            {
                return EditorPrefs.GetString(Application.dataPath + "/" + _groupName + "/" + _name);
            }
            set
            {
                EditorPrefs.SetString(Application.dataPath + "/" + _groupName + "/" + _name, value);
            }
        }
    }
}
