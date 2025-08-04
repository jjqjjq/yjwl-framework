using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
#if UNITY_ANDROID
using CodeStage.AntiCheat.Storage;
using JQCore.tAntiCheat;
#endif
using JQCore.tFileSystem;
using JQCore.tUtil;
using JQEditor.Build;
using JQEditor.Check.tCheckTexture;
using JQCore.tRes;
using JQCore.tString;
using JQEditor.Tool;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JQEditor.Check
{
    public static class CheckGlobalTools
    {
        public static void showAllU3DClass(string name, Action endAction)
        {
            string allStr = "";
            List<string> dllList = new List<string>();
            dllList.Add(@"C:\Program Files\Unity\Editor\Data\Managed\UnityEngine\UnityEngine.CoreModule.dll");
            dllList.Add(
                @"C:\Program Files\Unity\Editor\Data\Managed\UnityEngine\UnityEngine.UnityWebRequestWWWModule.dll");
            dllList.Add(@"C:\Program Files\Unity\Editor\Data\Managed\UnityEngine\UnityEngine.AssetBundleModule.dll");
            dllList.Add(@"C:\Program Files\Unity\Editor\Data\Managed\UnityEngine\UnityEngine.AudioModule.dll");
            dllList.Add(@"C:\Program Files\Unity\Editor\Data\Managed\UnityEngine\UnityEngine.dll");
            dllList.Add(
                @"C:\Program Files\Unity\Editor\Data\PlaybackEngines\AndroidPlayer\Variations\il2cpp\Managed\UnityEngine.ImageConversionModule.dll");
            dllList.Add(@"C:\Program Files\Unity\Editor\Data\Managed\UnityEngine\UnityEngine.ScreenCaptureModule.dll");
            dllList.Add(@"C:\Program Files\Unity\Editor\Data\Managed\UnityEngine\UnityEngine.ParticleSystemModule.dll");
            dllList.Add(@"C:\Program Files\Unity\Editor\Data\Managed\UnityEngine\UnityEngine.Physics2DModule.dll");
            dllList.Add(@"C:\Program Files\Unity\Editor\Data\Managed\UnityEngine\UnityEngine.PhysicsModule.dll");
            dllList.Add(
                @"C:\Program Files\Unity\Editor\Data\UnityExtensions\Unity\Timeline\RuntimeEditor\UnityEngine.Timeline.dll");
            dllList.Add(@"C:\Program Files\Unity\Editor\Data\Managed\UnityEngine\UnityEngine.TimelineModule.dll");
            dllList.Add(@"C:\Program Files\Unity\Editor\Data\UnityExtensions\Unity\GUISystem\UnityEngine.UI.dll");
            dllList.Add(@"C:\Program Files\Unity\Editor\Data\Managed\UnityEngine\UnityEngine.UIModule.dll");
            dllList.Add(@"C:\Program Files\Unity\Editor\Data\Managed\UnityEngine\UnityEngine.AnimationModule.dll");
            dllList.Add(@"C:\Program Files\Unity\Editor\Data\Managed\UnityEngine\UnityEngine.SpriteMaskModule.dll");
            ssssss(allStr, dllList);
        }

        private static void ssssss(string ssss, List<string> sssList)
        {
            foreach (string sss in sssList)
            {
                Assembly ass = Assembly.LoadFile(sss);
                Type[] tips = ass.GetTypes();
                for (int i = 0; i < tips.Length; i++)
                {
                    Type type = tips[i];
                    if (type.Namespace == null) continue;

                    if (type.Name.Contains("ManagedStreamHelpers"))
                    {
                        Debug.LogError(type.IsPublic);
                    }

                    if (type.IsClass && type.IsAnsiClass && type.IsAnsiClass && type.IsPublic)
                    {
                        if (type.Namespace.StartsWith("UnityEngine.Windows") ||
                            type.Namespace.StartsWith("UnityEngine.WSA") ||
                            type.Namespace.StartsWith("UnityEngine.WSA") ||
                            type.Namespace.StartsWith("UnityEngine.tvOS") ||
                            type.Namespace.StartsWith("UnityEngine.Experimental") ||
                            type.Namespace.StartsWith("JetBrains.Annotations"))
                        {
                            continue;
                        }

                        ssss += $"typeof({type.Namespace}.{type.Name}),\n";
                    }
                }
            }

            Debug.Log(ssss);
        }


        //[MenuItem("IrfCheck/检查无用组件")]
        public static void CheckUnuseCompnent(string name, Action endAction)
        {
            CheckCommonTools.Search<GameObject>(name, $"Assets/{PathUtil.RES_FOLDER}", checkUnuseComponent, null,
                endAction);
        }

        //检查根节点是否挂载Canva的prefab
        public static void CheckRootCanvas(string name, Action endAction)
        {
            CheckCommonTools.Search<GameObject>(name, $"Assets/{PathUtil.RES_FOLDER}", checkRootCanvas, null,
                endAction);
        }

        private static bool checkRootCanvas(string assetPath, GameObject obj, object obj1)
        {
            Canvas canvas = obj.GetComponent<Canvas>();
            if (canvas != null)
            {
                Debug.Log($"canvas:   {assetPath} ");
            }

            return true;
        }

        public static void RemoveABName(string name, Action endAction)
        {
            var abNames = AssetDatabase.GetAllAssetBundleNames();
            foreach (var abName in abNames)
            {
                AssetDatabase.RemoveAssetBundleName(abName, true);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
        }

        public static void CheckUnuseRes(string name, Action endAction)
        {
            // LuaEnv _luaEnv = new LuaEnv();
            // _luaEnv.AddLoader(LuaLoader.CustomLoader);
            // _luaEnv.DoString(@"require 'editor.CheckResources'");
            // LuaTable luaTable = _luaEnv.Global.Get<LuaTable>("CheckResources");
            // VoidDelegateO func = luaTable.Get<VoidDelegateO>("check");
            // LuaTable tab = (LuaTable)func(null);
            // CheckCommonTools.Search<GameObject>(name, $"Assets/{PathUtil.RES_FOLDER}{"/Card"}", checkUnuseRes, new Dictionary<string, LuaTable> { { "Card", tab } }, endAction);
            // CheckCommonTools.Search<GameObject>(name, $"Assets/{PathUtil.RES_FOLDER}{"/Effect"}", checkUnuseRes, new Dictionary<string, LuaTable> { { "Effect", tab } }, endAction);
            // CheckCommonTools.Search<GameObject>(name, $"Assets/{PathUtil.RES_FOLDER}{"/Model"}", checkUnuseRes, new Dictionary<string, LuaTable> { { "Model", tab } }, endAction);
            // CheckCommonTools.Search<GameObject>(name, $"Assets/{PathUtil.RES_FOLDER}{"/Scene"}", checkUnuseRes, new Dictionary<string, LuaTable> { { "Scene", tab } }, endAction);
            // CheckCommonTools.Search<GameObject>(name, $"Assets/{PathUtil.RES_FOLDER}{"/Sound"}", checkUnuseRes, new Dictionary<string, LuaTable> { { "Sound", tab } }, endAction);
            // CheckCommonTools.Search<Texture2D>(name, $"Assets/{PathUtil.RES_FOLDER}{"/UI"}", checkUnuseTexture, new Dictionary<string, LuaTable> { { "UI", tab } }, endAction, true, ".png");
        }

        public static void CheckImgFormat(string name, Action endAction)
        {
            CheckCommonTools.Search<Texture>(name, $"Assets/{PathUtil.RES_FOLDER}", checkImgFormat, null, endAction,
                true, "", "t:texture");
        }


        /// <summary>
        /// 由输入的Dll文件去生成link.xml
        /// </summary>
        /// <param name="name"></param>
        /// <param name="endAction"></param>
        public static void BuildLinkXml(string name, Action endAction)
        {
#if SDK_WEIXIN
            string unityPath =
                "D:\\Program Files\\Tuanjie\\Hub\\Editor\\2022.3.2t16\\Editor\\Data\\Managed\\UnityEngine\\";
#else
            string unityPath =
 "D:\\Program Files\\Unity\\Hub\\Editor\\2022.3.20f1c1\\Editor\\Data\\Managed\\UnityEngine\\";
#endif
            if (!Directory.Exists(unityPath))
            {
                Debug.LogError($"unity安装路径不存在：{unityPath}  请更正！");
                return;
            }

            string[] readlyDll = new[]
            {
                "UnityEngine.JSONSerializeModule",
                "UnityEngine.PhysicsModule",
                "UnityEngine.TerrainPhysicsModule",
                "UnityEngine.AIModule",
                "UnityEngine.TerrainModule",
                "UnityEngine.AudioModule",
                "UnityEngine.AnimationModule",
                "UnityEngine.WindModule",
                "UnityEngine.CoreModule",
                "UnityEngine.AndroidJNIModule"
            };
            int count = 0;
            List<string> hotfixAssemblies = new List<string>(readlyDll);

            Dictionary<string, List<string>> moduleDic = new Dictionary<string, List<string>>();


            for (int i = 0; i < hotfixAssemblies.Count; i++)
            {
                string dllPath = hotfixAssemblies[i];
                string fullPath = unityPath + dllPath + ".dll";
                if (!File.Exists(fullPath))
                {
                    continue;
                }

                Assembly assembly = Assembly.LoadFile(fullPath);
                List<string> typeList = new List<string>();
                moduleDic.Add(dllPath, typeList);
                Type[] types = assembly.GetExportedTypes();

                for (int j = 0; j < types.Length; j++)
                {
                    Type type = types[j];
                    typeList.Add(type.FullName);
                }

                count++;
                EditorUtility.DisplayProgressBar("Find Class", dllPath, count / (float)hotfixAssemblies.Count);
            }

            EditorUtility.ClearProgressBar();

            StringBuilder stringBuilder = new StringBuilder();


            stringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            stringBuilder.AppendLine("<linker>");
            foreach (var kv in moduleDic)
            {
                string dllName = kv.Key;
                stringBuilder.AppendLine($"  <assembly fullname=\"{dllName}\">");
                List<string> typeNameList = kv.Value;
                for (int i = 0; i < typeNameList.Count; i++)
                {
                    string typeName = typeNameList[i];
                    stringBuilder.AppendLine($"    <type fullname=\"{typeName}\" preserve=\"all\" />");
                }

                stringBuilder.AppendLine($"  </assembly>");
            }

            stringBuilder.AppendLine("</linker>");

            byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
            JQFileUtil.SaveFile(bytes, Application.dataPath + "/Templink.xml");
            Debug.Log("finish:" + Application.dataPath + "/Templink.xml");
            endAction?.Invoke();
        }


        /// <summary>
        /// 打印所有资源引用到的dll
        /// </summary>
        /// <param name="name"></param>
        /// <param name="endAction"></param>
        public static void LogArtUseDll(string name, Action endAction)
        {
            EditorUtility.DisplayProgressBar("Progress", "Find Class...", 0);
            string[] dirs = { $"Assets/{PathUtil.RES_FOLDER}" };
            var asstIds = AssetDatabase.FindAssets("t:Prefab", dirs);
            int count = 0;
            List<string> dllNameList = new List<string>();
            for (int i = 0; i < asstIds.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(asstIds[i]);
                if (path.Equals("Temp")) continue;
                var pfb = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                var coms = pfb.GetComponentsInChildren<Component>(true);
                foreach (var com in coms)
                {
                    if (com == null) continue;
                    Type type = com.GetType();
                    string dllName = type.Module.ScopeName;
                    if (!dllNameList.Contains(dllName))
                    {
                        dllNameList.Add(dllName);
                        Debug.Log(dllName);
                    }
                }

                count++;
                EditorUtility.DisplayProgressBar("Find Class", pfb.name, count / (float)asstIds.Length);
            }

            EditorUtility.ClearProgressBar();

            endAction?.Invoke();
            Debug.Log("finish");
        }

        private static bool checkImgFormat(string assetPath, Texture obj, object obj1)
        {
            if (!assetPath.EndsWith(".exr") && !assetPath.EndsWith(".jpg") && !assetPath.EndsWith(".png") &&
                !assetPath.EndsWith(".PNG") && !assetPath.EndsWith(".ttf") && !assetPath.EndsWith(".asset"))
            {
                Debug.LogError("图片格式必需为jpg、png、exr、ttf、asset：" + assetPath);
            }

            return true;
        }

        //[MenuItem("IrfTemp/Model/模型批处理")]
        public static void CheckMissingReferences(string name, Action endAction)
        {
            CheckCommonTools.Search<GameObject>(name, $"Assets/{PathUtil.RES_FOLDER}", checkMissingReferences, null,
                endAction);
        }

        static Dictionary<string, Material> _replaceDic;

        public static Dictionary<string, Material> replaceDic
        {
            get
            {
                if (_replaceDic == null)
                {
                    _replaceDic = new Dictionary<string, Material>();
                    replaceDic["Packages/com.unity.render-pipelines.universal/Runtime/Materials/Lit.mat"] =
                        AssetDatabase.LoadAssetAtPath<Material>(
                            $"Assets/{PathUtil.RES_FOLDER}/Common/Materials/Lit.mat");
                    replaceDic["Packages/com.unity.render-pipelines.universal/Runtime/Materials/ParticlesUnlit.mat"] =
                        AssetDatabase.LoadAssetAtPath<Material>(
                            $"Assets/{PathUtil.RES_FOLDER}/Common/Materials/ParticlesUnlit.mat");
                    replaceDic["Packages/com.unity.render-pipelines.universal/Runtime/Materials/TerrainLit.mat"] =
                        AssetDatabase.LoadAssetAtPath<Material>(
                            $"Assets/{PathUtil.RES_FOLDER}/Common/Materials/TerrainLit.mat");
                }

                return _replaceDic;
            }
        }


        public static void replaceUsePackageMaterials(GameObject rootGameObject)
        {
            Renderer[] rendererArr = rootGameObject.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in rendererArr)
            {
                Material[] materials = renderer.sharedMaterials;
                Material[] newMaterials = new Material[materials.Length];
                for (int i = 0; i < materials.Length; i++)
                {
                    Material material = materials[i];
                    newMaterials[i] = material;
                    if (material == null) continue;

                    string materialPath = AssetDatabase.GetAssetPath(material);
                    if (materialPath.StartsWith("Packages"))
                    {
                        string rootPath = PathUtil.getFullPath(renderer.gameObject);
                        if (replaceDic.ContainsKey(materialPath))
                        {
                            newMaterials[i] = replaceDic[materialPath];
                            Debug.Log($"替换插件材质球： {materialPath} {rootPath}");
                        }
                        else
                        {
                            Debug.LogError($"材质球在插件内： {materialPath}  {rootPath}");
                        }
                    }
                }

                renderer.sharedMaterials = newMaterials;
            }

            EditorUtility.SetDirty(rootGameObject);
        }


        public static void CheckSceneMissingReferences(string name, Action endAction)
        {
            CheckCommonTools.Search<GameObject>(name, $"Assets/{PathUtil.RES_FOLDER}/Scene", checkMissingReferences,
                null, endAction);
        }

        public static void CheckEffectMissingReferences(string name, Action endAction)
        {
            CheckCommonTools.Search<GameObject>(name, $"Assets/{PathUtil.RES_FOLDER}/Effect", checkMissingReferences,
                null, endAction);
//            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/Effect", checkMissingReferences, null, endAction);
        }

        public static void CheckUIMissingReferences(string name, Action endAction)
        {
            CheckCommonTools.Search<GameObject>(name, $"Assets/{PathUtil.RES_FOLDER}/UI", checkMissingReferences, null,
                endAction);
//            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/UI", checkMissingReferences, null, endAction);
        }

        private static bool checkMissingReferences(string assetPath, GameObject obj, object obj1)
        {
            bool change = false;
            Component[] cps = obj.GetComponentsInChildren<Component>(true);
            for (int idx = 0; idx < cps.Length; idx++)
            {
                Component cp = cps[idx];
                bool componentChange = false;
                SerializedObject so = new SerializedObject(cp);
                SerializedProperty sp = so.GetIterator();

                PropertyInfo objRefValueMethod = typeof(SerializedProperty).GetProperty("objectReferenceStringValue",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                while (sp.Next(true))
                {
                    if (sp.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        string objectReferenceStringValue = string.Empty;

                        if (objRefValueMethod != null)
                        {
                            objectReferenceStringValue =
                                (string)objRefValueMethod.GetGetMethod(true).Invoke(sp, new object[] { });
                        }

                        if (objectReferenceStringValue.StartsWith("Missing"))
                        {
                            string varName = ObjectNames.NicifyVariableName(sp.name);
                            //                            if (varName.Equals("Mesh") &&  cp is ParticleSystemRenderer)
                            //                            {
                            //                                ParticleSystemRenderer particleSystemRenderer = cp as ParticleSystemRenderer;
                            //                                if (particleSystemRenderer.renderMode != ParticleSystemRenderMode.Mesh)
                            //                                {
                            //                                    sp.objectReferenceValue = null;
                            //                                    change = true;
                            //                                    componentChange = true;
                            //                                    Debug.LogError($"ParticleSystemRenderer Mesh Fix：{assetPath}=>{cp.name} =>{cp.GetType().Name}=>{varName}=>{objectReferenceStringValue}");
                            //                                    continue;
                            //                                }
                            //                            }
//                                                        if (varName.Equals("Mesh") &&  cp is ParticleSystem)
//                                                        {
//                                                            ParticleSystem particleSystem = cp as ParticleSystem;
//                                                           
//                                                            if (!(particleSystem.shape.shapeType == ParticleSystemShapeType.Mesh || particleSystem.shape.shapeType == ParticleSystemShapeType.MeshRenderer))
//                                                            {
//                                                                sp.objectReferenceValue = null;
//                                                                change = true;
//                                                                componentChange = true;
//                                                                Debug.LogError($"ParticleSystem.shape Mesh Fix：{assetPath}=>{cp.name} =>{cp.GetType().Name}=>{varName}=>{objectReferenceStringValue}");
//                                                                continue;
//                                                            }
//                                                        }
//                            sp.objectReferenceValue = null;
//                            change = true;
//                            componentChange = true;
                            //                            if (varName.Equals("Sprite") && cp is Image)
                            //                            {
                            //                                sp.objectReferenceValue = null;
                            //                                change = true;
                            //                                componentChange = true;
                            //                                Debug.LogError($"Image Sprite Fix：{assetPath}=>{cp.name} =>{cp.GetType().Name}=>{varName}=>{objectReferenceStringValue}");
                            //                                continue;
                            //                            }

                            Debug.LogError(
                                $"引用丢失：{assetPath}=>{cp.name} =>{cp.GetType().Name}=>{varName}=>{objectReferenceStringValue}");
                        }
                    }
                }

                if (componentChange)
                {
                    so.ApplyModifiedProperties();
                }
            }

            return change;
        }

        private static bool checkUnuseComponent(string assetPath, GameObject obj, object obj1)
        {
            //animator 
            Animator[] animators = obj.GetComponentsInChildren<Animator>();
            for (int j = 0; j < animators.Length; j++)
            {
                Animator animator = animators[j];
                if (animator.runtimeAnimatorController == null)
                {
                    Debug.LogErrorFormat("[无用的Animator]path:{0},   obj:{1}", assetPath, animator.name);
                }
            }

            return true;
        }

        // private static bool checkUnuseRes(string assetPath, GameObject obj, object obj1)
        // {
        //     Dictionary<string, LuaTable> dic = (Dictionary<string, LuaTable>)obj1;
        //     LuaTable tab = null;
        //     object b;
        //     foreach (var item in dic)
        //     {
        //         item.Value.Get(item.Key, out tab);
        //     }
        //     if (tab != null)
        //     {
        //         tab.Get(obj.name.ToString(), out b);
        //         if (b == null)
        //         {
        //             Debug.LogErrorForrmat("无用资源：{0}", assetPath);
        //         }
        //     }
        //     else
        //     {
        //         Debug.LogErrorForrmat("无用资源：{0}", assetPath);
        //     }
        //     return true;
        // }
        // private static bool checkUnuseTexture(string assetPath, Texture2D obj, object obj1)
        // {
        //     Dictionary<string, LuaTable> dic = (Dictionary<string, LuaTable>)obj1;
        //     LuaTable tab = null;
        //     object b;
        //     foreach (var item in dic)
        //     {
        //         item.Value.Get(item.Key, out tab);
        //     }
        //     if (tab != null)
        //     {
        //         tab.Get(obj.name, out b);
        //         if (b == null)
        //         {
        //             Debug.LogErrorForrmat("无用资源：{0}", assetPath);
        //         }
        //     }
        //     else
        //     {
        //         Debug.LogErrorForrmat("无用资源：{0}", assetPath);
        //     }
        //     return true;
        // }
        //[MenuItem("Tools/Search For Components")]
        public static void SearchMissComponents(string name, Action endAction)
        {
            SearchForComponents.Init();
            if (endAction != null)
            {
                endAction();
            }
        }

        private static void checkAction(List<string> urlList, string log)
        {
            for (int i = 0; i < urlList.Count; i++)
            {
                string url = urlList[i];
                Object obj = AssetDatabase.LoadAssetAtPath($"Assets/{PathUtil.RES_FOLDER}/" + url, typeof(Object));
                if (obj == null)
                {
                    Debug.LogError(String.Format("[{0}] 查无该资源：{1}", log, url));
                }
            }
        }

        public static void addAssetToAtlasLib<T>(string folderName, string prefabName, string searchStr)
            where T : Object
        {
            string prefabPath = $"Assets/{PathUtil.RES_FOLDER}/{prefabName}.prefab";
            Object itemPrefab = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
            if (itemPrefab == null)
            {
                Debug.LogError($"查无Prefab:{prefabPath}");
                return;
            }

            GameObject handleGameObject = PrefabUtility.InstantiatePrefab(itemPrefab) as GameObject;
            AssetObjectLib assetObjectLib = handleGameObject.GetComponent<AssetObjectLib>();
            string[] lookFor = new string[] { $"Assets/{PathUtil.RES_FOLDER}/UI/ProgramLoad/" + folderName };
            string[] sss = AssetDatabase.FindAssets(searchStr, lookFor);
            Debug.Log(string.Format("处理{0}:{1}", folderName, sss.Length));
            List<T> spriteList = new List<T>();
            foreach (string guid in sss)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("Temp")) continue;
                T assetObj = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
                if (assetObj == null) continue;
                spriteList.Add(assetObj);
            }

            assetObjectLib.assets = spriteList.ToArray();
            PrefabUtility.ReplacePrefab(handleGameObject, itemPrefab);
            Object.DestroyImmediate(handleGameObject);

            //是图片，打图集
            if (searchStr.Equals("t:texture2D"))
            {
                CheckSpriteAtlasInfo checkSpriteAtlasInfo =
                    new CheckSpriteAtlasInfo($"Assets/{PathUtil.RES_FOLDER}", prefabName);
                checkSpriteAtlasInfo.spriteList = new List<Sprite>();
                checkSpriteAtlasInfo.AddSprites(spriteList.ToArray() as Sprite[]);
            }
        }

        public static void PrintErrorCode(String arg1, Action arg2)
        {
            string[] lines =
                JQFileUtil.ReadFileAllLine("Assets\\Scripts\\ETFramework\\Model\\Share\\Module\\Message\\ErrorCode.cs");
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string line in lines)
            {
                if (line.Contains("public const int"))
                {
                    string[] lineParts1 = line.Split(" = ");
                    string[] lineParts2 = lineParts1[1].Split("; //");
                    string errorCode = lineParts2[0];
                    string errorStr = lineParts2[1];
                    if (errorCode != "0")
                    {
                        stringBuilder.Append($"{lineParts2[0]}\t{lineParts2[1]}\n");
                    }
                }
            }

            Debug.Log(stringBuilder.ToString());
        }

        #region 团结引擎GUID转换为Unity可识别

        #region 收集需要转换的

        public static void fixGUIDCollect(string arg1, Action arg2)
        {
            StringBuilder stringBuilder = new StringBuilder();
            fixGUIDCollectByFolder($"", stringBuilder);
            Debug.Log(stringBuilder.ToString());
            JQFileUtil.SaveFile(stringBuilder.ToString(), "Assets/FixGuidLib.txt");
        }

        private static void fixGUIDCollectByFolder(string folder, StringBuilder stringBuilder)
        {
            string[] lookFor = new string[] { $"Assets{folder}" };
            string[] sss = AssetDatabase.FindAssets("", lookFor);
            Debug.Log(string.Format("处理{0}:{1}", folder, sss.Length));
            for (int i = 0; i < sss.Length; i++)
            {
                string guid = sss[i];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                fixGUIDCollectMetaFile(path, guid, stringBuilder);
            }
        }

        private static void fixGUIDCollectMetaFile(string path, string guid, StringBuilder stringBuilder)
        {
            string metaFilePath = path + ".meta";
            string[] lines = JQFileUtil.ReadFileAllLine(metaFilePath);

            foreach (string line in lines)
            {
                string appendLine = line;
                if (line.StartsWith("guid: "))
                {
                    string metaFileGUID = line.Split(": ")[1];
                    if (!guid.Equals(metaFileGUID))
                    {
                        appendLine = line.Replace(metaFileGUID, guid);
                        Debug.Log($"path:{path} {line}=>{appendLine}");
                        stringBuilder.Append($"{path}:{metaFileGUID}:{guid}\n");
                    }
                }
            }
        }

        #endregion

        #region 处理需要转换的GUID

        public static void fixGUIDExecute(string arg1, Action arg2)
        {
            string[] lines = JQFileUtil.ReadFileAllLine("Assets/FixGuidLib.txt");
            foreach (string line in lines)
            {
                string[] ss = line.Split(':');
                string path = ss[0];
                string metaFileGUID = ss[1];
                string guid = ss[2];
                fixGUIDExecuteMetaFile(path, metaFileGUID, guid);
            }

            fixSceneExecute("");
        }

        private static void fixSceneExecute(string path)
        {
            //收集path目录中后缀为.scene的文件，将后缀改为.unity
            List<string> sceneList = new List<string>();
            Debug.LogError(Application.dataPath);
            JQFileUtil.getAllFile(ref sceneList, Application.dataPath, isSceneSuffix, true);
            foreach (string filePath in sceneList)
            {
                Debug.LogError(filePath);
                string newFilePath = filePath.Replace(".scene", ".unity");
                JQFileUtil.FileRenamePath(filePath, newFilePath);
            }
        }

        private static bool isSceneSuffix(string path)
        {
            return path.EndsWith(".scene");
        }

        private static void fixGUIDExecuteMetaFile(string path, string metaFileGUID, string guid)
        {
            string metaFilePath = path + ".meta";
            string[] lines = JQFileUtil.ReadFileAllLine(metaFilePath);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string line in lines)
            {
                string appendLine = line;
                if (line.StartsWith("guid: "))
                {
                    appendLine = line.Replace(metaFileGUID, guid);
                    Debug.LogError($"path:{path} {line}=>{appendLine}");
                }

                stringBuilder.Append(appendLine + "\n");
            }

            JQFileUtil.SaveFile(stringBuilder.ToString(), metaFilePath);
        }

        #endregion

        #endregion

        public static void RenameAssets(string arg1, Action arg2)
        {
            //批量重命名指定文件夹下的所有文件
            DirectoryInfo dir = new DirectoryInfo($"Assets/{PathUtil.RES_FOLDER}/Goods"); //目录
            DirectoryInfo[] xx = dir.GetDirectories();
            foreach (DirectoryInfo d in xx)
            {
                string folder = d.FullName;
                folder = folder.Replace("\\", "/");
                folder = folder.Replace(Application.dataPath, "Assets");
                string[] lookFor = new string[] { folder };
                string[] assetGUIDs = AssetDatabase.FindAssets("", lookFor);
                Debug.Log(string.Format("处理{0}:{1}", folder, assetGUIDs.Length));
                int folderIndex = int.Parse(d.Name);
                for (int i = 0; i < assetGUIDs.Length; i++)
                {
                    int name = folderIndex * 100 + i + 1; //新名字的定义
                    string guid = assetGUIDs[i];
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    AssetDatabase.RenameAsset(path, name.ToString());
                }
            }
        }

        public static void DeleteSaves(string arg1, Action arg2)
        {
#if UNITY_ANDROID
            var deviceLockSettings = new DeviceLockSettings(DeviceLockLevel.Strict);
            var encryptionSettings = new EncryptionSettings(AntiCheatMgr.encryptionKey);
            var settings = new ObscuredFileSettings(encryptionSettings, deviceLockSettings);
            ObscuredFilePrefs.Init("android_setting.bin", settings, true);
            ObscuredFilePrefs.DeleteAll();
#else
            UnityEngine.PlayerPrefs.DeleteAll();
#endif
        }

        private static int _captureIndex = 0;

        public static void CaptureScreenshot(string arg1, Action arg2)
        {
            JQFileUtil.CreateDirectory($"{Application.dataPath}/CaptureScreenshot");
            string path = $"{Application.dataPath}/CaptureScreenshot/{_captureIndex}.png";
            ScreenCapture.CaptureScreenshot(path);
            Debug.Log("CaptureScreenshot:" + path);
            _captureIndex++;
        }

        private static int _lines;

        public static void CollectCode(string arg1, Action arg2)
        {
            _lines = 0;
            StringBuilder stringBuilder = new StringBuilder();
            searchCodeFile($"Scripts/JQGame/ECS", stringBuilder);

            if (_lines < 3100)
            {
                searchCodeFile($"Scripts/JQGame/MVC/Module", stringBuilder);
            }

            Debug.Log(stringBuilder.ToString());
            JQFileUtil.SaveFile(stringBuilder.ToString(), "Assets/Code.txt");
        }

        private static void searchCodeFile(string folder, StringBuilder stringBuilder)
        {
            string[] lookFor = new string[] { $"Assets/{folder}" };
            string[] sss = AssetDatabase.FindAssets("t:Script", lookFor);
            Debug.Log(string.Format("处理{0}:{1}", folder, sss.Length));
            for (int i = 0; i < sss.Length; i++)
            {
                string guid = sss[i];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string csFilePath = path;
                string[] lines = JQFileUtil.ReadFileAllLine(csFilePath);
                foreach (string line in lines)
                {
                    stringBuilder.AppendLine(line);
                    _lines++;
                }
                if (_lines > 3100)
                {
                    return;
                }
            }
        }
    }
}