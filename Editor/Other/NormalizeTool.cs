using System.Collections.Generic;
using JQCore.tFileSystem;
using JQCore.tLog;
using JQCore.tUtil;
using JQEditor.Tool;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JQEditor.Other
{
    public class NormalizeTool
    {
        // private static NormalizeAssetTypeCtrl _effectCtrl = null;
        // private static NormalizeAssetTypeCtrl _itemCtrl = null;
        // private static NormalizeAssetTypeCtrl _monsterCtrl = null;

        //引用资源重命名并归纳到类型文件夹中
//        [MenuItem("Assets/自动格式化/方案B-Model")]
        private static void RenameAndMove()
        {
            ClassifyFileB($"Assets/{PathUtil.RES_FOLDER}/Model");
        }

        private static int GetModelIdFromFileName(string fileName)
        {
            if (fileName.Contains("Role_"))
            {
                fileName = fileName.Replace("Role_", "");
            }

            if (fileName.Contains("Npc_"))
            {
                fileName = fileName.Replace("Npc_", "");
            }

            if (fileName.Contains("Weapon_"))
            {
                fileName = fileName.Replace("Weapon_", "");
            }

            int modelId = 0;
            bool success = int.TryParse(fileName, out modelId);
            if (!success)
            {
                Debug.LogError("文件名不是数字：" + fileName);
            }

            return modelId;
        }

        private static void moveAsset(Dictionary<string, int> typeCountDic, string typeFolder, string assetPath, int modelId, string assetFileName, string assetExtension)
        {
            int count = 0;
            if (!typeCountDic.TryGetValue(assetExtension, out count))
            {
                typeCountDic[assetExtension] = 0;
            }
            else
            {
                count++;
                typeCountDic[assetExtension] = count;
            }

            string toPath = null;
            if (string.IsNullOrEmpty(typeFolder))
            {
                toPath = $"Assets/{PathUtil.RES_FOLDER}/Model/model_{modelId}.{assetExtension}";
            }
            else
            {
                toPath = $"Assets/{PathUtil.RES_FOLDER}/Model/{typeFolder}/model_{modelId}_{assetFileName}.{assetExtension.ToLower()}";
            }

            Debug.Log($"move file: from={assetPath}     to={toPath}");
            string error = AssetDatabase.MoveAsset(assetPath, toPath);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError(error);
            }
        }

        private static void ClassifyFileB(string rootPath)
        {
            //创建目录
            List<string> targetPathList = new List<string>(new[] { "Material", "Texture", "Anim", "Fbx" });
            foreach (string folder in targetPathList)
            {
                JQFileUtil.CreateDirectory(rootPath + "/" + folder);
            }

            AssetDatabase.Refresh();

            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new string[] { rootPath });
            for (int i = 0; i < prefabGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
                if (path.Contains("Temp")) continue;
                if (path.Contains("Card")) continue;
                string[] dependencies = AssetDatabase.GetDependencies(path); //获取每个预制体所用的的图片的路径
                string fileName = JQFileUtil.getCurrFolderOrFileName(path, false);
                int modelId = GetModelIdFromFileName(fileName);
                if (modelId == 0) continue;
                Debug.Log("modelId=====:" + modelId);
                Dictionary<string, int> typeCountDic = new Dictionary<string, int>();
                foreach (string dependency in dependencies)
                {
                    Object asset = AssetDatabase.LoadAssetAtPath<Object>(dependency);
                    string assetPath = dependency.ToLower();
                    if (assetPath.EndsWith("cs") || assetPath.EndsWith("shader")) continue;
                    if (!assetPath.StartsWith(rootPath.ToLower()))
                    {
                        Debug.LogError("跨目录引用！：" + assetPath + "             " + path);
                        continue;
                    }

                    string assetFileName = JQFileUtil.getCurrFolderOrFileName(assetPath, false);
                    string assetExtension = JQFileUtil.getExtension(assetPath);
                    if (asset is Texture2D) //贴图
                    {
                        //                        Debug.Log("贴图："+ assetPath);
                        moveAsset(typeCountDic, "Texture", assetPath, modelId, assetFileName, assetExtension);
                    }
                    else if (assetPath.EndsWith("anim") || assetPath.EndsWith(".controller")) //动画
                    {
                        //                        Debug.Log("Anim：" + assetPath);
                        moveAsset(typeCountDic, "Anim", assetPath, modelId, assetFileName, assetExtension);
                    }
                    else if (asset is Material) //anim  controller
                    {
                        //                        Debug.Log("Material：" + assetPath);
                        moveAsset(typeCountDic, "Material", assetPath, modelId, assetFileName, assetExtension);
                    }
                    else if (assetPath.EndsWith("fbx")) //fbx
                    {
                        //                        Debug.Log("Fbx：" + assetPath);
                        moveAsset(typeCountDic, "Fbx", assetPath, modelId, assetFileName, assetExtension);
                    }
                    else if (assetPath.EndsWith("prefab")) //fbx
                    {
                        //                        Debug.Log("Prefab：" + assetPath);
                        moveAsset(typeCountDic, null, assetPath, modelId, assetFileName, assetExtension);
                    }
                    else
                    {
                        Debug.LogError($"未知类型：{asset.GetType()}     path:{assetPath}");
                    }
                }
            }

            EditorFileUtil.RemoveEmptyFolder($"Assets/{PathUtil.RES_FOLDER}/Model");
            AssetDatabase.Refresh();
        }


