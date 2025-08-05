using System;
using JQCore.tUtil;
using JQCore;
using JQCore.tCfg;
using UnityEditor;
using UnityEngine;

namespace JQEditor.Build
{
    public static class BuildAppInfo
    {
        public static bool isDevelop
        {
            get => EditorPrefs.GetBool(Application.dataPath + "BuildAppInfo.isDevelop", false);
            set => EditorPrefs.SetBool(Application.dataPath + "BuildAppInfo.isDevelop", value);
        }

        public static bool PERFORMANCE_LOG
        {
            get => EditorPrefs.GetBool(Application.dataPath + "BuildAppInfo.PERFORMANCE_LOG", false);
            set => EditorPrefs.SetBool(Application.dataPath + "BuildAppInfo.PERFORMANCE_LOG", value);
        }

        public static bool UseSBP
        {
            get => EditorPrefs.GetBool(Application.dataPath + "BuildAppInfo.UseSBP", true);
            set => EditorPrefs.SetBool(Application.dataPath + "BuildAppInfo.UseSBP", value);
        }

        public static bool UseUPR
        {
            get => EditorPrefs.GetBool(Application.dataPath + "BuildAppInfo.UseUPR", true);
            set => EditorPrefs.SetBool(Application.dataPath + "BuildAppInfo.UseUPR", value);
        }

        public static bool ReporterLog
        {
            get => EditorPrefs.GetBool(Application.dataPath + "BuildAppInfo.ReporterLog", true);
            set => EditorPrefs.SetBool(Application.dataPath + "BuildAppInfo.ReporterLog", value);
        }

        public static bool HybridCLR
        {
            get => EditorPrefs.GetBool(Application.dataPath + "BuildAppInfo.HybridCLR", true);
            set => EditorPrefs.SetBool(Application.dataPath + "BuildAppInfo.HybridCLR", value);
        }

        public static bool IL2CPP
        {
            get => EditorPrefs.GetBool(Application.dataPath + "BuildAppInfo.IL2CPP", true);
            set => EditorPrefs.SetBool(Application.dataPath + "BuildAppInfo.IL2CPP", value);
        }
        
        public static bool useLocalCDN
        {
            get => EditorPrefs.GetBool(Application.dataPath + "BuildAppInfo.useLocalCDN", true);
            set => EditorPrefs.SetBool(Application.dataPath + "BuildAppInfo.useLocalCDN", value);
        }

        public static bool ForceRebuild
        {
            get => EditorPrefs.GetBool(Application.dataPath + "BuildAppInfo.ForceRebuild", false);
            set => EditorPrefs.SetBool(Application.dataPath + "BuildAppInfo.ForceRebuild", value);
        }

        public static bool fullPackage
        {
            get => EditorPrefs.GetBool(Application.dataPath + "BuildAppInfo.fullPackage", false);
            set => EditorPrefs.SetBool(Application.dataPath + "BuildAppInfo.fullPackage", value);
        }
        //
        // public static bool isLog
        // {
        //     get
        //     {
        //         return EditorPrefs.GetBool(Application.dataPath + "BuildAppInfo.isLog", true);
        //     }
        //     set
        //     {
        //         EditorPrefs.SetBool(Application.dataPath + "BuildAppInfo.isLog", value);
        //     }
        // }


