using System;
using System.Collections.Generic;
using JQEditor.Check.tCheckApp;
using UnityEditor;
using UnityEngine;

namespace JQEditor.Check.tCheckApp
{
    public class CheckAppWindow : EditorWindow
    {
        [MenuItem("YjwlWindows/CheckApp")]
        public static void Init()
        {
            GetWindow(typeof(CheckAppWindow), false, "Check_App");
        }

        Vector2 scrollViewPos = Vector2.zero;
        
        private void OnGUI()
        {
            scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos);
            List<CheckAppGroupInfo> list = CheckApp.Instance.List;
            foreach (CheckAppGroupInfo checkAppGroupInfo in list)
            {
                checkAppGroupInfo.onGroupGUI();
            }

            if (GUILayout.Button("清除时间"))
            {
                foreach (CheckAppGroupInfo checkAppGroupInfo in list)
                {
                    checkAppGroupInfo.deletaTime();
                }
            }
            EditorGUILayout.EndScrollView();
        }
        

 
    }
}