//        [MenuItem("Assets/自动格式化/方案A-Effect")]
        private static void ClassifyFile_Effect()
        {
            ClassifyFileA($"Assets/{PathUtil.RES_FOLDER}/Effect");
        }

        private static void ClassifyFileA(string rootPath)
        {
            List<string> targetPathList = new List<string>(new[] { "Material", "Texture", "Anim", "Fbx" });
            foreach (string folder in targetPathList)
            {
                JQFileUtil.CreateDirectory(rootPath + "/" + folder);
            }

            Dictionary<string, string> typeDictionary = new Dictionary<string, string>();
            typeDictionary[".prefab"] = "Prefab";
            typeDictionary[".mat"] = "Material";
            typeDictionary[".png"] = "Texture";
            typeDictionary[".jpg"] = "Texture";
            typeDictionary[".tga"] = "Texture";
            typeDictionary[".anim"] = "Anim";
            typeDictionary[".controller"] = "Anim";
            typeDictionary[".FBX"] = "Model";
            typeDictionary[".fbx"] = "Model";
            List<string> pathList = new List<string>();
            JQFileUtil.getAllFile(ref pathList, rootPath);
            for (int i = 0; i < pathList.Count; i++)
            {
                string assetPath = pathList[i];
                string parentFolder = JQFileUtil.getParentFolderPath(assetPath, false);
                if (targetPathList.Contains(parentFolder)) continue;
                foreach (KeyValuePair<string, string> keyValuePair in typeDictionary)
                {
                    string type = keyValuePair.Key;
                    string folder = keyValuePair.Value;
                    if (moveAsset(rootPath, assetPath, type, folder))
                    {
                        continue;
                    }
                }
            }

            AssetDatabase.Refresh();
            JQLog.Log(" done!");
        }

        private static bool moveAsset(string rootPath, string assetPath, string type, string folder)
        {
            if (assetPath.EndsWith(type))
            {
                string fileName = JQFileUtil.getCurrFolderOrFileName(assetPath);
                fileName = JQFileUtil.removeExtension(fileName);
                string toPath = $"{rootPath}/{folder}/{fileName}{type}";
                string error = AssetDatabase.MoveAsset(assetPath, toPath);
                int count = 0;
                while (!string.IsNullOrEmpty(error))
                {
                    count++;
                    toPath = $"{rootPath}/{folder}/{fileName}_{count}{type}";
                    error = AssetDatabase.MoveAsset(assetPath, toPath);
                }

                Debug.Log("from:" + assetPath + "  toPath:" + toPath);
                return true;
            }

            return false;
        }

        // [MenuItem("Assets/自动格式化/打印Shader依赖")]
        // public static void printShaderDependencies()
        // {
        //     string[] guids = Selection.assetGUIDs;
        //     if (guids.Length == 0)
        //     {
        //         Debug.LogError("未选中任何文件");
        //         return;
        //     }
        //     string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        //     if (!path.EndsWith(".shader"))
        //     {
        //         Debug.LogError("选中的不是shader文件");
        //         return;
        //     }
        //     Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
        //     string[] dependencies = shader.GetDependency();
        //     foreach (string dependency in dependencies)
        //     {
        //         Debug.Log(dependency);
        //     }
        // }


        private static void CheckFolder(string rootPath, string folder)
        {
            if (!AssetDatabase.IsValidFolder($"{rootPath}/{folder}"))
            {
                AssetDatabase.CreateFolder(rootPath, folder);
            }
        }


        [MenuItem("Assets/场景/检查引用资源路径")]
        public static void customNormalizeScene2222()
        {
            string rootPath = "";
            string assetPath = "";
            var selectObjs = Selection.objects;

            for (int i = 0; i < selectObjs.Length; i++)
            {
                Object selectObj = selectObjs[i];
                assetPath = AssetDatabase.GetAssetPath(selectObj);
                rootPath = JQFileUtil.getParentFolderPath(assetPath);
            
                //根据path拿所有引用资源的path
                string[] depends = AssetDatabase.GetDependencies(assetPath, true);

                int count = 0;
                foreach (var path in depends)
                {
                    if (path.Contains("DownloadRes"))
                    {
                        Debug.Log($"引用到DownloadRes目录的资源：{path}");
                    }
                    // if (path.Contains("Temp"))
                    // {
                    //     Debug.Log($"引用到Temp目录的资源：{path}");
                    // }
                }
                Debug.Log("Finish:"+depends.Length);
            }
            
        }

        [MenuItem("Assets/场景/自动格式化")]
        public static void customNormalizeScene()
        {
            string rootPath = "";
            string assetPath = "";
            var selectobj = Selection.objects;

            assetPath = AssetDatabase.GetAssetPath(selectobj[0]);
            rootPath = JQFileUtil.getParentFolderPath(assetPath);

            CheckFolder(rootPath, "Texture");
            CheckFolder(rootPath, "Anim");
            CheckFolder(rootPath, "Material");
            CheckFolder(rootPath, "Fbx");
            CheckFolder(rootPath, "Prefab");
            CheckFolder(rootPath, "Spm");
            CheckFolder(rootPath, "Script");
            CheckFolder(rootPath, "Shader");

            //根据path拿所有引用资源的path
            string[] depends = AssetDatabase.GetDependencies(assetPath, true);

            int count = 0;
            foreach (var path in depends)
            {
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                // if (path.EndsWith("cs") || path.EndsWith("shader")) continue;

                //场景资源不动
                if (path.EndsWith("unity"))
                {
                    continue;
                }
                
                //插件资源不动
                if (path.StartsWith("Packages"))
                {
                    if (!path.EndsWith(".cs") && !path.EndsWith(".shader"))
                    {
                        Debug.LogWarning("使用了插件内的资源：   " + path);
                    }
                    continue;
                }

                //跨目录引用不动
                if (!path.StartsWith(rootPath))
                {
                    if (!path.EndsWith(".cs") && !path.EndsWith(".shader"))
                    {
                        Debug.LogWarning("跨目录引用：" + path);
                    }
                    continue;
                }

                //光照贴图目录不动
                if (path.StartsWith(rootPath + "/scene_"))
                {
                    continue;
                }

                string assetFileName = JQFileUtil.getCurrFolderOrFileName(path, false);
                string assetExtension = JQFileUtil.getExtension(path);

                if (asset is Texture2D) //贴图
                {
                    //                        Debug.Log("贴图："+ assetPath);
                    string temp = string.Concat(rootPath, "/", "Texture/", assetFileName, ".", assetExtension);
                    AssetDatabase.MoveAsset(path, temp);
                }
                else if (assetExtension.Equals("cs")) //代码
                {
                    //                        Debug.Log("Anim：" + assetPath);
                    AssetDatabase.MoveAsset(path, string.Concat(rootPath, "/", "Script/", assetFileName, ".", assetExtension));
                }
                else if (assetExtension.Equals("shader")) //shader
                {
                    //                        Debug.Log("Anim：" + assetPath);
                    AssetDatabase.MoveAsset(path, string.Concat(rootPath, "/", "Shader/", assetFileName, ".", assetExtension));
                }
                else if (assetExtension.Equals("anim") || assetExtension.Equals("controller")) //动画
                {
                    //                        Debug.Log("Anim：" + assetPath);
                    AssetDatabase.MoveAsset(path, string.Concat(rootPath, "/", "Anim/", assetFileName, ".", assetExtension));
                }
                else if (assetExtension.Equals("spm")) //SpeedTree
                {
                    SpeedTreeImporter modelImporter = AssetImporter.GetAtPath(path) as SpeedTreeImporter;
                    modelImporter.materialLocation = SpeedTreeImporter.MaterialLocation.InPrefab;
                    modelImporter.SaveAndReimport();
                    //                        Debug.Log("Anim：" + assetPath);
                    AssetDatabase.MoveAsset(path, string.Concat(rootPath, "/", "Spm/", assetFileName, ".", assetExtension));
                }
                else if (asset is Material) //anim  controller
                {
                    //                        Debug.Log("Material：" + assetPath);
                    AssetDatabase.MoveAsset(path, string.Concat(rootPath, "/", "Material/", assetFileName, ".", assetExtension));
                }
                else if (assetExtension.Equals("fbx")) //fbx
                {
                    ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
                    modelImporter.materialLocation = ModelImporterMaterialLocation.InPrefab;
                    modelImporter.SaveAndReimport();
                    //                        Debug.Log("Fbx：" + assetPath);
                    AssetDatabase.MoveAsset(path, string.Concat(rootPath, "/", "Fbx/", assetFileName, ".", assetExtension));
                }
                else if (assetExtension.Equals("prefab")) //prefab
                {
                    AssetDatabase.MoveAsset(path, string.Concat(rootPath, "/", "Prefab/", assetFileName, ".", assetExtension));
                }
                else
                {
                    Debug.LogError($"未知类型：{asset.GetType()}     path:{assetPath}");
                }
            }


            AssetDatabase.Refresh();
            JQLog.Log(count + " done!");
        }

