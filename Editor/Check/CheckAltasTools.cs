using System;
using JQCore.tUtil;
using JQEditor.Build;
using JQEditor.tAssetPostprocessor;
using UnityEditor;
using UnityEngine;

namespace JQEditor.Check
{
    public static class CheckAltasTools
    {
        public static string commonAtlasPrefabPath = $"Assets/{PathUtil.RES_FOLDER}/Atlas_al/AtlasPrefab.prefab";

        public static void checkAll()
        {
        }

        public static void PutIconsButtomAnchor(string name, Action endAction)
        {
            CheckCommonTools.Search<Texture2D>(name, $"Assets/{PathUtil.RES_FOLDER}/UI/ProgramLoad/GoodsIcon", changeToButtomAnchor, null, endAction, true, ".png", "t:Texture");
        }
        
        private static bool changeToButtomAnchor(string assetPath, Texture2D animatorController, object obj1)
        {
            Debug.LogError("changeToButtomAnchor:"+assetPath);
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            textureImporter.spritePivot = new Vector2(0.5f, 0);
            // textureImporter.SaveAndReimport();
            return true;
        }

        //[MenuItem("IrfTools/加工流程/图集处理")]
        public static void PutIconsToPrefab(string name, Action endAction)
        {
            //类型图集
            CheckGlobalTools.addAssetToAtlasLib<Sprite>("GoodsIcon", "UI/SpriteLib/GoodsIconLib", "t:texture2D");
            CheckGlobalTools.addAssetToAtlasLib<Sprite>("ItemIcon", "UI/SpriteLib/ItemIconLib", "t:texture2D");
            //杂项图集（程序代码会设置）
            CheckGlobalTools.addAssetToAtlasLib<Sprite>("CommonIcon", "UI/SpriteLib/CommonIconUILib", "t:texture2D"); //通用图集 程序也会用
            CheckGlobalTools.addAssetToAtlasLib<Sprite>("CommonBigIcon", "UI/SpriteLib/CommonBigIconUILib", "t:texture2D"); //通用图集 程序也会用
            CheckGlobalTools.addAssetToAtlasLib<Sprite>("MainIcon", "UI/SpriteLib/MainIconUILib", "t:texture2D"); //主界面兼容图集

            
            AssetDatabase.Refresh();
            endAction?.Invoke();
        }

        //[MenuItem("IrfTools/道具图标缺失检查")]
        public static void CheckGoodsData()
        {
            EditorWindow.GetWindow(typeof(CheckAltasWindows), false, "检查道具图标");
            return;
//            GameDataUtil.initData();
//            GameObject itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(commonAtlasPrefabPath);
//            Transform iconRoot = itemPrefab.transform.Find(CommonAtlasManager.TYPE_ITEM_SPRITE);
//            foreach (KeyValuePair<string, GoodsData>  keyValuePair in GoodsData.dataMap)
//            {
//                GoodsData goodsData = keyValuePair.Value;
//                string name = CommonAtlasManager.TYPE_ITEM_SPRITE.Replace("library", goodsData.idStr);
//                Transform icon = iconRoot.Find(name);
//                if (icon == null)
//                {
//                    HyDebug.LogError("道具图标不存在:"+ goodsData.idStr);
//                }
//                else
//                {
//                    Image image = icon.GetComponent<Image>();
//                    if (image.sprite == null)
//                    {
//                        HyDebug.LogError("道具图标不存在:" + goodsData.idStr);
//                    }
//                }
//            }
        }
//
//        public static void CheckSpritePacker()
//        {
//            string[] atlasNames = Packer.atlasNames;
//            foreach (string atlasName in atlasNames)
//            {
//                Texture2D[] texture2Ds = Packer.GetTexturesForAtlas(atlasName);
//                for (int i = 0; i < texture2Ds.Length; i++)
//                {
//                    Texture2D texture2D = texture2Ds[i];
//                    if (texture2D.width > 1024 || texture2D.height > 1024)
//                    {
//                        HyDebug.Log("[这个图集挺大的，看能缩减不？]"+atlasName);
//                    }
//                }
//            }
//        }
    }
}