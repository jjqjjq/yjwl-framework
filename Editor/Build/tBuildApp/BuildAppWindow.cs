using System;
using System.Collections.Generic;
using JQCore;
using JQCore.tFileSystem;
using JQEditor.MainSubPackage;
using JQEditor.Other;
using JQCore.tCfg;
using UnityEditor;
using UnityEngine;
#if SDK_WEIXIN
using WeChatWASM;
#endif
namespace JQEditor.Build
{
    public class BuildAppWindow : EditorWindow
    {
        private string[] _sceneArray;
        private int _sceneIndex;

        private Vector2 scrollViewPos = Vector2.zero;
       
        

        private void OnBecameVisible()
        {
            var files = new List<string>();
            JQFileUtil.getAllFile(ref files, $"{Application.dataPath}/Scenes", isScene, true);
            _sceneArray = new string[files.Count];
            for (var i = 0; i < files.Count; i++) _sceneArray[i] = JQFileUtil.getCurrFolderOrFileName(files[i]);
            _sceneIndex = Array.IndexOf(_sceneArray, BuildAppInfo.mainScene);
        }

        private void OnGUI()
        {
            scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos);
            EditorTools.SetLabelWidth(80f);
            GUILayout.Space(3f);

            thisGUI();

            BuildApp.OnGUI();
            EditorGUILayout.EndScrollView();
        }

        [MenuItem("YjwlWindows/BuildApp")]
        public static void Init()
        {
            GetWindow(typeof(BuildAppWindow), false, "Build_App");
        }

        private static bool isScene(string name)
        {
            return name.EndsWith(".scene") || name.EndsWith(".unity");
        }

