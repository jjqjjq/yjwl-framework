/*----------------------------------------------------------------
// 文件名：BuildAssetBundleUtil.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/10/11 20:42:15
//----------------------------------------------------------------*/

using System;
using System.Diagnostics;
using System.IO;
using JQCore.tFileSystem;
using JQCore.tUtil;
using UnityEditor;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.SceneManagement;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;
using Debug = UnityEngine.Debug;

namespace JQEditor.Build
{
    public static class BuildAssetBundleUtil
    {
        public delegate string SetABName(string rootPath, string path);

//        private static string _folderName;
//        private static string _luaFloder;
        private static bool _showUI;

        public static readonly BuildAssetBundleOptions addOption =
            BuildAssetBundleOptions.DisableWriteTypeTree |
            BuildAssetBundleOptions.DeterministicAssetBundle |
            BuildAssetBundleOptions.ChunkBasedCompression |
            //BuildAssetBundleOptions.UncompressedAssetBundle | 
            //BuildAssetBundleOptions.DisableLoadAssetByFileName |
            //BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension |
            BuildAssetBundleOptions.StrictMode;


        public static readonly BuildAssetBundleOptions allOption =
            BuildAssetBundleOptions.DisableWriteTypeTree |
            BuildAssetBundleOptions.DeterministicAssetBundle |
            BuildAssetBundleOptions.ChunkBasedCompression |
            //BuildAssetBundleOptions.UncompressedAssetBundle | 
            //BuildAssetBundleOptions.DisableLoadAssetByFileName |
            //BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension |
            BuildAssetBundleOptions.StrictMode |
            BuildAssetBundleOptions.ForceRebuildAssetBundle;


        // public static void setLua(bool showUI, List<string> luaList, Action endAction = null)
        // {
        //     _showUI = showUI;
        //
        //     Console.WriteLine($"buildRes-setLua-start:    {DateTime.Now.ToString()}");
        //     //            Stopwatch stopwatchAll = new Stopwatch();
        //     //            stopwatchAll.Start();
        //     settingOne(PathUtil.LUA_FOLDER, setBaseLua, isLua, luaList, false);
        //     settingOne($"{PathUtil.LUA_FOLDER}/Data", setRootPath, isLua, luaList);
        //     settingOne($"{PathUtil.LUA_FOLDER}/framework", setRootPath, isLua, luaList);
        //     settingOne($"{PathUtil.LUA_FOLDER}/global", setRootPath, isLua, luaList);
        //     settingSubFolder($"{PathUtil.LUA_FOLDER}/Module", setRootPath, isLua, luaList);
        //     settingOne($"{PathUtil.LUA_FOLDER}/Proto", setRootPath, isLua, luaList);
        //     settingOne($"{PathUtil.LUA_FOLDER}/xlua", setRootPath, isLua, luaList);
        //     settingOne($"{PathUtil.LUA_FOLDER}/Hotfix", setRootPath, isLua, luaList);
        //     //            stopwatchAll.Stop();
        //     //            Console.WriteLine($"setLua:{stopwatchAll.ElapsedMilliseconds}ms");
        //
        //     Console.WriteLine($"buildRes-setLua-end:    {DateTime.Now.ToString()}");
        //     if (endAction != null)
        //     {
        //         endAction();
        //     }
        // }
        //
        // public static void settingSubFolder(string rootPath, SetABName settingAction, Predicate<string> filterFun, List<string> luaList = null, bool depth = true)
        // {
        //     DirectoryInfo dir = new DirectoryInfo($"Assets/{PathUtil.RES_FOLDER}/" + rootPath);
        //     foreach (DirectoryInfo info in dir.GetDirectories())
        //     {
        //         settingOne(rootPath + "/" + info.Name, settingAction, filterFun, luaList, depth);
        //     }
        // }
        //
        // public static void settingOne(string rootPath, SetABName settingAction, Predicate<string> filterFun, List<string> luaList = null, bool depth = true)
        // {
        //     List<string> pathList = new List<string>();
        //     FileUtil.getAllFile(ref pathList, $"Assets/{PathUtil.RES_FOLDER}/" + rootPath, filterFun, depth);
        //     for (int i = 0; i < pathList.Count; i++)
        //     {
        //         if (_showUI)
        //         {
        //             EditorUtility.DisplayProgressBar($"设置AB名称({rootPath})...", $"{i}/{pathList.Count}", (float)i / pathList.Count);
        //         }
        //
        //         string path = pathList[i];
        //         string abName = settingAction(rootPath, path);
        //         if (!string.IsNullOrEmpty(abName))
        //         {
        //             setABNameByAssetDatabase(path, abName);
        //         }
        //
        //         if (!string.IsNullOrEmpty(abName))
        //         {
        //             if (luaList != null && !luaList.Contains(abName))
        //             {
        //                 luaList.Add(abName);
        //             }
        //         }
        //     }
        //
        //     if (_showUI)
        //     {
        //         EditorUtility.ClearProgressBar();
        //         AssetDatabase.SaveAssets();
        //     }
        // }


