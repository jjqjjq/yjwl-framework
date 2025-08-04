using System.IO;
using JQCore.tUtil;

namespace JQEditor.tAssetPostprocessor
{
    public class ProjectTypeTools
    {
        public static string resourcesPath = "Assets/Resources/";
        public static string cardPath = $"Assets/{PathUtil.RES_FOLDER}/Card/";
        public static string commonPath = $"Assets/{PathUtil.RES_FOLDER}/Common/";
        public static string tmpFontPath = $"Assets/{PathUtil.RES_FOLDER}/Common/Font_TMP";
        public static string modelPath = $"Assets/{PathUtil.RES_FOLDER}/Model/";
        public static string effectPath = $"Assets/{PathUtil.RES_FOLDER}/Effect/";
        public static string soundPath = $"Assets/{PathUtil.RES_FOLDER}/Sound/";
        public static string uiPath = $"Assets/{PathUtil.RES_FOLDER}/UI/";
        public static string scenePath = $"Assets/{PathUtil.RES_FOLDER}/Scene/";
        public static string lanPath = $"Assets/{PathUtil.RES_FOLDER}/Language/";

        public static bool IsCommonResource(string assetsPath)
        {
            return assetsPath.Contains(commonPath);
        }
        
        public static bool IsTMPFontResource(string assetsPath)
        {
            return assetsPath.Contains(tmpFontPath);
        }

        public static bool IsResourcesResource(string assetsPath)
        {
            return assetsPath.Contains(resourcesPath);
        }

        //判断是否是卡牌文件夹
        public static bool IsCardResource(string assetsPath)
        {
            return assetsPath.Contains(cardPath);
        }

        //判断是否是模型文件夹
        public static bool IsModelResource(string assetsPath)
        {
            return assetsPath.ToLower().EndsWith(".fbx");
        }

        //判断是否是特效文件夹
        public static bool IsTerrainResource(string assetsPath)
        {
            return assetsPath.Contains("Terrain");
        }

        //判断是否是特效文件夹
        public static bool IsEffectResource(string assetsPath)
        {
            return assetsPath.Contains(effectPath);
        }

        //判断是否是光照贴图
        public static bool IsLightmap(string assetsPath)
        {
            string ext = Path.GetExtension(assetsPath);
            string fileName = Path.GetFileName(assetsPath);
            if (fileName.StartsWith("Lightmap-"))
            {
                return true;
            }

            return false;
        }

        //判断是否是模型文件
        public static bool IsSoundResource(string assetsPath)
        {
            return assetsPath.Contains(soundPath);
        }

        //UI
        public static bool IsUIResource(string assetPath)
        {
            return assetPath.Contains(uiPath);
        }

        //场景
        public static bool isSceneResource(string assetPath)
        {
            return assetPath.Contains(scenePath);
        }

        //多语言
        public static bool isLanResource(string assetPath)
        {
            return assetPath.Contains(lanPath);
        }
    }
}