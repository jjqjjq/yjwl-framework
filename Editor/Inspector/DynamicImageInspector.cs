/*----------------------------------------------------------------
// 文件名：BigImageInspector.cs
// 文件功能描述：
// 
// 创建者：JJQ
// 时间：2020/9/8 19:47:45
//----------------------------------------------------------------*/

using JQCore.DynamicTexture;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace JQEditor.Inspector
{
    [CustomEditor(typeof(DynamicImage))]
    public class DynamicImageInspector : ImageEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DynamicImage dynamicImage = target as DynamicImage;
            GUILayout.Label("path   "+dynamicImage.spritePath);
            GUILayout.Label($"Border    L:{dynamicImage.leftBorder} B:{dynamicImage.bottomBorder} R:{dynamicImage.rightBorder} T:{dynamicImage.topBorder}");
//            EditorGUILayout.ColorField(dynamicImage.originalColor);
            if (GUILayout.Button("SavePath"))
            {
                Debug.Log("SavePath");
                dynamicImage.removeSprite();
            }
            if (GUILayout.Button("RemoveAll"))
            {
                Debug.Log("RemoveAll");
                dynamicImage.removeAllSprite();
            }
            if (GUILayout.Button("RestoreAll"))
            {
                Debug.Log("RestoreAll");
                dynamicImage.restoreAllSprite();
            }
        }
    }
}