        private static void setABNameByAssetDatabase(string path, string abName)
        {
            var assetImporter = AssetImporter.GetAtPath(path);
            if (assetImporter == null) Console.WriteLine("[Error]assetImporter is null:" + path);

            if (assetImporter.assetBundleName != abName) assetImporter.assetBundleName = abName;
        }

        private static string removePathHead(string path)
        {
            return path.Replace("\\", "/").Replace($"Assets/{PathUtil.RES_FOLDER}/", "");
        }


        public static AssetBundleManifest buildAssetBundleAll()
        {
            var stopwatchAll = new Stopwatch();
            stopwatchAll.Start();
            JQFileUtil.ClearFolder("Assets/StreamingAssets/resource");
            JQFileUtil.CreateDirectory("Assets/StreamingAssets/resource");
            var assetBundleManifest = BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/resource", allOption, EditorUserBuildSettings.activeBuildTarget);
            Debug.Log("all build end");
            stopwatchAll.Stop();
            Console.WriteLine($"buildAssetBundleAll:{stopwatchAll.ElapsedMilliseconds}ms");
            return assetBundleManifest;
        }

        public static AssetBundleManifest buildAssetBundleAdd()
        {
            Console.WriteLine($"buildRes-buildAssetBundleAdd-start:    {DateTime.Now.ToString()}");
            //            Stopwatch stopwatchAll = new Stopwatch();
            //            stopwatchAll.Start();
            //        FileUtil.ClearDirectory("Assets/StreamingAssets/resource");
            JQFileUtil.CreateDirectory("Assets/StreamingAssets/resource");
            var main = BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/resource", addOption, EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();
            Debug.Log("add build end");
            //            stopwatchAll.Stop();
            //            Console.WriteLine($"buildAssetBundleAdd:{stopwatchAll.ElapsedMilliseconds}ms");

            Console.WriteLine($"buildRes-buildAssetBundleAdd-end:    {DateTime.Now.ToString()}");
            return main;
        }

        #region 过滤规则

        private static bool isPic(string assetPath)
        {
            return assetPath.EndsWith(".jpg") || assetPath.EndsWith(".png");
        }

        private static bool isLua(string assetPath)
        {
            return assetPath.EndsWith(".bytes");
        }

        private static bool isPrefab(string assetPath)
        {
            return assetPath.EndsWith(".prefab");
        }

        private static bool isScene(string assetPath)
        {
            return assetPath.EndsWith(".unity");
        }

        private static bool isJson(string assetPath)
        {
            return assetPath.EndsWith(".json");
        }

        private static bool isCommonAnima(string assetPath)
        {
            if (assetPath.EndsWith(".anim")) return assetPath.Contains("model_34001_") || assetPath.Contains("model_34002_");

            return false;
        }

