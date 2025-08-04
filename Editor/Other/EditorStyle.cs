using UnityEditor;
using UnityEngine;

namespace JQEditor.Other
{
    public static class EditorStyle
    {
        private static GUIStyle _headGuiStyle;
        private static GUIStyle _rightStyle;
        private static GUIStyle _timeStyle;

        public static GUIStyle timeStyle
        {
            get
            {
                if (_timeStyle == null)
                {
                    _timeStyle = new GUIStyle();
                    _timeStyle.fontSize = 12;
                    _timeStyle.alignment = TextAnchor.MiddleRight;
                    _timeStyle.normal.textColor = Color.Lerp(Color.white, Color.gray, 0.5f);
                }

                return _timeStyle;
            }
        }
        
        public static GUIStyle headGuiStyle
        {
            get
            {
                if (_headGuiStyle == null)
                {
                    _headGuiStyle = new GUIStyle();
                    _headGuiStyle.fontSize = 12;
                    _headGuiStyle.normal.textColor = Color.Lerp(Color.white, Color.gray, 0.5f);
                    _headGuiStyle.fontStyle = FontStyle.Bold;
                    _headGuiStyle.alignment = TextAnchor.UpperCenter;
                }

                return _headGuiStyle;
            }
        }

        public static GUIStyle rightStyle
        {
            get
            {
                if (_rightStyle == null)
                {
                    _rightStyle = new GUIStyle();
                    _rightStyle.fontSize = 12;
                    _rightStyle.alignment = TextAnchor.MiddleRight;
                    _rightStyle.normal.textColor = Color.Lerp(Color.white, Color.gray, 0.5f);
                }

                return _rightStyle;
            }
        }
    }
}