//        [MenuItem("Assets/自动格式化/Model")]
        public static void customNormalizeModel()
        {
            List<NormalizeInfo> infoList = new List<NormalizeInfo>();
            infoList.Add(new NormalizeInfo("Anim", IsAnim));
            infoList.Add(new NormalizeInfo("Material", isMaterial));
            infoList.Add(new NormalizeInfo("Fbx", IsFbx));
            //                        infoList.Add(new NormalizeInfo("Prefab", IsPrefab));
            infoList.Add(new NormalizeInfo("Texture", IsTexture));
            NormalizeAssetTypeCtrl effectCtrl = new NormalizeAssetTypeCtrl("model", infoList);
            effectCtrl.Do();
        }

        [MenuItem("GameObject/SPI/场景转Prefab")]
        public static void ChangeSceneFbxSpmToPrefab()
        {
            if (Selection.activeGameObject != null)
            {
                ChangeFbxSpmToPrefab(Selection.activeGameObject);
                Debug.Log("Done!");
                return;
            }

            GameObject[] gameObjects = Object.FindObjectsOfType<GameObject>(true);
            foreach (GameObject gameObject in gameObjects)
            {
                ChangeFbxSpmToPrefab(gameObject);
            }
            Debug.Log("Done!");
        }

        private static void ChangeFbxSpmToPrefab(GameObject gameObject)
        {
            PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(gameObject);
            if (prefabAssetType != PrefabAssetType.Model) return;

            GameObject prefabGo = PrefabUtility.GetNearestPrefabInstanceRoot(gameObject);
            string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefabGo);
            string extension = JQFileUtil.getExtension(path);
            string prefabPath = path.Replace($".{extension}", ".prefab");
            Debug.Log($"Fbx转Prefab, {path} -> {prefabPath}");
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            GameObject[] instances = PrefabUtility.FindAllInstancesOfPrefab(prefab);
            bool hadAddGo = false;
            for (int i = 0; i < instances.Length; i++)
            {
                GameObject instance = instances[i];
                List<AddedGameObject> addedGameObjects = PrefabUtility.GetAddedGameObjects(instance);
                List<RemovedComponent> removedComponents = PrefabUtility.GetRemovedComponents(instance);
                if (addedGameObjects.Count > 0 || removedComponents.Count > 0)
                {
                    Debug.LogError($"            预设内有添加的物体, 请先处理: {PathUtil.getFullPath(instance)}");
                    hadAddGo = true;
                }
            }

            if (hadAddGo) return;
            List<Vector3> positions = new List<Vector3>();
            List<Quaternion> rotations = new List<Quaternion>();
            List<Vector3> scales = new List<Vector3>();
            for (int i = 0; i < instances.Length; i++)
            {
                GameObject instance = instances[i];
                positions.Add(instance.transform.localPosition);
                rotations.Add(instance.transform.localRotation);
                scales.Add(instance.transform.localScale);
            }


            Debug.Log($"相同预设数量：" + instances.Length);
            for (int i = 0; i < instances.Length; i++)
            {
                GameObject instance = instances[i];
                Debug.Log($"            关联Prefab: {PathUtil.getFullPath(instance)}");
                PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.UserAction);
                PrefabUtility.SaveAsPrefabAssetAndConnect(instance, prefabPath, InteractionMode.UserAction);
            }

            //还原Transform
            for (int i = 0; i < instances.Length; i++)
            {
                GameObject instance = instances[i];
                instance.transform.localPosition = positions[i];
                instance.transform.localRotation = rotations[i];
                instance.transform.localScale = scales[i];
            }
        }

        public void Do()
        {
        }

        private static bool IsAnim(Object o, string path)
        {
            return o is AnimationClip || o is AnimatorController;
        }

        private static bool isMaterial(Object o, string path)
        {
            return path.EndsWith("mat");
        }

        private static bool IsFbx(Object o, string path)
        {
            return path.EndsWith("fbx");
        }

        private static bool IsTexture(Object o, string path)
        {
            return o is Texture;
        }

        private static bool IsPrefab(Object o, string path)
        {
            return path.EndsWith("prefab");
        }
    }
}