        private static bool needClearHandle(string assetPath)
        {
            if (assetPath.EndsWith(".cs")) return false;

            if (assetPath.Contains(PathUtil.LUA_FOLDER)) return false;

            return true;
        }

        private static bool needHandle(string assetPath)
        {
            if (assetPath.EndsWith(".cs")) return false;

            if (assetPath.Contains("Temp")) return false;

            return true;
        }

        #endregion


        #region 各类规则

        private static string setBaseLua(string rootPath, string path)
        {
            var abName = $"{PathUtil.LUA_FOLDER}/baseLua.ab";
            return abName;
        }

        private static string setRootPath(string rootPath, string path)
        {
            var abName = rootPath + ".ab";
            return abName;
        }

        private static string setCommonPrefab(string rootPath, string path)
        {
            if (!path.EndsWith("prefab")) return null;
            var shortPath = removePathHead(path);
            var abName = shortPath + ".ab";
            return abName;
        }

        private static string setSound(string rootPath, string path)
        {
            return "Sound/Sound.ab";
        }

        private static string setSelf(string rootPath, string path)
        {
            var shortPath = removePathHead(path);
            var abName = shortPath + ".ab";
            return abName;
        }

        private static string setMain(string rootPath, string path)
        {
            return "ui/main.ab";
        }


        private static string setFolder(string rootPath, string path)
        {
            var shortPath = removePathHead(path);
            var abName = JQFileUtil.getParentFolderPath(shortPath) + ".ab";
            return abName;
        }

        private static string setCommonModelAnima(string rootPath, string path)
        {
            return "Common/AnimaLib.ab";
        }

        private static string setPrefabName(string rootPath, string path)
        {
            var shortPath = removePathHead(path);
            var abName = shortPath + ".ab";
            return abName;
        }


        private static string setTexturesDeepth2FolderName(string rootPath, string path)
        {
            var isPic = path.EndsWith(".jpg") || !path.EndsWith(".png");
            if (!isPic) return null;
            var shortPath = removePathHead(path);
            for (var i = 0; i < 2; i++) shortPath = JQFileUtil.getParentFolderPath(shortPath);

            return shortPath + "_atlas.ab";
        }

        private static string setDeepthFolder1(string rootPath, string path)
        {
            if (!path.EndsWith(".prefab")) return null;
            var shortPath = removePathHead(path);
            for (var i = 0; i < 1; i++) shortPath = JQFileUtil.getParentFolderPath(shortPath);

            return shortPath + ".ab";
        }

        private static string setDeepthFolder2(string rootPath, string path)
        {
            if (!path.EndsWith(".prefab")) return null;
            var shortPath = removePathHead(path);
            for (var i = 0; i < 2; i++) shortPath = JQFileUtil.getParentFolderPath(shortPath);

            return shortPath + ".ab";
        }

        private static string setDeepthFolder3(string rootPath, string path)
        {
            if (!path.EndsWith(".prefab")) return null;
            var shortPath = removePathHead(path);
            for (var i = 0; i < 3; i++) shortPath = JQFileUtil.getParentFolderPath(shortPath);

            return shortPath + ".ab";
        }

        #endregion


        #region YooAsset

        public static void BuildAssetBundleAll()
        {
            Console.WriteLine($"buildRes-BuildAssetBundleAll-start:    {DateTime.Now.ToString()}");
            ClearOutputCacheFolder();
            Build(true);
            Console.WriteLine($"buildRes-BuildAssetBundleAll-end:    {DateTime.Now.ToString()}");
        }

        public static void BuildAssetBundleAdd()
        {
            Console.WriteLine($"buildRes-BuildAssetBundleAdd-start:    {DateTime.Now.ToString()}");
            Build(false);
            Console.WriteLine($"buildRes-BuildAssetBundleAdd-end:    {DateTime.Now.ToString()}");
        }

