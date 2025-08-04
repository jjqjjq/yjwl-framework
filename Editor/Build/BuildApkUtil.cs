/*----------------------------------------------------------------
// 文件名：BuildApkUtil.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/10/15 14:37:59
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JQCore.tCfg;
using JQCore.tFileSystem;
using JQCore.tJson;
using JQCore.tUtil;
using JQEditor.MainSubPackage;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
#if SDK_WEIXIN
using UnityEngine.SceneManagement;
using WeChatWASM;
#endif
using Debug = UnityEngine.Debug;

namespace JQEditor.Build
{
    public static class BuildApkUtil
    {
//        public const string startScene = "Assets/Scene/main.unity";
//        private static string[] buildScens = new[] { startScene };
        private static string _apkFileName;
        private static BuildOptions _buildOptions;

        private static string getAppKey()
        {
            return BuildAppInfo.identifier;
        }

        private static void setFileName()
        {
            string fileName = null;
            var head = BuildAppInfo.ProductNick;
            var versionStr = BuildAppInfo.version.Replace(".", "-");
            fileName = string.Format("{0}_{1}_{2}_{3}_{4}", head, BuildAppInfo.platform, versionStr,
                BuildAppInfo.PackageVersion, BuildAppInfo.sysCfgType);
            if (BuildAppInfo.isDevelop) //测试包
                fileName += "_dep";
            // if (BuildAppInfo.isLog)//测试包
            // {
            //     fileName += "_log";
            // }
            //
            //            if (BuildAppInfo.isGM)
            //            {
            //                fileName += "_gm";
            //            }
            _apkFileName = fileName;
        }

#if SDK_WEIXIN
        private static WXEditorScriptObject _config;

        private static WXEditorScriptObject config
        {
            get
            {
                if (_config == null)
                {
                    _config = WeChatWASM.UnityUtil.GetEditorConf();
                }

                return _config;
            }
        }
#endif

        private static void setOption()
        {
            if (BuildAppInfo.isDevelop)
                //BuildOptions.AllowDebugging IL2CPP开启这个选项会报错，先不管
                _buildOptions = BuildOptions.Development | BuildOptions.ConnectWithProfiler |
                                BuildOptions.CompressWithLz4;
            else
                _buildOptions = BuildOptions.CompressWithLz4;

#if SDK_WEIXIN


            if (BuildAppInfo.useLocalCDN)
            {
                config.ProjectConf.CDN = $"{BuildAppInfo.LocalCDN}/{BuildAppInfo.ProductNick}";
            }
            else
            {
                string sysCfgFileName = BuildAppInfo.sysCfgType.ToString();
                config.ProjectConf.CDN =
                    $"{BuildAppInfo.CDN}/{sysCfgFileName}/CDN/{PathUtil.platformName}/v{BuildAppInfo.version}";
            }

            config.CompileOptions.DevelopBuild = BuildAppInfo.isDevelop;
            EditorUtility.SetDirty(config);
#endif
        }

        private static BuildTargetGroup getBuildTargetGroup()
        {
            var buildTargetGroup = BuildTargetGroup.Unknown;
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    buildTargetGroup = BuildTargetGroup.Standalone;
                    break;
                case BuildTarget.Android:
                    buildTargetGroup = BuildTargetGroup.Android;
                    break;
                case BuildTarget.iOS:
                    buildTargetGroup = BuildTargetGroup.iOS;
                    break;
                case BuildTarget.WebGL:
                    buildTargetGroup = BuildTargetGroup.WebGL;
                    break;
#if SDK_WEIXIN
                // case BuildTarget.WeixinMiniGame:
                //     buildTargetGroup = BuildTargetGroup.WeixinMiniGame;
                //     break;
#endif
            }

            return buildTargetGroup;
        }

        private static void setPlayerSetting()
        {
            var appKey = getAppKey();
            var productName = BuildAppInfo.ProductName;
            var icons = new Texture2D[1];
            icons[0] =
                AssetDatabase.LoadMainAssetAtPath($"Assets/{PathUtil.RES_FOLDER}/Build/Icon/apkIcon.png") as Texture2D;
            if (BuildAppInfo.isDevelop)
            {
                appKey += "Dep";
                productName += "Dep";
                icons[0] =
                    AssetDatabase.LoadMainAssetAtPath($"Assets/{PathUtil.RES_FOLDER}/Build/Icon/depIcon.png") as
                        Texture2D;
            }

            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, icons);
            PlayerSettings.productName = productName;
            var buildTargetGroup = getBuildTargetGroup();
            if (buildTargetGroup == BuildTargetGroup.iOS)
                PlayerSettings.SetApplicationIdentifier(buildTargetGroup, appKey);
            else
                PlayerSettings.SetApplicationIdentifier(buildTargetGroup, appKey);
            PlayerSettings.keystorePass = "7428%-hdK";
            PlayerSettings.keyaliasPass = "7428%-hdK";
            PlayerSettings.bundleVersion = BuildAppInfo.version;
            PlayerSettings.iOS.buildNumber = BuildAppInfo.subVersion;
            PlayerSettings.Android.keystoreName = "yjwl.keystore";
            PlayerSettings.Android.keyaliasName = "yjwl";
            PlayerSettings.Android.bundleVersionCode = int.Parse(BuildAppInfo.subVersion);


            if (BuildAppInfo.IL2CPP)
                PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.IL2CPP);
            else
                PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.Mono2x);


            //是否开启性能日志
            if (BuildAppInfo.PERFORMANCE_LOG)
            {
                AddDefineSymbols("PERFORMANCE_LOG");
            }
            else
            {
                RemoveDefineSymbols("PERFORMANCE_LOG");
            }

            if (BuildAppInfo.useLocalCDN)
            {
                PlayerSettings.insecureHttpOption = InsecureHttpOption.AlwaysAllowed;
            }
            else
            {
                PlayerSettings.insecureHttpOption = InsecureHttpOption.NotAllowed;
            }

            //sdk平台宏设置
            RemoveDefineSymbols("PLATFORM_", true);
            if (BuildAppInfo.sdkPlatform == SdkPlatform.none) return;
            var type = "PLATFORM_" + BuildAppInfo.sdkPlatform.ToString().ToUpper();
            AddDefineSymbols(type);
        }

        public static void AddDefineSymbols(string symbol)
        {
            var buildTargetGroup = getBuildTargetGroup();
            var defineSymbolsStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var symbolArr = defineSymbolsStr.Split(';');
            var symbolList = new List<string>(symbolArr);
            if (!symbolList.Contains(symbol)) symbolList.Add(symbol);
            var newSymbolStr = string.Join(";", symbolList.ToArray());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, newSymbolStr);
        }

        public static void RemoveDefineSymbols(string symbol, bool isStartWith = false)
        {
            var buildTargetGroup = getBuildTargetGroup();
            var defineSymbolsStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var symbolArr = defineSymbolsStr.Split(';');
            var symbolList = new List<string>(symbolArr);
            if (isStartWith)
            {
                symbolList.RemoveAll(s => s.StartsWith(symbol));
            }
            else
            {
                if (symbolList.Contains(symbol)) symbolList.Remove(symbol);
            }

            var newSymbolStr = string.Join(";", symbolList.ToArray());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, newSymbolStr);
        }

        public static void RenameResources(bool isInClude, Action endAction = null)
        {
            if (isInClude)
            {
                AssetDatabase.RenameAsset("Assets/Plugins/TextMesh Pro/Resources_Exclude", "Resources");
            }
            else
            {
                AssetDatabase.RenameAsset("Assets/Plugins/TextMesh Pro/Resources", "Resources_Exclude");
            }

            AssetDatabase.Refresh();
            endAction?.Invoke();
        }

        public static void build(bool showUI, Action endAction = null)
        {
            //            removeOtherPlatform();
            setFileName();
            setOption();
            setPlayerSetting();
            SetSysCfgs();
            startBuildApk();
            if (endAction != null) endAction();
        }

        private static void SetSysCfgs()
        {
            ChangeSysCfg();
            var cfgJsonPath = $"{Application.streamingAssetsPath}/syscfg.json";
            var result = File.ReadAllText(cfgJsonPath);
            var jsonObject = new JSONObject(result);
            jsonObject.SetField("AppVersion", BuildAppInfo.version);
            jsonObject.SetField("HybridCLR", BuildAppInfo.HybridCLR.ToString());
            jsonObject.SetField("ReporterLog", BuildAppInfo.ReporterLog.ToString());
            jsonObject.SetField("SdkPlatform", BuildAppInfo.sdkPlatform.ToString());
#if !UNITY_ANDROID
            if (BuildAppInfo.useLocalCDN)
            {
                jsonObject.SetField("CDN", BuildAppInfo.LocalCDN);
            }
            else
            {
                jsonObject.SetField("CDN", $"{BuildAppInfo.CDN}/{BuildAppInfo.sysCfgType}");
            }
#endif
#if SDK_WEIXIN //微信启动网络加载太慢了
            Scene scene = EditorSceneManager.OpenScene($"Assets/Scenes/{BuildAppInfo.mainScene}");
            GameObject appStartGo = GameObject.Find("AppStart");
            AppStartParams appStartParams = appStartGo.GetComponent<AppStartParams>();
            if (appStartParams)
            {
                appStartParams.appStartParams = new AppStartParams.AppStartParam[jsonObject.Count];
                for (int i = 0; i < jsonObject.keys.Count; i++)
                {
                    string key = jsonObject.keys[i];
                    string value = jsonObject[key].str;
                    appStartParams.appStartParams[i] = new AppStartParams.AppStartParam()
                    {
                        key = key,
                        value = value
                    };
                }
            }

            EditorSceneManager.SaveScene(scene);
#else
            File.WriteAllText(cfgJsonPath, jsonObject.ToString());
#endif
        }

        private static void startBuildApk()
        {
            //加密：打包apk前处理
            // BuildABEncryptUtil.setEncryptEnable(BuildAppInfo.encryptEnable);

            //需要延迟执行的方法体...
            //            EditorApplication.Step();
            Debug.Log("指定编译目标为：" + EditorUserBuildSettings.activeBuildTarget);

            string[] buildScenes = { $"Assets/Scenes/{BuildAppInfo.mainScene}" };
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    BuildInWin(buildScenes);
                    break;
                case BuildTarget.Android:
                    BuildInAndroid(buildScenes);
                    break;
                case BuildTarget.iOS:
                    BuildInIOS(buildScenes);
                    break;
                case BuildTarget.WebGL:
                    // BuildInWebGl(buildScenes);
                    BuildInWxMiniGame(buildScenes);
                    break;
#if SDK_WEIXIN
                // case BuildTarget.WeixinMiniGame:
                //     BuildInWxMiniGame(buildScenes);
                //     break;
#endif
            }

            //加密：打包apk后处理
            // BuildABEncryptUtil.restoreEncryptEnable();
        }

        private static void BuildInAndroid(string[] buildScenes)
        {
            var path = string.Concat(BuildAppInfo.resPath, "/android/", _apkFileName, ".apk");
            BuildPipeline.BuildPlayer(buildScenes, path, BuildTarget.Android, _buildOptions);
            Debug.Log("输出路径：" + path);
            //打开文件夹
            OpenFolder(BuildAppInfo.resPath + "/android");
        }

        private static void BuildInWin(string[] buildScenes)
        {
            var path = string.Format("{0}/{1}/{2}/{3}.exe", BuildAppInfo.resPath, "window", _apkFileName, _apkFileName);
            BuildPipeline.BuildPlayer(buildScenes, path, BuildTarget.StandaloneWindows64, _buildOptions);
            Debug.Log("输出路径：" + path);
            //打开文件夹
            OpenFolder(BuildAppInfo.resPath + "/window");
        }

        private static void BuildInIOS(string[] buildScenes)
        {
            var path = string.Format("{0}/{1}/{2}", BuildAppInfo.resPath, "ios", "Xcode");
            if (BuildPipeline.BuildCanBeAppended(BuildTarget.iOS, path) == CanAppendBuild.Yes)
                _buildOptions |= BuildOptions.AcceptExternalModificationsToPlayer;
            BuildPipeline.BuildPlayer(buildScenes, path, BuildTarget.iOS, _buildOptions); //append模式
            //BuildPipeline.BuildPlayer(buildScenes, path, BuildTarget.iOS, _buildOptions);//rebuild模式
            Debug.Log("输出路径：" + path);
            //IOS系统下的打开文件夹
            OpenFolder(BuildAppInfo.resPath + "/ios");
        }

        private static void BuildInWebGl(string[] buildScenes)
        {
            var path = string.Format("{0}/{1}/{2}", BuildAppInfo.resPath, "web", _apkFileName);
            //BuildPipeline.BuildPlayer(buildScens, path, BuildTarget.iOS, _buildOptions | BuildOptions.AcceptExternalModificationsToPlayer);//append模式
            BuildPipeline.BuildPlayer(buildScenes, path, BuildTarget.WebGL, _buildOptions); //rebuild模式
            Debug.Log("输出路径：" + path);
            //IOS系统下的打开文件夹
            OpenFolder(BuildAppInfo.resPath + "/web");
        }

#if SDK_WEIXIN
        private static void BuildInWxMiniGame(string[] buildScenes)
        {
            // JQFileUtil.DeleteDirectory($"{config.ProjectConf.DST}");
            config.ProjectConf.DST =
                $"{BuildAppInfo.resPath}/web/{BuildAppInfo.ProductNick}/{BuildAppInfo.sysCfgType}/{BuildAppInfo.version}";
            config.ProjectConf.projectName = BuildAppInfo.ProductName;
            List<EditorBuildSettingsScene> sceneList = new List<EditorBuildSettingsScene>();
            foreach (var scenePath in buildScenes)
            {
                sceneList.Add(new EditorBuildSettingsScene(scenePath, true));
            }

            EditorBuildSettings.scenes = sceneList.ToArray();
            if (WXConvertCore.DoExport() == WXConvertCore.WXExportError.SUCCEED)
            {
                Debug.Log("转换完成");
            }
            else
            {
                Debug.LogError("转换失败");
            }
        }

        public static void uploadCfgToCDN(Action endAction)
        {
            List<OssUploadBean> ossUploadBeans = GetCfgPackage();
            Debug.Log("上传文件：" + ossUploadBeans.Count);
            OssUploadTask ossUploadTask = new OssUploadTask(ossUploadBeans);
            ossUploadTask.Run(endAction);
        }

        private static List<OssUploadBean> GetCfgPackage()
        {
            var files = new List<string>();
            string sourcePath = BuildAppInfo.LocalCfgFloder.Replace("\\", "/");
            JQFileUtil.getAllFile(ref files, sourcePath, uploadFilter, true);
            List<OssUploadBean> ossUploadBeans = new List<OssUploadBean>();

            for (int i = 0; i < files.Count; i++)
            {
                string fileFullPath = files[i].Replace("\\", "/");
                string fileSubPath = fileFullPath.Replace($"{sourcePath}/", "");
                fileSubPath = "StreamingAssets/cfg/" + fileSubPath;
                (string ossPath, string localPath) = getOssPackagePath(sourcePath, fileSubPath);
                localPath = files[i];
                Debug.Log(
                    $"fileFullPath：{fileFullPath} \nfileSubPath：{fileSubPath} \nossPath：{ossPath} \nlocalPath：{localPath}");
                FileInfo fileInfo = new FileInfo(localPath);
                OssUploadBean ossUploadBean = new OssUploadBean(fileInfo.Length, ossPath, localPath);
                ossUploadBeans.Add(ossUploadBean);
            }

            return ossUploadBeans;
        }


        public static void uploadToCDN(Action endAction)
        {
            if (BuildAppInfo.useLocalCDN)
            {
                uploadToLocalCDNFloader(endAction);
            }
            else
            {
                uploadToWebCDN(endAction);
            }
        }

        private static bool isWebBinFile(string path)
        {
            return path.EndsWith(".bin.txt") || path.EndsWith(".bin.br");
        }

        private static void uploadToLocalCDNFloader(Action endAction)
        {
            string buildPath = $"{config.ProjectConf.DST}/webgl";
            string webPath = $"{BuildAppInfo.LocalCDNFloder}/{BuildAppInfo.ProductNick}";
            JQFileUtil.DeleteDirectory(webPath);
            JQFileUtil.CopyDirectory(buildPath, webPath);
            // var files = new List<string>();
            // JQFileUtil.getAllFile(ref files, $"{config.ProjectConf.DST}/webgl", isWebBinFile, false);
            // foreach (string webBinFilePath in files)
            // {
            //     string webBinFileName = JQFileUtil.getCurrFolderOrFileName(webBinFilePath);
            //     string fromPath = $"{BuildAppInfo.LocalCDNFloder}/{BuildAppInfo.ProductNick}/{webBinFileName}";
            //     string destPath = $"{BuildAppInfo.LocalCDNFloder}/{BuildAppInfo.ProductNick}/CUS/{webBinFileName}";
            //     JQFileUtil.CopyFile(fromPath, destPath);
            // }
            Debug.Log($"转换完成，上传到本地CDN:{buildPath} => {webPath}");
            endAction();
        }

        private static bool uploadFilter(string path)
        {
            if (path.Contains("Build")) return false;
            return true;
        }

        private static List<OssUploadBean> GetAllPackage()
        {
            var files = new List<string>();
            JQFileUtil.getAllFile(ref files, $"{config.ProjectConf.DST}/webgl", uploadFilter, true);
            List<OssUploadBean> ossUploadBeans = new List<OssUploadBean>();

            for (int i = 0; i < files.Count; i++)
            {
                string fileFullPath = files[i].Replace("\\", "/");
                string fileSubPath = fileFullPath.Replace($"{config.ProjectConf.DST}/webgl/", "");
                (string ossPath, string localPath) = getOssPackagePath($"{config.ProjectConf.DST}/webgl", fileSubPath);
                Debug.Log(
                    $"fileFullPath：{fileFullPath} \nfileSubPath：{fileSubPath} \nossPath：{ossPath} \nlocalPath：{localPath}");
                FileInfo fileInfo = new FileInfo(localPath);
                OssUploadBean ossUploadBean = new OssUploadBean(fileInfo.Length, ossPath, localPath);
                ossUploadBeans.Add(ossUploadBean);
            }

            return ossUploadBeans;
        }

        private static (string, string) getOssPackagePath(string packageOutputDirectory, string fileName)
        {
            string sysCfgFileName = BuildAppInfo.sysCfgType.ToString();
            string ossPath = $"{sysCfgFileName}/CDN/{PathUtil.platformName}/v{BuildAppInfo.version}/{fileName}";
            // if (fileName.EndsWith(".bin.txt"))
            // {
            //     ossPath = $"{sysCfgFileName}/CDN/{PathUtil.platformName}/v{BuildAppInfo.version}/CUS/{fileName}";
            // }

            string localPath = $"{packageOutputDirectory}/{fileName}";
            return (ossPath, localPath);
        }

        private static void uploadToWebCDN(Action endAction)
        {
            //auto streaming 未开启
            if (!WXConvertCore.IsInstantGameAutoStreaming())
            {
                List<OssUploadBean> ossUploadBeans = GetAllPackage();
                Debug.Log("上传文件：" + ossUploadBeans.Count);
                OssUploadTask ossUploadTask = new OssUploadTask(ossUploadBeans);
                ossUploadTask.Run(endAction);
            }
            else
            {
#if (UNITY_WEBGL || SDK_WEIXIN)
                // 上传首包资源
                if (!string.IsNullOrEmpty(WXConvertCore.FirstBundlePath) && File.Exists(WXConvertCore.FirstBundlePath))
                {
                    // if (Unity.InstantGame.IGBuildPipeline.UploadWeChatDataFile(WXConvertCore.FirstBundlePath))
                    if (true)
                    {
                        Debug.Log("转换完成并成功上传首包资源");
                    }
                    else
                    {
                        Debug.LogError("首包资源上传失败，请检查网络以及Auto Streaming配置是否正确。");
                    }
                }
                else
                {
                    Debug.LogError("转换失败");
                }
#else
                Debug.Log($"转换完成");
#endif
                endAction();
            }
        }
#endif


        private static void ChangeSysCfg()
        {
            var sysCfgFileName = BuildAppInfo.sysCfgType.ToString();
            var cfgFromDir = $"{Application.dataPath}/{PathUtil.RES_FOLDER}/Build/sysCfgs/syscfg-{sysCfgFileName}.json";
            var cfgTargetDir = Application.streamingAssetsPath + "/syscfg.json";
            Debug.Log("复制cfg文件：" + cfgFromDir + " => " + cfgTargetDir);
            File.Copy(cfgFromDir, cfgTargetDir, true);
            AssetDatabase.Refresh();
            Debug.Log("Done!");
        }


        private static void OpenFolder(string path)
        {
            path = path.Replace("/", "\\");
#if UNITY_EDITOR_WIN
            Process.Start("explorer.exe", path);
#else
                System.Diagnostics.Process.Start("open", path);
#endif
        }

        //        private static void removeOtherPlatform()
        //        {
        //            return;
        //#if !UNITY_ANDROID
        //            FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath + "/Audio/GeneratedSoundBanks/Android"); 
        //#endif
        //#if !UNITY_IPHONE
        //            FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath + "/Audio/GeneratedSoundBanks/iOS");
        //#endif
        //#if !UNITY_STANDALONE_OSX
        //            FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath + "/Audio/GeneratedSoundBanks/Mac");
        //#endif
        //#if !UNITY_STANDALONE_WIN
        //            FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath + "/Audio/GeneratedSoundBanks/Windows");
        //#endif
        //        }
        public static void copyWxCloudFunction(bool isTo, Action exeEndAction)
        {
            string fromPath = BuildAppInfo.WxCloudFunctionFloder;
            string toPath =
                $"{BuildAppInfo.resPath}/web/{BuildAppInfo.ProductNick}/{BuildAppInfo.sysCfgType}/{BuildAppInfo.version}/minigame/cloudfunctions";
            Debug.Log($"fromPath:{fromPath} toPath:{toPath}");
            if (isTo)
            {
                JQFileUtil.CopyDirectory(fromPath, toPath);
            }
            else
            {
                JQFileUtil.CopyDirectory(toPath, fromPath);
            }
        }
    }
}