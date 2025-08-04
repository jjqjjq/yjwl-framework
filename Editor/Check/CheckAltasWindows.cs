using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace JQEditor.Check
{
    public class CheckAltasWindows : EditorWindow
    {
        private List<CheckGoodDataInfo> _list = new List<CheckGoodDataInfo>();
        private List<string> _unuseSpriteList = new List<string>();

        private Vector2 scrollPos1 = Vector2.zero;
        private Vector2 scrollPos2 = Vector2.zero;

        private void OnGUI()
        {

            if (GUILayout.Button("Refresh"))
            {
                refresh(true);
            }
            EditorGUILayout.BeginHorizontal();
            //创建 scrollView  窗口  
            scrollPos1 = EditorGUILayout.BeginScrollView(scrollPos1);
            Texture2D texture2D = new Texture2D(64, 64);
            for (int i = 0; i < _list.Count; i++)
            {
                CheckGoodDataInfo info = _list[i];
                EditorGUILayout.BeginHorizontal();
                Sprite sprite = info.sprite;
                uint id = info.id;
                string name = info.name;
                Texture2D showTexture2D = null;
                if (sprite != null)
                {
                    showTexture2D = sprite.texture;
                }
                else
                {
                    showTexture2D = texture2D;
                }

                GUILayout.Label(id.ToString());
                GUILayout.Label(name);
                GUILayout.Toggle(false, showTexture2D, new[] {GUILayout.Height(showTexture2D.height), GUILayout.Width(showTexture2D.width)});
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(10);
            }

            EditorGUILayout.EndScrollView(); //结束 ScrollView 窗口  


            GUILayout.Label("未用到的图标");
            scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2);
            for (int i = 0; i < _unuseSpriteList.Count; i++)
            {
                string name = _unuseSpriteList[i];
                GUILayout.Label(name);
            }

            EditorGUILayout.EndScrollView(); //结束 ScrollView 窗口  

            EditorGUILayout.EndHorizontal();
        }

        private void refresh(bool always)
        {
//            _list = new List<CheckGoodDataInfo>();
//            _unuseSpriteList = new List<string>();
//            GameDataUtil.initData(always);//初始化数据
//            GameObject itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CheckAltasTools.commonAtlasPrefabPath);
//            Transform iconRoot = itemPrefab.transform.Find(CommonAtlasManager.TYPE_ITEM_SPRITE);
//            Dictionary<string, Sprite> spriteLibIdc = new Dictionary<string, Sprite>();
//            Dictionary<string, int> spriteCountdc = new Dictionary<string, int>();
//            foreach (Transform childTransform in iconRoot)
//            {
//                Image image = childTransform.GetComponent<Image>();
//                if (image != null)
//                {
//                    spriteLibIdc[childTransform.name] = image.sprite;
//                    spriteCountdc[childTransform.name] = 0;
//                }
//            }
//
//            foreach (KeyValuePair<string, GoodsData> keyValuePair in GoodsData.dataMap)
//            {
//                CheckGoodDataInfo info = new CheckGoodDataInfo();
//                GoodsData goodsData = keyValuePair.Value;
//                info.id = goodsData.id;
//                info.name = goodsData.name;
//                string name = CommonAtlasManager.TYPE_ITEM_SPRITE.Replace("library", goodsData.idStr);
//                if (spriteLibIdc.ContainsKey(name))
//                {
//                    info.sprite = spriteLibIdc[name];
//                    spriteCountdc[name]++;
//                }
//                _list.Add(info);
//            }
//
//            foreach (KeyValuePair<string, int> keyValuePair in spriteCountdc)
//            {
//                string name = keyValuePair.Key;
//                int count = keyValuePair.Value;
//                if (count == 0)
//                {
//                    _unuseSpriteList.Add(name);
//                }
//            }
        }

        private void OnEnable()
        {
            refresh(true);
        }

        public class CheckGoodDataInfo
        {
            public uint id;
            public Sprite sprite;
            public string name;
        }
    }
}
