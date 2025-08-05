/*
 *	作者：JJQ\Administrator
 *	时间：2018-02-20 11:53:20
 */

using System;
using System.Collections.Generic;
using JQEditor.Other;
using UnityEditor;
using UnityEngine;

namespace JQEditor.Build
{
    public static class BuildApp
    {
        private static BuildBase[] _currBuilds;

        private static readonly BuildBase[] buildBaseNoShader =
        {
//            new BuildUI("UI批处理", "buildUI"),
            // new BuildAtlas("打图集Tag", "buildAtlasTag"),
            // new BuildClearAB("清除ABName", "BuildClearAB"),
            // new BuildResAB("设置资源ABName", "BuildRes"),
            // new BuildLua("复制Lua", "buildLua"),
            // new BuildShader("搜集Shader变体", "buildShader"),
#if !UNITY_STANDALONE_WIN
#if HYBRIDCLR
            new BuildHybridCLR_AOT("编译HybridCLR-AOT", "buildHybridCLR-AOT"),
            new BuildHybridCLR_Hotfix("编译HybridCLR-Hotfix", "buildHybridCLR-Hotfix"),
            new BuildHybridCLR_BindDll("编译HybridCLR-BindDll", "buildHybridCLR-BindDll"),
#endif
#endif
            new BuildAtlasPacker("打图集", "buildAtlas"),
            new BuildAssetBundle("打AssetBundle", "buildAssetBundleAdd", false),
            // new BuildEncryptAB("加密AB", "buildEncryptAB"),
            // new BuildMainSubPackage("主分包处理", "buildMainSubPackage"),
            // new BuildRename("Resources重命名-1","buildRename1",false),
            new BuildApk("Build Player", "buildApp"),
            // new BuildRename("Resources重命名-2","buildRename2", true),
#if UNITY_WEBGL
            new BuildUploadCdn("上传CDN", "buildUploadCdn")
#endif
        };
        
        private static readonly BuildBase[] buildBaseAll =
        {
//            new BuildUI("UI批处理", "buildUI"),
            // new BuildAtlas("打图集Tag", "buildAtlasTag"),
            // new BuildClearAB("清除ABName", "BuildClearAB"),
            // new BuildResAB("设置资源ABName", "BuildRes"),
            // new BuildLua("复制Lua", "buildLua"),
            new BuildShader("搜集Shader变体", "buildShader"),
#if !UNITY_STANDALONE_WIN
#if HYBRIDCLR
            new BuildHybridCLR_AOT("编译HybridCLR-AOT", "buildHybridCLR-AOT"),
            new BuildHybridCLR_Hotfix("编译HybridCLR-Hotfix", "buildHybridCLR-Hotfix"),
            new BuildHybridCLR_BindDll("编译HybridCLR-BindDll", "buildHybridCLR-BindDll"),
#endif
#endif
            new BuildAtlasPacker("打图集", "buildAtlas"),
            new BuildAssetBundle("打AssetBundle", "buildAssetBundleAdd", false),
            // new BuildEncryptAB("加密AB", "buildEncryptAB"),
            // new BuildMainSubPackage("主分包处理", "buildMainSubPackage"),
            // new BuildRename("Resources重命名-1","buildRename1",false),
            new BuildApk("Build Player", "buildApp"),
            // new BuildRename("Resources重命名-2","buildRename2", true),
#if UNITY_WEBGL
            new BuildUploadCdn("上传CDN", "buildUploadCdn")
#endif
        };

        private static readonly BuildBase[] buildBasePlayer =
        {
            // new BuildRename("Resources重命名-1","buildRename1",false),
            new BuildApk("Build Player", "buildApp"),
            // new BuildRename("Resources重命名-2","buildRename2", true),
#if UNITY_WEBGL
            new BuildUploadCdn("上传CDN", "buildUploadCdn")
#endif
        };

        private static readonly BuildBase[] buildBaseAssets =
        {
            // new BuildAtlas("打图集Tag", "buildAtlasTag"),
//            new BuildUI("UI批处理", "buildUI"),
            // new BuildClearAB("清除ABName", "BuildClearAB"),
            // new BuildResAB("设置资源ABName", "BuildRes"),
            // new BuildLua("复制Lua", "buildLua"),
            new BuildShader("搜集Shader变体", "buildShader"),
#if HYBRIDCLR
            new BuildHybridCLR_AOT("编译HybridCLR-AOT", "buildHybridCLR-AOT"),
            new BuildHybridCLR_Hotfix("编译HybridCLR-Hotfix", "buildHybridCLR-Hotfix"),
            new BuildHybridCLR_BindDll("编译HybridCLR-BindDll", "buildHybridCLR-BindDll"),
#endif
            new BuildAtlasPacker("打图集", "buildAtlas"),
            new BuildAssetBundle("打AssetBundle", "buildAssetBundleAdd", false)
        };


        private static readonly BuildBase[] buildAdditionRes =
        {
            new BuildUpdateSvn("SVN-Update", "updateSvn"),
            new BuildCopyAB("复制AB", "buildCopyAB")
        };

