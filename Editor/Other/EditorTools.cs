using System;
#if Cinemachine
using Cinemachine;
#endif
using UnityEditor;
using UnityEngine;

namespace JQEditor.Other
{
    public class EditorTools
    {


#if Cinemachine
        [MenuItem("GameObject/SPI/添加FreeLook", false, 0)]
        public static void CreateMediaPlayerEditor()
        {
            GameObject go = new GameObject("CM FreeLook");
            go.transform.SetParent(Selection.activeTransform);
            go.transform.localPosition = Vector3.zero;
            CinemachineFreeLook cinemachineFreeLook = go.AddComponent<CinemachineFreeLook>();
            cinemachineFreeLook.m_YAxis.m_InputAxisName = String.Empty;
            cinemachineFreeLook.m_XAxis.m_InputAxisName = String.Empty;
            cinemachineFreeLook.Follow = Selection.activeTransform;
            cinemachineFreeLook.LookAt = Selection.activeTransform;
            cinemachineFreeLook.m_BindingMode = CinemachineTransposer.BindingMode.LockToTarget;
            cinemachineFreeLook.enabled = false;
        }

#endif

        private static bool mEndHorizontal = false;
        private static GUIStyle _style1;

        public static GUIStyle Style1
        {
            get
            {
                if (_style1 == null)
                {
                    _style1 = new GUIStyle();
                    _style1.alignment = TextAnchor.MiddleCenter;
                    _style1.fontSize = 12;
                    _style1.normal.textColor = Color.white;
                }

                return _style1;
            }
        }

        public static void BeginContents()
        {
            BeginContents(false);
        }

        /// <summary>
        /// Begin drawing the content area.
        /// </summary>
        public static void BeginContents(bool minimalistic)
        {
            if (!minimalistic)
            {
                mEndHorizontal = true;
                GUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
            }
            else
            {
                mEndHorizontal = false;
                EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
                GUILayout.Space(10f);
            }

            GUILayout.BeginVertical();
            GUILayout.Space(2f);
        }

        public static void SetLabelWidth(float width)
        {
            EditorGUIUtility.labelWidth = width;
        }

        /// <summary>
        /// End drawing the content area.
        /// </summary>
        public static void EndContents()
        {
            GUILayout.Space(3f);
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            if (mEndHorizontal)
            {
                GUILayout.Space(3f);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(3f);
        }
    }
}