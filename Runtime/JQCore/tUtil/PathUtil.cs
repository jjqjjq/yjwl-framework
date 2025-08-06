using System.IO;
using UnityEngine;

namespace JQCore.tUtil
{
    public static class PathUtil
    {
        public const int EncryptHeadLen = 4;
        public static string DOWNLOAD_FOLDER = "res"; //热更新后的持久化目录
        public static string RES_FOLDER = "Art";
        public static string DLL_FOLDER = "Code";

        public static string CODE_DLL_LIB = "CodeDllLib.prefab";

        // public static string PACKAGE_NAME = "asset";
        public static string YOOASSET_PACKAGE_NAME = "asset";

        public static string LUA_FOLDER = "LuaScripts";


        public static string platformName
        {
            get
            {
#if UNITY_ANDROID
                    return "android";
#elif UNITY_IOS
                    return "iphone";
#elif UNITY_STANDALONE_WIN
                    return "windows";
#elif SDK_WEIXIN
                return "weixin";
#elif SDK_DOUYIN
                return "douyin";
#endif
            }
        }


        #region all path

        //streamingAsset路径
        public static string getStreamPathByPlatform(string path, bool isWWW)
        {
            if (isWWW)
            {
#if UNITY_EDITOR
#if UNITY_EDITOR_WIN
                path = "file:///" + path;
#else
                path = "file://" + path;
#endif
#elif UNITY_STANDALONE_WIN
                path = "file:///" + path;
#elif UNITY_ANDROID
                path = path;
#endif
            }
#if UNITY_IPHONE
            path = "file://" + path;
#endif
            return path;
        }

        //持久化路径
        private static string getPersistentDataPathByPlatform(string path)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (false) path = "file:///" + path;
#elif UNITY_ANDROID
            if(false)
            {
                path = "file:///" + path;
            }
            else
            {
                
            }
#elif UNITY_IPHONE
            if(true)
            {
                path = "file:///" + path;
            }
            else
            {
                
            }
#endif
            return path;
        }

        //editor本地路径
        private static string getDataPathByPlatform(string path, bool isWWW)
        {
            if (isWWW)
            {
#if UNITY_EDITOR
#if UNITY_EDITOR_WIN
                path = "file:///" + path;
#else
                path = "file://" + path;
#endif
#elif UNITY_STANDALONE_WIN
                path = "file:///" + path;
#elif UNITY_ANDROID
                path = path;
#elif UNITY_IPHONE || UNITY_IOS
                path = path;
#endif
            }

            return path;
        }

        #endregion

        #region persistentData目录

        public static string URL_PERSISTENT = Application.persistentDataPath + "/" + DOWNLOAD_FOLDER;

        public static string getPersistentResPath(string path)
        {
            var returnPath = Path.Combine(URL_PERSISTENT, path).Replace("\\", "/");
            return getPersistentDataPathByPlatform(returnPath);
        }

        #endregion

        #region Res目录

        public static string URL_ASSETS_DATA = Application.dataPath + $"/{RES_FOLDER}/Data/";

        public static string getResPath(string path, bool isWWW)
        {
            var returnPath = Path.Combine(URL_ASSETS_DATA, path);
            return getDataPathByPlatform(returnPath, isWWW);
        }

        #endregion

        #region StreamingAssets目录

        public static string URL_STREAM_XML = Application.streamingAssetsPath + "/xml/";
        public static string URL_STREAM_DATA = Application.streamingAssetsPath + "/data/";
        public static string URL_STREAM_RES = Application.streamingAssetsPath + "/resource/";
        public static string URL_STREAM_RES_ENCRYPT = Application.streamingAssetsPath + "/resourceEncrypt/";
        public static string URL_STREAM_APK_RES = Application.streamingAssetsPath + "/apk_res/";

        public static string getStreamingXmlFloder(string path, bool isWWW)
        {
            var returnPath = Path.Combine(URL_STREAM_XML, path);

            return getStreamPathByPlatform(returnPath, isWWW);
        }

        public static string getStreamingXmlFile(string path, bool isWWW)
        {
            var returnPath = Path.Combine(URL_STREAM_XML, path + ".xml");
            return getStreamPathByPlatform(returnPath, isWWW);
        }

        public static string getStreamingDataPath(string path, bool isWWW)
        {
            var returnPath = Path.Combine(URL_STREAM_DATA, path + ".dat");
            return getStreamPathByPlatform(returnPath, isWWW);
        }

        public static string getStreamingResPath(string path, bool isWWW)
        {
            string returnPath;
            returnPath = Path.Combine(URL_STREAM_RES, path);
            return getStreamPathByPlatform(returnPath, isWWW);
        }

        public static string getStreamingApkResPath(string path, bool isWWW)
        {
            var returnPath = Path.Combine(URL_STREAM_APK_RES, path);
            return getStreamPathByPlatform(returnPath, isWWW);
        }

        #endregion


        public static void printGo(GameObject gameObject)
        {
            var fullName = getFullPath(gameObject);
            Debug.Log("fullPaht:" + fullName);
        }

        public static string getFullPath(GameObject gameObject)
        {
            var nameStr = "";
            var fullName = getParentName(gameObject.transform, nameStr);
            return fullName;
        }

        private static string getParentName(Transform transform, string nameStr)
        {
            nameStr = transform.name + "/" + nameStr;
            if (transform.parent != null) return getParentName(transform.parent, nameStr);
            return nameStr;
        }
    }
}