        private void thisGUI()
        {
            GUILayout.Space(3f);
            GUILayout.BeginVertical();
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    EditorGUILayout.LabelField("PC", EditorStyle.headGuiStyle);
                    break;
                case BuildTarget.Android:
                    EditorGUILayout.LabelField("Android", EditorStyle.headGuiStyle);
                    break;
                case BuildTarget.iOS:
                    EditorGUILayout.LabelField("iOS", EditorStyle.headGuiStyle);
                    break;
            }

            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            EditorTools.BeginContents(); //========================
            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            BuildAppInfo.isDevelop =
                EditorGUILayout.Toggle("Development Build", BuildAppInfo.isDevelop, GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();

            // GUILayout.BeginHorizontal();
            // BuildAppInfo.isLog = EditorGUILayout.Toggle("Log", BuildAppInfo.isLog, GUILayout.MinWidth(100f));
            // GUILayout.EndHorizontal();
            //
            // GUILayout.BeginHorizontal();
            // BuildAppInfo.isAssetEncrypt = EditorGUILayout.Toggle("资源加密", BuildAppInfo.isAssetEncrypt, GUILayout.MinWidth(100f));
            // GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            BuildAppInfo.PERFORMANCE_LOG =
                EditorGUILayout.Toggle("性能日志", BuildAppInfo.PERFORMANCE_LOG, GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            BuildAppInfo.UseUPR = EditorGUILayout.Toggle("UPR性能测试", BuildAppInfo.UseUPR, GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();

            if (BuildAppInfo.UseUPR)
            {
                BuildAppInfo.ReporterLog = false;
                BuildAppInfo.HybridCLR = false;
                BuildAppInfo.IL2CPP = false;
            }
            else
            {
                GUILayout.BeginHorizontal();
                BuildAppInfo.ReporterLog =
                    EditorGUILayout.Toggle("Reporter日志", BuildAppInfo.ReporterLog, GUILayout.MinWidth(100f));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                BuildAppInfo.HybridCLR =
                    EditorGUILayout.Toggle("HybridCLR", BuildAppInfo.HybridCLR, GUILayout.MinWidth(100f));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (BuildAppInfo.HybridCLR)
                    BuildAppInfo.IL2CPP = true;
                else
                    BuildAppInfo.IL2CPP =
                        EditorGUILayout.Toggle("IL2CPP", BuildAppInfo.IL2CPP, GUILayout.MinWidth(100f));

                GUILayout.EndHorizontal();
            }
#if SDK_WEIXIN
            GUILayout.BeginHorizontal();
            BuildAppInfo.useLocalCDN =
                EditorGUILayout.Toggle("UseLocalCDN", BuildAppInfo.useLocalCDN, GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();
#endif

#if HYBRIDCLR
            if (SettingsUtil.Enable != BuildAppInfo.HybridCLR)
            {
                SettingsUtil.Enable = BuildAppInfo.HybridCLR;
            }
#endif


            GUILayout.BeginHorizontal();
            BuildAppInfo.fullPackage = EditorGUILayout.Toggle("整包", BuildAppInfo.fullPackage, GUILayout.MinWidth(100f));
            if (GUILayout.Button("复制AB到StreamingAssets")) MainSubPackageUtil.CopyMainPackageToStreamAsseting(null);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            BuildAppInfo.ForceRebuild =
                EditorGUILayout.Toggle("强制重打AB", BuildAppInfo.ForceRebuild, GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            BuildAppInfo.UseSBP = EditorGUILayout.Toggle("SBP", BuildAppInfo.UseSBP, GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            BuildAppInfo.identifier =
                EditorGUILayout.TextField("包名", BuildAppInfo.identifier, GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            BuildAppInfo.ProductName =
                EditorGUILayout.TextField("应用名", BuildAppInfo.ProductName, GUILayout.MinWidth(100f));
            BuildAppInfo.ProductNick =
                EditorGUILayout.TextField("英文名", BuildAppInfo.ProductNick, GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            BuildAppInfo.version =
                EditorGUILayout.TextField("AppVersion", BuildAppInfo.version, GUILayout.MinWidth(100f));
            BuildAppInfo.subVersion =
                EditorGUILayout.TextField("subVersion", BuildAppInfo.subVersion, GUILayout.MinWidth(100f));

            if (GUILayout.Button("增加小版本"))
            {
                BuildAppInfo.addVersion2();
                BuildAppInfo.addSubVersion();
            }

            GUILayout.EndHorizontal();

#if SDK_WEIXIN
            if (BuildAppInfo.useLocalCDN)
            {
                GUILayout.BeginHorizontal();
                BuildAppInfo.LocalCDN =
                    EditorGUILayout.TextField("本地CDN域名", BuildAppInfo.LocalCDN, GUILayout.MinWidth(100f));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                BuildAppInfo.LocalCDNFloder =
                    EditorGUILayout.TextField("CDN本地路径", BuildAppInfo.LocalCDNFloder, GUILayout.MinWidth(100f));
                GUILayout.EndHorizontal();
                
            }
            else
            {
                GUILayout.BeginHorizontal();
                BuildAppInfo.CDN = EditorGUILayout.TextField("CDN域名", BuildAppInfo.CDN, GUILayout.MinWidth(100f));
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            BuildAppInfo.LocalCfgFloder =
                EditorGUILayout.TextField("Cfg本地路径", BuildAppInfo.LocalCfgFloder, GUILayout.MinWidth(100f));
            if (!BuildAppInfo.useLocalCDN)
            {
                if (GUILayout.Button("上传配置", GUILayout.MaxWidth(204f))) BuildApkUtil.uploadCfgToCDN(null);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            BuildAppInfo.WxCloudFunctionFloder = EditorGUILayout.TextField("微信云开发", BuildAppInfo.WxCloudFunctionFloder, GUILayout.MinWidth(100f));
            if (GUILayout.Button("WX->U3D", GUILayout.MaxWidth(100f))) BuildApkUtil.copyWxCloudFunction(false, null);
            if (GUILayout.Button("U3D->WX", GUILayout.MaxWidth(100f))) BuildApkUtil.copyWxCloudFunction(true, null);
            GUILayout.EndHorizontal();
#endif

            GUILayout.BeginHorizontal();
            BuildAppInfo.PackageVersion = EditorGUILayout.TextField("PackageVersion", BuildAppInfo.PackageVersion,
                GUILayout.MinWidth(100f));
            if (GUILayout.Button("新版本", GUILayout.MaxWidth(204f))) BuildAppInfo.updatePackageVersion();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("打包路径", BuildAppInfo.resPath, GUILayout.MinWidth(100f));
            if (GUILayout.Button("...", GUILayout.MaxWidth(20f)))
                BuildAppInfo.resPath = EditorUtility.OpenFolderPanel("打包路径", "", BuildAppInfo.resPath);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("syscfg路径", BuildAppInfo.sysCfgPath, GUILayout.MinWidth(100f));
            if (GUILayout.Button("...", GUILayout.MaxWidth(20f)))
                BuildAppInfo.sysCfgPath = EditorUtility.OpenFolderPanel("syscfg路径", "", BuildAppInfo.sysCfgPath);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("sdk路径", BuildAppInfo.androidSdkPath, GUILayout.MinWidth(100f));
            if (GUILayout.Button("...", GUILayout.MaxWidth(20f)))
                BuildAppInfo.androidSdkPath = EditorUtility.OpenFolderPanel("sdk路径", "", BuildAppInfo.androidSdkPath);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            BuildAppInfo.sdkPlatform =
                (SdkPlatform)EditorGUILayout.EnumPopup("SDK:", BuildAppInfo.sdkPlatform);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            BuildAppInfo.sysCfgType = (AppCfgType)EditorGUILayout.EnumPopup("SysCfg:", BuildAppInfo.sysCfgType);
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            // BuildAppInfo.mainScene = EditorGUILayout.TextField("主场景", BuildAppInfo.mainScene, GUILayout.MinWidth(100f));
            _sceneIndex = EditorGUILayout.Popup("主场景", _sceneIndex, _sceneArray);
            if (_sceneIndex >= 0 && _sceneIndex < _sceneArray.Length) BuildAppInfo.mainScene = _sceneArray[_sceneIndex];

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            EditorTools.EndContents();


            //========================
        }

    }
}