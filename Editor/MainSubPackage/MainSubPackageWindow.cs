using JQCore;
using JQEditor.Build;
using JQEditor.Other;
using UnityEditor;
using UnityEngine;

namespace JQEditor.MainSubPackage
{
    public class MainSubPackageWindow : EditorWindow
    {
        [MenuItem("YjwlWindows/MainSubPackage")]
        public static void Init()
        {
            GetWindow(typeof(MainSubPackageWindow), false, "MainSubPackage");
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            BuildAppInfo.OldPackageVersion = EditorGUILayout.TextField("上次上传版本:", BuildAppInfo.OldPackageVersion, GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("App版本:      " + BuildAppInfo.version);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("补丁包版本:      " + BuildAppInfo.PackageVersion);
            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField("CDN", EditorStyle.headGuiStyle);
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            BuildAppInfo.aliyunOSS_endpoint = EditorGUILayout.TextField("endpoint", BuildAppInfo.aliyunOSS_endpoint, GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            BuildAppInfo.aliyunOSS_accessKeyId = EditorGUILayout.TextField("accessKeyId", BuildAppInfo.aliyunOSS_accessKeyId, GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            BuildAppInfo.aliyunOSS_accessKeySecret = EditorGUILayout.TextField("accessKeySecret", BuildAppInfo.aliyunOSS_accessKeySecret, GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            BuildAppInfo.aliyunOSS_bucketName = EditorGUILayout.TextField("bucketName", BuildAppInfo.aliyunOSS_bucketName, GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            BuildAppInfo.aliyunCDN_host = EditorGUILayout.TextField("CDN域名（host）", BuildAppInfo.aliyunCDN_host, GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            BuildAppInfo.sysCfgType = (AppCfgType)EditorGUILayout.EnumPopup("SysCfg:", BuildAppInfo.sysCfgType);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("本地web目录", BuildAppInfo.LocalWebCdn, GUILayout.MinWidth(100f));
            if (GUILayout.Button("...", GUILayout.MaxWidth(20f)))
            {
                BuildAppInfo.LocalWebCdn = EditorUtility.OpenFolderPanel("本地web目录", "", BuildAppInfo.LocalWebCdn);
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(100f));
            GUILayout.Label("补丁包", GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("预热失败点我"))
            {
                MainSubPackageUtil.refreshCDNUpdateDirectory(MainSubPackageUtil.cacheUploadList);
            }
            if (GUILayout.Button("上传到OSS目录"))
            {
                if (string.IsNullOrEmpty(BuildAppInfo.OldPackageVersion))
                {
                    MainSubPackageUtil.uploadAllPackageToOSS(BuildAppInfo.PackageVersion);
                }
                else
                {
                    MainSubPackageUtil.uploadComparePackageToOSS(BuildAppInfo.OldPackageVersion, BuildAppInfo.PackageVersion);
                }
            }


            if (GUILayout.Button("上传到本地web目录"))
            {
                MainSubPackageUtil.uploadAllPackageToLocalWeb(BuildAppInfo.PackageVersion);
            }


            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(100f));
            GUILayout.Label("App版本文件", GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("生成版本文件"))
            {
                MainSubPackageUtil.saveAppVersionFile();
            }

            if (GUILayout.Button("上传到OSS目录"))
            {
                MainSubPackageUtil.uploadAppVersionFileOSS();
            }

            if (GUILayout.Button("上传到本地web目录"))
            {
                MainSubPackageUtil.uploadAppVersionFileLocalWeb();
            }

            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(100f));
            GUILayout.Label("Router文件", GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("生成路由文件"))
            {
                MainSubPackageUtil.saveRouterFile();
            }

            if (GUILayout.Button("上传到OSS目录"))
            {
                MainSubPackageUtil.uploadRouterFileOSS();
            }

            if (GUILayout.Button("上传到本地web目录"))
            {
                MainSubPackageUtil.uploadRouterFileLocalWeb();
            }

            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();

            GUILayout.Space(3f);

            EditorGUILayout.LabelField("主分包", EditorStyle.headGuiStyle);
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("生成首包资源列表"))
            {
                MainSubPackageUtil.saveRecorder();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("刷新资源配置Excel"))
            {
                MainSubPackageUtil.updateResCfgExcel();
            }

            GUILayout.EndHorizontal();


            GUILayout.EndVertical();
        }
    }
}