        private static int _currIndex;

        private static void stepGUI(BuildBase buildBase)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(buildBase.Name, GUILayout.Width(300f), GUILayout.ExpandWidth(true)))
                EditorApplication.delayCall += () => { buildBase.build(null); };

            EditorGUILayout.LabelField(buildBase.lastBuildCostTime, EditorStyle.rightStyle, GUILayout.MaxWidth(80f));
            EditorGUILayout.LabelField(buildBase.lastBuildTime, EditorStyle.rightStyle, GUILayout.MaxWidth(80f));
            GUILayout.EndHorizontal();
        }

        private static BuildBase[] filterBuildBase(BuildBase[] buildBaseArr, Type[] filterTypes)
        {
            if (filterTypes == null) filterTypes = new Type[0];
            List<Type> filterTypeList = new List<Type>(filterTypes);
            List<BuildBase> buildBaseList = new List<BuildBase>();
            foreach (BuildBase buildBase in buildBaseArr)
            {
                if (!BuildAppInfo.HybridCLR)
                {
#if HYBRIDCLR
                    if (buildBase is BuildHybridCLR_AOT) continue;
                    if (buildBase is BuildHybridCLR_Hotfix) continue;
                    if (buildBase is BuildHybridCLR_BindDll) continue;
#endif
                }
                Type type = buildBase.GetType();
                if(filterTypeList.Contains(type))continue;
                buildBaseList.Add(buildBase);
            }
            return buildBaseList.ToArray();
        }


        private static void BuildButton()
        {
            EditorGUILayout.LabelField("打包流程", EditorStyle.headGuiStyle);
            GUILayout.Space(3f);
            GUILayout.BeginVertical("Box");
            BuildBase[] allBuildBases = buildBaseAll;
            if (!BuildAppInfo.HybridCLR)
            {
                allBuildBases = filterBuildBase(allBuildBases, null);
            }

            for (var i = 0; i < allBuildBases.Length; i++) stepGUI(allBuildBases[i]);

            GUILayout.EndVertical();
            GUILayout.Space(5);
            if (GUILayout.Button("Build Assets"))
            {
                allBuildBases = filterBuildBase(buildBaseAssets, null);
                _currBuilds = allBuildBases;
                buildAll();
            }

            if (GUILayout.Button("Build Players"))
            {
                allBuildBases = filterBuildBase(buildBasePlayer, null);
                _currBuilds = allBuildBases;
                buildAll();
            }

            if (GUILayout.Button("Build All(no shader)"))
            {
                allBuildBases = filterBuildBase(buildBaseNoShader, null);
                _currBuilds = allBuildBases;
                buildAll();
            }
            
            if (GUILayout.Button("Build All"))
            {
                _currBuilds = allBuildBases;
                buildAll();
            }


            if (GUILayout.Button("Build All-升版本"))
            {
                _currBuilds = allBuildBases;
                BuildAppInfo.addVersion2();
                BuildAppInfo.addSubVersion();
                buildAll();
            }

            // GUILayout.Space(5);
            // if (GUILayout.Button("调用平台接口-buildAB "))
            // {
            //     Stopwatch stopwatch = new Stopwatch();
            //     stopwatch.Start();
            //     BuildAllUtil.buildAB(manifest =>
            //     {
            //         stopwatch.Stop();
            //         Debug.Log($"-----------打包完毕，耗时：{stopwatch.ElapsedMilliseconds/1000}s");
            //     }, true);
            // }

//            if (GUILayout.Button("调用平台接口-buildAll "))
//            {
//                Debug.Log("打包开始：" + Time.time);
//                BuildAllUtil.buildAll(manifest => { Debug.Log("-----------打包完毕：" + Time.time); }, true);
//            }
        }


        private static void BuildResButton()
        {
            GUILayout.Space(30f);
            EditorGUILayout.LabelField("增量打包流程", EditorStyle.headGuiStyle);
            GUILayout.Space(3f);
            GUILayout.BeginVertical("Box");
            for (var i = 0; i < buildAdditionRes.Length; i++) stepGUI(buildAdditionRes[i]);

            GUILayout.EndVertical();
        }

        public static void buildAll()
        {
            _currIndex = 0;
            next();
        }

        public static void next()
        {
            var len = _currBuilds.Length;
            if (_currIndex < len)
            {
                var buildBase = _currBuilds[_currIndex];
                _currIndex++;
                Debug.Log($"Step {_currIndex}----{buildBase.Name}...");
                EditorApplication.Step();
                EditorUtility.DisplayProgressBar(buildBase.Name, $"{_currIndex}/{len}", (float)_currIndex / len);
                EditorApplication.delayCall += () => { buildBase.build(next); };
            }
            else
            {
                EditorUtility.ClearProgressBar();
                Debug.Log("Finish All.");
            }
        }

        public static void OnGUI()
        {
            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            BuildButton();
            BuildResButton();
        }
    }
}