using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using JQCore.tUtil;
using JQCore.tRes;
using JQAppStart.tLuancher;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using YooAsset.Editor;
using FileUtil = UnityEditor.FileUtil;

namespace JQEditor.Check
{
    public static class CheckShaderTools
    {
        public static void GetAllShaderVariantCount(string name, Action endAction)
        {
            string assetFolderPath = $"Assets/{PathUtil.RES_FOLDER}/Shader";
            string dataSavePath = $"{Application.dataPath}/{PathUtil.RES_FOLDER}/Shader";

            Assembly asm = Assembly.GetAssembly(typeof(UnityEditor.SceneView));
            // Assembly asm = Assembly.LoadFile(@"D:\Unity\Unity2018.4.7f1\Editor\Data\Managed\UnityEditor.dll");
            System.Type t2 = asm.GetType("UnityEditor.ShaderUtil");
            MethodInfo method = t2.GetMethod("GetVariantCount", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            Debug.Log(Application.dataPath);
            string projectPath = Application.dataPath.Replace("Assets", "");
            Debug.Log(projectPath);
            assetFolderPath = assetFolderPath.Replace(projectPath, "");
            var shaderList = AssetDatabase.FindAssets("t:Shader", new[] { assetFolderPath });

            // var output = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
            string date = System.DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
            string pathF = string.Format("{0}/ShaderVariantCount{1}.txt", dataSavePath, date);
            FileStream fs = new FileStream(pathF, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

            EditorUtility.DisplayProgressBar("Shader统计文件", "正在写入统计文件中...", 0f);
            int ix = 0;
            sw.WriteLine("Shader 数量：" + shaderList.Length);
            sw.WriteLine("ShaderFile, VariantCount");
            int totalCount = 0;
            foreach (var i in shaderList)
            {
                EditorUtility.DisplayProgressBar("Shader统计文件", "正在写入统计文件中...", ix / shaderList.Length);
                var path = AssetDatabase.GUIDToAssetPath(i);
                Shader s = AssetDatabase.LoadAssetAtPath(path, typeof(Shader)) as Shader;
                var variantCount = method.Invoke(null, new System.Object[] { s, true });
                if (int.Parse(variantCount.ToString()) > 20)
                {
                    sw.WriteLine(path + "," + variantCount.ToString() + "!!!!!!!!!!!!!!!!!!!");
                }
                else
                {
                    sw.WriteLine(path + "," + variantCount.ToString());
                }

                totalCount += int.Parse(variantCount.ToString());
                ++ix;
            }

            sw.WriteLine("Shader Variant Total Amount: " + totalCount);
            EditorUtility.ClearProgressBar();
            sw.Close();
            fs.Close();
        }

        public static void checkAllAssetUsePackageMat(string name, Action endAction)
        {
            int progressValue = 0;
            List<string> allAssets = new List<string>(1000);

            // 获取所有打包的资源
            CollectResult collectResult = AssetBundleCollectorSettingData.Setting.GetPackageAssets(EBuildMode.DryRunBuild, PathUtil.YOOASSET_PACKAGE_NAME);

            List<string> allMaterial = new List<string>(1000);
            foreach (var assetInfo in collectResult.CollectAssets)
            {
                string[] depends = AssetDatabase.GetDependencies(assetInfo.AssetPath, true);
                foreach (var dependAsset in depends)
                {
                    if (allAssets.Contains(dependAsset) == false)
                    {
                        allAssets.Add(dependAsset);
                        System.Type assetType = AssetDatabase.GetMainAssetTypeAtPath(dependAsset);
                        if (assetType == typeof(UnityEngine.Material))
                        {
                            allMaterial.Add(dependAsset);
                            if (dependAsset.StartsWith("Packages"))
                            {
                                Debug.LogError("Material在Packages中：" + dependAsset + "  " + assetInfo.AssetPath);
                            }
                        }
                    }
                }

                YooAsset.Editor.EditorTools.DisplayProgressBar("获取所有打包资源", ++progressValue, collectResult.CollectAssets.Count);
            }

            YooAsset.Editor.EditorTools.ClearProgressBar();
        }


//         /// <summary>
//         /// 检查Prefab未注册的Shader ShaderManager.buildInShaders ShaderManager.HyShaders
//         /// </summary>
//         /// <param name="name"></param>
//         /// <param name="endAction"></param>
//         public static void checkShaders(string name, Action endAction)
//         {
//             List<string> shaderPathList = new List<string>();
//             CheckCommonTools.Search<GameObject>(name + "-Effect", $"Assets/{PathUtil.RES_FOLDER}/Effect", checkUnregistShader, shaderPathList, endAction);
//             CheckCommonTools.Search<GameObject>(name + "-Model", $"Assets/{PathUtil.RES_FOLDER}/Model", checkUnregistShader, shaderPathList, endAction);
//             CheckCommonTools.Search<GameObject>(name + "-UI", $"Assets/{PathUtil.RES_FOLDER}/UI", checkUnregistShader, shaderPathList, endAction);
//             CheckCommonTools.Search<GameObject>(name + "-Card", $"Assets/{PathUtil.RES_FOLDER}/Card", checkUnregistShader, shaderPathList, endAction);
//
//             if (shaderPathList.Count > 0)
//             {
//                 Debug.LogError("未注册的Shader");
//                 foreach (string shaderPath in shaderPathList)
//                 {
//                     Debug.LogError("   " + shaderPath);
//                 }
//             }
//         }
//         
//         public static bool checkUnregistShader(string assetPath, GameObject obj, object obj1)
//         {
//             List<string> shaderPathList = obj1 as List<string>;
//             Renderer[]  renderers = obj.GetComponentsInChildren<Renderer>();
//             for (int i = 0; i < renderers.Length; i++)
//             {
//                 Renderer renderer = renderers[i];
//                 for (int j = 0; j < renderer.sharedMaterials.Length; j++)
//                 {
//                     Material material = renderer.sharedMaterials[j];
//                     if (material!=null && !isShaderInConfig(material.shader))
//                     {
//                         string shaderPath = AssetDatabase.GetAssetPath(material.shader);
//                         if (!shaderPathList.Contains(shaderPath))
//                         {
// //                            Debug.LogError(material.shader.name + "   " + materialPath + "   " + assetPath + "   " + renderer.name);
//                             shaderPathList.Add(shaderPath + "  " + material.shader.name);
//                         }
//                     }
//                 }
//             }
//
//             return true;
//         }
//
//         private static bool isShaderInConfig(Shader shader)
//         {
//             string checkShaderName = shader.name;
//             if (ShaderManager.buildInShaders.Contains(checkShaderName))
//             {
//                 return true;
//             }
//             if (ShaderManager.HyShaders.Contains(checkShaderName))
//             {
//                 return true;
//             }
//             return false;
//         }

//获取shader中所有的宏
        public static bool GetShaderKeywords(Shader target, out string[] global, out string[] local)
        {
            try
            {
                MethodInfo globalKeywords = typeof(ShaderUtil).GetMethod("GetShaderGlobalKeywords", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                global = (string[])globalKeywords.Invoke(null, new object[] { target });
                MethodInfo localKeywords = typeof(ShaderUtil).GetMethod("GetShaderLocalKeywords", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                local = (string[])localKeywords.Invoke(null, new object[] { target });
                return true;
            }
            catch
            {
                global = local = null;
                return false;
            }
        }

        // static void CleanMatProperty(Material m)
        // {
        //     if (GetShaderKeywords(m.shader, out var global, out var local))
        //     {
        //         HashSet<string> keywords = new HashSet<string>();
        //         foreach (var g in global)
        //         {
        //             keywords.Add(g);
        //         }
        //
        //         foreach (var l in local)
        //         {
        //             keywords.Add(l);
        //         }
        //
        //         //重置keywords
        //         List<string> resetKeywords = new List<string>(m.shaderKeywords);
        //         foreach (var item in m.shaderKeywords)
        //         {
        //             if (!keywords.Contains(item))
        //                 resetKeywords.Remove(item);
        //         }
        //
        //         m.shaderKeywords = resetKeywords.ToArray();
        //     }
        //
        //     HashSet<string> property = new HashSet<string>();
        //     int count = m.shader.GetPropertyCount();
        //     for (int i = 0; i < count; i++)
        //     {
        //         property.Add(m.shader.GetPropertyName(i));
        //     }
        //
        //     SerializedObject o = new SerializedObject(m);
        //     SerializedProperty disabledShaderPasses = o.FindProperty("disabledShaderPasses");
        //     SerializedProperty SavedProperties = o.FindProperty("m_SavedProperties");
        //     SerializedProperty TexEnvs = SavedProperties.FindPropertyRelative("m_TexEnvs");
        //     SerializedProperty Floats = SavedProperties.FindPropertyRelative("m_Floats");
        //     SerializedProperty Colors = SavedProperties.FindPropertyRelative("m_Colors");
        //     //对比属性删除残留的属性
        //     for (int i = disabledShaderPasses.arraySize - 1; i >= 0; i--)
        //     {
        //         if (!property.Contains(disabledShaderPasses.GetArrayElementAtIndex(i).displayName))
        //         {
        //             disabledShaderPasses.DeleteArrayElementAtIndex(i);
        //         }
        //     }
        //
        //     for (int i = TexEnvs.arraySize - 1; i >= 0; i--)
        //     {
        //         if (!property.Contains(TexEnvs.GetArrayElementAtIndex(i).displayName))
        //         {
        //             TexEnvs.DeleteArrayElementAtIndex(i);
        //         }
        //     }
        //
        //     for (int i = Floats.arraySize - 1; i >= 0; i--)
        //     {
        //         if (!property.Contains(Floats.GetArrayElementAtIndex(i).displayName))
        //         {
        //             Floats.DeleteArrayElementAtIndex(i);
        //         }
        //     }
        //
        //     for (int i = Colors.arraySize - 1; i >= 0; i--)
        //     {
        //         if (!property.Contains(Colors.GetArrayElementAtIndex(i).displayName))
        //         {
        //             Colors.DeleteArrayElementAtIndex(i);
        //         }
        //     }
        //
        //     o.ApplyModifiedProperties();
        //
        //     Debug.Log("Done!:"+m);
        // }


        /// <summary>
        /// 将插件Shader替换为 项目内自写Shader， 自写Shader控制keyword， 复杂度、fallback等来提高性能
        /// 需先跑变体收集，对变体收集场景内的所有材质球进行处理即可，节省收集时间
        /// </summary>
        /// <param name="name"></param>
        /// <param name="endAction"></param>
        public static void replaceShaderFromPackageToProject(string name, Action endAction)
        {
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            foreach (GameObject rootGameObject in rootGameObjects)
            {
                Renderer[] rendererArr = rootGameObject.GetComponentsInChildren<Renderer>(true);
                foreach (var renderer in rendererArr)
                {
                    Material[] materials = renderer.sharedMaterials;
                    foreach (Material material in materials)
                    {
                        if (material == null) continue;
                        // CleanMatProperty(material);
                        string shaderName = material.shader.name;
                        string materialPath = AssetDatabase.GetAssetPath(material);
                        if (materialPath.StartsWith("Packages"))
                        {
                            string rootPath = PathUtil.getFullPath(renderer.gameObject);
                            Debug.LogError($"材质球在插件内，无法替换： {materialPath}  {rootPath}");
                            continue;
                        }

                        string assetPath = AssetDatabase.GetAssetPath(material.shader);
                        if (assetPath.StartsWith("Packages"))
                        {
                            string replaceShaderName;
                            if (shaderName.StartsWith("Hidden"))
                            {
                                replaceShaderName = shaderName.Replace("Hidden/", "Hidden/spi/");
                            }
                            else
                            {
                                replaceShaderName = $"spi/{shaderName}";
                            }

                            Shader replaceShader = Shader.Find(replaceShaderName);
                            if (replaceShader != null)
                            {
                                material.shader = replaceShader;
                                Debug.Log($"替换shader成功： {replaceShaderName}");
                            }
                            else
                            {
                                Debug.LogError($"查无替换shader，请编写好替换shader再试： {replaceShaderName}");
                            }
                        }
                    }
                }
            }
        }


        private static List<string> getCollectShaderFolders()
        {
            List<string> shaderPathList = new List<string>();
            List<AssetBundleCollectorGroup> ss = AssetBundleCollectorSettingData.Setting.Packages[0].Groups;
            foreach (AssetBundleCollectorGroup assetBundleCollectorGroup in ss)
            {
                if (assetBundleCollectorGroup.GroupName == "Shader")
                {
                    foreach (AssetBundleCollector assetBundleCollector in assetBundleCollectorGroup.Collectors)
                    {
                        if (AssetDatabase.IsValidFolder(assetBundleCollector.CollectPath))
                        {
                            shaderPathList.Add(assetBundleCollector.CollectPath);
                        }
                    }
                }
            }

            return shaderPathList;
        }

        private static bool isInCollectShaderFolder(List<string> shaderPathList, string asssetPath)
        {
            for (int i = 0; i < shaderPathList.Count; i++)
            {
                string folderPath = shaderPathList[i];
                if (asssetPath.StartsWith(folderPath))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 从变体收集器中获得所有shader，并检查其中是否有未注册的shader，
        /// 需先跑变体收集，从变体收集结果json中读取shader列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="endAction"></param>
        public static void checkShaderRegisterFromSVC(string name, Action endAction)
        {
            //检查ShaderVariantCollection中是否有未注册的shader
            List<string> allShader = new List<string>();
            allShader.AddRange(ShaderManager.buildInShaders);
            allShader.AddRange(ShaderManager.HyShaders);

            List<string> buildInShaders = new List<string>(ShaderManager.buildInShaders);

            string savePath = ShaderVariantCollectorSettingData.Setting.SavePath;
            List<string> collectShaderFolders = getCollectShaderFolders();

            string jsonPath = savePath.Replace("shadervariants", "json");
            string jsonStr = FileUtility.ReadAllText(jsonPath);
            ShaderVariantCollectionManifest shaderVariantCollectionManifest = JsonUtility.FromJson<ShaderVariantCollectionManifest>(jsonStr);
            foreach (ShaderVariantCollectionManifest.ShaderVariantInfo shaderVariantInfo in shaderVariantCollectionManifest.ShaderVariantInfos)
            {
                if (shaderVariantInfo.AssetPath.Equals("Resources/unity_builtin_extra") && !buildInShaders.Contains(shaderVariantInfo.ShaderName))
                {
                    Debug.LogError($"Hidden shader未注册： shader:{shaderVariantInfo.ShaderName}  shaderPath:{shaderVariantInfo.AssetPath}");
                    continue;
                }

                if (shaderVariantInfo.AssetPath.StartsWith("Packages"))
                {
                    Debug.LogError($"插件shader未转移到工程内部： shader:{shaderVariantInfo.ShaderName}  shaderPath:{shaderVariantInfo.AssetPath}");
                    continue;
                }

                if (!shaderVariantInfo.AssetPath.Equals("Resources/unity_builtin_extra") && !isInCollectShaderFolder(collectShaderFolders, shaderVariantInfo.AssetPath))
                {
                    Debug.LogError($"自定义shader未归集到资源打包Shader目录下： shader:{shaderVariantInfo.ShaderName}  shaderPath:{shaderVariantInfo.AssetPath}");
                    continue;
                }

                if (!allShader.Contains(shaderVariantInfo.ShaderName))
                {
                    Debug.LogError($"shader未注册： shader:{shaderVariantInfo.ShaderName}  shaderPath:{shaderVariantInfo.AssetPath}");
                    continue;
                }
            }

            //把ShaderManager.buildInShaders中的shader加入到GraphicsSettings的Buildin中
            SetGraphicsSettingsBuildinShader();
            Debug.Log("IncludeShaders Finish!");
            endAction?.Invoke();
        }


        private static void SetGraphicsSettingsBuildinShader()
        {
            List<Shader> shaderList = new List<Shader>();

            for (int i = 0; i < ShaderManager.buildInShaders.Length; i++)
            {
                string shaderName = ShaderManager.buildInShaders[i];
                Shader shader = Shader.Find(shaderName);
                if (shader == null)
                {
                    Debug.LogError("shader不存在：" + shaderName);
                    continue;
                }

                string path = AssetDatabase.GetAssetPath(shader);
                if (path.Equals("Resources/unity_builtin_extra"))
                {
                    shaderList.Add(shader);
                }
                else
                {
                    Debug.LogError("非内置shader,请配置到ShaderMananger.cs中：" + shaderName + "  " + path);
                }
            }

            Shader[] shaders = shaderList.ToArray();

            SerializedObject graphicsSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
            SerializedProperty it = graphicsSettings.GetIterator();
            SerializedProperty dataPoint;
            while (it.NextVisible(true))
            {
                if (it.name == "m_AlwaysIncludedShaders")
                {
                    it.ClearArray();

                    for (int i = 0; i < shaders.Length; i++)
                    {
                        it.InsertArrayElementAtIndex(i);
                        dataPoint = it.GetArrayElementAtIndex(i);
                        dataPoint.objectReferenceValue = shaders[i];
                    }

                    graphicsSettings.ApplyModifiedProperties();
                }
            }
        }
    }
}