#if HybridCLR
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using JQCore.tHybirdCLR;
using JQCore.tRes;
using JQCore.tUtil;
using JQEditor.Build;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HybridCLR.Editor
{
    public static class HybridCLRAssetsCommand
    {
        public static string HybridCLRBuildCacheDir => Application.dataPath + "/HybridCLRBuildCache";

        public static string AssetBundleOutputDir => $"{HybridCLRBuildCacheDir}/AssetBundleOutput";

        public static string AssetBundleSourceDataTempDir => $"{HybridCLRBuildCacheDir}/AssetBundleSourceData";


        public static string GetAssetBundleOutputDirByTarget(BuildTarget target)
        {
            return $"{AssetBundleOutputDir}/{target}";
        }

        public static string GetAssetBundleTempDirByTarget(BuildTarget target)
        {
            return $"{AssetBundleSourceDataTempDir}/{target}";
        }

        public static string ToRelativeAssetPath(string s)
        {
            return s.Substring(s.IndexOf("Assets/"));
        }

        // /// <summary>
        // /// 将HotFix.dll和HotUpdatePrefab.prefab打入common包.
        // /// 将HotUpdateScene.unity打入scene包.
        // /// </summary>
        // /// <param name="tempDir"></param>
        // /// <param name="outputDir"></param>
        // /// <param name="target"></param>
        // private static void BuildAssetBundles(string tempDir, string outputDir, BuildTarget target)
        // {
        //     Directory.CreateDirectory(tempDir);
        //     Directory.CreateDirectory(outputDir);
        //     
        //     List<AssetBundleBuild> abs = new List<AssetBundleBuild>();
        //
        //     {
        //         var prefabAssets = new List<string>();
        //         string testPrefab = $"{Application.dataPath}/Prefabs/HotUpdatePrefab.prefab";
        //         prefabAssets.Add(testPrefab);
        //         AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        //         abs.Add(new AssetBundleBuild
        //         {
        //             assetBundleName = "prefabs",
        //             assetNames = prefabAssets.Select(s => ToRelativeAssetPath(s)).ToArray(),
        //         });
        //     }
        //
        //     BuildPipeline.BuildAssetBundles(outputDir, abs.ToArray(), BuildAssetBundleOptions.None, target);
        //     AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        // }

        // public static void BuildAssetBundleByTarget(BuildTarget target)
        // {
        //     BuildAssetBundles(GetAssetBundleTempDirByTarget(target), GetAssetBundleOutputDirByTarget(target), target);
        // }

        public static void CompileAotDll()
        {
            EditorUserBuildSettings.development = BuildAppInfo.isDevelop;
            PrebuildCommand.GenerateAll();
        }

        public static void CompileHotfixDll()
        {
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            // BuildAssetBundleByTarget(target);
            CompileDllCommand.CompileDll(target, BuildAppInfo.isDevelop);
        }

        [MenuItem("HybridCLR/Build/CopyAndBindAllDllToLib")]
        public static void CopyAndBindAllDllToLib()
        {
            CopyToCodeDir_AOT();
            CopyToCodeDir_HotUpdate();
            BindAllDllToLib();
        }

        private static void BindAllDllToLib()
        {
            AssetDatabase.Refresh();

            List<Object> dllObjs = new List<Object>();
            //收集AotDll
            string aotAssembliesDstDir = $"{PathUtil.RES_FOLDER}/{PathUtil.DLL_FOLDER}/{HybirdCLRUtils.AotDllDirName}";
            foreach (var dll in HybridCLRSettings.Instance.patchAOTAssemblies)
            {
                string dllBytesPath = $"Assets/{aotAssembliesDstDir}/{dll}.bytes";
                Object dllObj = AssetDatabase.LoadAssetAtPath(dllBytesPath, typeof(Object));
                dllObjs.Add(dllObj);
            }

            //收集HotfixDll
            string hotfixAssembliesDstDir = $"{PathUtil.RES_FOLDER}/{PathUtil.DLL_FOLDER}/{HybirdCLRUtils.HotfixDllDirName}";
            foreach (var dll in HybridCLRSettings.Instance.hotUpdateAssemblies)
            {
                string dllBytesPath = $"Assets/{hotfixAssembliesDstDir}/{dll}.bytes";
                Object dllObj = AssetDatabase.LoadAssetAtPath(dllBytesPath, typeof(Object));
                dllObjs.Add(dllObj);
            }

            //设置到CodeDllLib.prefab中
            string codeDllLibPath = $"Assets/{PathUtil.RES_FOLDER}/{PathUtil.DLL_FOLDER}/{PathUtil.CODE_DLL_LIB}";
            Object obj = AssetDatabase.LoadAssetAtPath(codeDllLibPath, typeof(Object));
            GameObject go = (GameObject)obj;
            AssetObjectLib assetObjectLib = go.GetComponent<AssetObjectLib>();
            assetObjectLib.assets = dllObjs.ToArray();
            PrefabUtility.SavePrefabAsset(go);
        }


        //[MenuItem("HybridCLR/Build/BuildAssetbundle")]
        // public static void BuildSceneAssetBundleActiveBuildTargetExcludeAOT()
        // {
        //     BuildAssetBundleByTarget(EditorUserBuildSettings.activeBuildTarget);
        // }

        /// <summary>
        /// 复制AOT到Code/AotDll目录下
        /// </summary>
        private static void CopyToCodeDir_AOT()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
            string aotAssembliesDstDir =
                $"{Application.dataPath}/{PathUtil.RES_FOLDER}/{PathUtil.DLL_FOLDER}/{HybirdCLRUtils.AotDllDirName}";

            Directory.Delete(aotAssembliesDstDir, true);
            Directory.CreateDirectory(aotAssembliesDstDir);

            foreach (var dll in HybridCLRSettings.Instance.patchAOTAssemblies)
            {
                string srcDllPath = $"{aotAssembliesSrcDir}/{dll}.dll";
                if (!File.Exists(srcDllPath))
                {
                    Debug.LogError($"添加dll:{srcDllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                    continue;
                }

                string dllBytesPath = $"{aotAssembliesDstDir}/{dll}.bytes";
                File.Copy(srcDllPath, dllBytesPath, true);
                Debug.Log($"[CopyToCodeDir_AOT]{srcDllPath} -> {dllBytesPath}");
            }
        }

        /// <summary>
        /// 拷贝HotUpdateDll到Code/HotfixDll目录下
        /// </summary>
        private static void CopyToCodeDir_HotUpdate()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;

            string hotfixDllSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
            string hotfixAssembliesDstDir =
                $"{Application.dataPath}/{PathUtil.RES_FOLDER}/{PathUtil.DLL_FOLDER}/{HybirdCLRUtils.HotfixDllDirName}";

            Directory.Delete(hotfixAssembliesDstDir, true);
            Directory.CreateDirectory(hotfixAssembliesDstDir);

            foreach (var dll in HybridCLRSettings.Instance.hotUpdateAssemblies)
            {
                string dllPath = $"{hotfixDllSrcDir}/{dll}.dll";
                if (!File.Exists(dllPath))
                {
                    Debug.LogError($"添加dll:{dllPath} 时发生错误,文件不存在。");
                    continue;
                }

                string dllBytesPath = $"{hotfixAssembliesDstDir}/{dll}.bytes";
                File.Copy(dllPath, dllBytesPath, true);
                Debug.Log($"[CopyToCodeDir_HotUpdate]{dllPath} -> {dllBytesPath}");
            }
        }

        // public static void CopyAssetBundlesToStreamingAssets(BuildTarget target)
        // {
        //     string streamingAssetPathDst = Application.streamingAssetsPath;
        //     Directory.CreateDirectory(streamingAssetPathDst);
        //     string outputDir = GetAssetBundleOutputDirByTarget(target);
        //     var abs = new string[] { "prefabs" };
        //     foreach (var ab in abs)
        //     {
        //         string srcAb = ToRelativeAssetPath($"{outputDir}/{ab}");
        //         string dstAb = ToRelativeAssetPath($"{streamingAssetPathDst}/{ab}");
        //         Debug.Log($"[CopyAssetBundlesToStreamingAssets] copy assetbundle {srcAb} -> {dstAb}");
        //         AssetDatabase.CopyAsset( srcAb, dstAb);
        //     }
        // }
    }
}
#endif