        public static string resPath
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.resPath", "E:/work/package/");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.resPath", value);
        }
        
        public static string AppId
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.AppId", "");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.AppId", value);
        }

        public static string identifier
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.identifier", "com.yjwl.xxx");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.identifier", value);
        }

        public static string ProductName
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.productName", "项目名称");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.productName", value);
        }
        
        public static string ProductNick
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.productNick", "productName");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.productNick", value);
        }

        public static string aliyunOSS_endpoint
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.aliyunOSS_endpoint", "https://oss-cn-guangzhou.aliyuncs.com");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.aliyunOSS_endpoint", value);
        }

        public static string aliyunOSS_accessKeyId
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.aliyunOSS_accessKeyId", "");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.aliyunOSS_accessKeyId", value);
        }

        public static string aliyunOSS_accessKeySecret
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.aliyunOSS_accessKeySecret", "");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.aliyunOSS_accessKeySecret", value);
        }


        public static string aliyunCDN_host
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.aliyunCDN_host", "cdn.jijianquan.cn");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.aliyunCDN_host", value);
        }

        public static string aliyunOSS_bucketName
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.aliyunOSS_bucketName", "");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.aliyunOSS_bucketName", value);
        }

        public static string OldPackageVersion
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.OldPackageVersion", "");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.OldPackageVersion", value);
        }

        public static string LocalCDN
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.LocalCDN", "http://127.0.0.1");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.LocalCDN", value);
        }
        
        public static string LocalCDNFloder
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.LocalCDNFloder", "E:\\wamp64\\www");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.LocalCDNFloder", value);
        }
        
        public static string WxCloudFunctionFloder
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.WxCloudFunctionFloder1", $"{Application.dataPath}\\..\\WXCloudFunction\\cloudfunctions");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.WxCloudFunctionFloder1", value);
        }
        
        public static string LocalCfgFloder
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.LocalCfgFloder", "E:\\work\\yjwl-p2\\P2-Server\\Config\\Json\\c");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.LocalCfgFloder", value);
        }
        
        public static string CDN
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.CDN", "https://cdn.jijianquan.cn");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.CDN", value);
        }
        
        public static string PackageVersion
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.PackageVersion", GetBuildPackageVersion());
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.PackageVersion", value);
        }

        public static string LocalWebCdn
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.LocalWebCdn", "");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.LocalWebCdn", value);
        }

        public static string version
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.version", "1.0.0");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.version", value);
        }

        public static string subVersion
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.subVersion", "1");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.subVersion", value);
        }

        public static string platform
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.platform", "none");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.platform", value);
        }
        //
        // public static bool isAssetEncrypt
        // {
        //     get
        //     {
        //         return EditorPrefs.GetBool(Application.dataPath + "BuildAppInfo.isAssetEncrypt", true);
        //     }
        //     set
        //     {
        //         EditorPrefs.SetBool(Application.dataPath + "BuildAppInfo.isAssetEncrypt", value);
        //     }
        // }

        public static string mainScene
        {
            get => EditorPrefs.GetString(Application.dataPath + "BuildAppInfo.mainScene", "Assets/Scenes/main.unity");
            set => EditorPrefs.SetString(Application.dataPath + "BuildAppInfo.mainScene", value);
        }

        /// <summary>
        ///     sdk源路径
        /// </summary>
        public static string sysCfgPath
        {
            get => EditorPrefs.GetString("BuildAppInfo.sysCfgPath", Application.dataPath + $"{PathUtil.RES_FOLDER}/Build/sysCfgs");
            set => EditorPrefs.SetString("BuildAppInfo.sysCfgPath", value);
        }

        /// <summary>
        ///     sdk源路径
        /// </summary>
        public static string androidSdkPath
        {
            get => EditorPrefs.GetString("BuildAppInfo.androidSdkPath", "none");
            set => EditorPrefs.SetString("BuildAppInfo.androidSdkPath", value);
        }

        public static SdkPlatform sdkPlatform
        {
            get
            {
                SdkPlatform sdkPlatform;
                var enumStr = EditorPrefs.GetString("BuildAppInfo.sdkPlatform", SdkPlatform.none.ToString());
                Enum.TryParse(enumStr, out sdkPlatform);
                return sdkPlatform;
            }
            set => EditorPrefs.SetString("BuildAppInfo.sdkPlatform", value.ToString());
        }

        //todo 待删除
        public static string configPath => $"{Application.dataPath}/{PathUtil.RES_FOLDER}/Build/MainSubPackage/";


        public static AppCfgType sysCfgType
        {
            get
            {
                AppCfgType sysCfgType;
                var enumStr = EditorPrefs.GetString("BuildAppInfo.sysCfgType", AppCfgType.outTest.ToString());
                Enum.TryParse(enumStr, out sysCfgType);
                return sysCfgType;
            }
            set => EditorPrefs.SetString("BuildAppInfo.sysCfgType", value.ToString());
        }

        public static void SetStringVal(string key, string val)
        {
            EditorPrefs.SetString(Application.dataPath + "BuildAppInfo." + key, val);
        }

        public static string GetStringVal(string key)
        {
            return EditorPrefs.GetString(Application.dataPath + "BuildAppInfo." + key, "");
        }


        public static void addVersion2()
        {
            var versionArr = BuildAppInfo.version.Split(".");
            var version = int.Parse(versionArr[2]);
            version++;
            BuildAppInfo.version = string.Format("{0}.{1}.{2}", versionArr[0], versionArr[1], version);
        }

        public static void addSubVersion()
        {
            var subVersion = int.Parse(BuildAppInfo.subVersion);
            subVersion++;
            BuildAppInfo.subVersion = subVersion.ToString();
        }

        // 构建版本相关
        private static string GetBuildPackageVersion()
        {
            return DateTime.Now.ToString("yyMMddHHmm");
        }

        //获取当前时间的算上年月日的总分钟数


        public static void updatePackageVersion()
        {
            PackageVersion = GetBuildPackageVersion();
        }
    }
}