        private static void ClearOutputCacheFolder()
        {
            string outputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            var pipelineOutputDirectory = $"{outputRoot}/{EditorUserBuildSettings.activeBuildTarget}/{PathUtil.YOOASSET_PACKAGE_NAME}/{YooAssetSettings.OutputFolderName}";
            EditorTools.ClearFolder(pipelineOutputDirectory);
            EditorTools.ClearFolder($"{Application.dataPath}/../Library/BuildCache");
            BuildCache.PurgeCache(false);
        }

        private static void Build(bool forceRebuild)
        {
            BuildAppInfo.updatePackageVersion();
            var useSBP = BuildAppInfo.UseSBP;

            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            Debug.Log($"开始构建 : {buildTarget}");

            // 构建参数
            string defaultOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            var buildParameters = new BuildParameters();
            
            buildParameters.StreamingAssetsRoot = AssetBundleBuilderHelper.GetDefaultStreamingAssetsRoot();
            buildParameters.BuildOutputRoot = defaultOutputRoot;
            buildParameters.BuildTarget = buildTarget;
            //先用BuildIn的，SBPYooAsset暂时不支持ForceReBuild   实际SBP是可以的 IBuildParameters.UseCache = false;

            if (useSBP)
            {
                buildParameters.BuildPipeline = EBuildPipeline.ScriptableBuildPipeline;
                buildParameters.BuildMode = EBuildMode.IncrementalBuild;
            }
            else
            {
                buildParameters.BuildPipeline = EBuildPipeline.BuiltinBuildPipeline;
                buildParameters.BuildMode = forceRebuild ? EBuildMode.ForceRebuild : EBuildMode.IncrementalBuild;
            }


            buildParameters.PackageName = PathUtil.YOOASSET_PACKAGE_NAME;
            buildParameters.PackageVersion = BuildAppInfo.PackageVersion;
            buildParameters.VerifyBuildingResult = true;
            buildParameters.SharedPackRule = new ZeroRedundancySharedPackRule();
#if !UNITY_WEBGL
            buildParameters.EncryptionServices = new CustomFileOffsetEncryption();
#endif
            buildParameters.CompressOption = ECompressOption.LZ4;
            buildParameters.OutputNameStyle = EOutputNameStyle.BundleName_HashName;
            buildParameters.CopyBuildinFileOption = ECopyBuildinFileOption.None; //什么都不做，我自己来
            buildParameters.CopyBuildinFileTags = string.Empty;

            //SBP
            if (useSBP)
            {
                buildParameters.SBPParameters = new BuildParameters.SBPBuildParameters();
                buildParameters.SBPParameters.WriteLinkXML = true;
            }

            // 执行构建
            string path = GetPackageOutputDirectory(buildParameters);
            Debug.Log("path:"+path);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            var builder = new AssetBundleBuilder();
            
            var buildResult = builder.Run(buildParameters);
            if (buildResult.Success)
                Debug.Log($"构建成功 : {buildResult.OutputPackageDirectory}");
            else
                Debug.LogError($"构建失败 : {buildResult.ErrorInfo}");
        }
        
        /// <summary>
        /// 获取本次构建的补丁输出目录
        /// </summary>
        private static string GetPackageOutputDirectory(BuildParameters parameters)
        {
            return $"{parameters.BuildOutputRoot}/{parameters.BuildTarget}/{parameters.PackageName}/{parameters.PackageVersion}";
        }

// 从构建命令里获取参数示例
        // private static string GetBuildPackageName()
        // {
        //     foreach (string arg in System.Environment.GetCommandLineArgs())
        //     {
        //         if (arg.StartsWith("buildPackage"))
        //             return arg.Split("="[0])[1];
        //     }
        //     return string.Empty;
        // }


        public static void CollectSVC(Action completedCallback)
        {
            var savePath = ShaderVariantCollectorSettingData.Setting.SavePath;
            var packageName = ShaderVariantCollectorSettingData.Setting.CollectPackage;
            ShaderVariantCollector.Run(savePath, packageName, 1000, ()=>
            {
                completedCallback?.Invoke();
            });
        }

        #endregion
    }
}