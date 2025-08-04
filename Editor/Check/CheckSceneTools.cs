/*----------------------------------------------------------------
// 文件名：CheckSceneTools.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/8/14 14:05:52
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using JQCore.tUtil;
using JQEditor.Build;
using JQCore.Log;
using JQCore.tString;
using JQEditor.Excel;
using JQEditor.MainSubPackage;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JQEditor.Check
{

    public static class CheckSceneTools
    {

        /// <summary>
        /// 整理场景引用资源
        /// Dependence
        /// </summary>
        public static void TidyScene()
        {
            // AssetDatabase.GetDependencies()
        }


        public static void setOrder(string name, Action endAction)
        {
            CheckCommonTools.SearchAndDo(name, $"Assets/{PathUtil.RES_FOLDER}/Scene", linkAllDynIamge, null, endAction);
        }

        public static void checkUsePackageMaterials(string name, Action endAction)
        {
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            foreach (GameObject rootGameObject in rootGameObjects)
            {
                CheckGlobalTools.replaceUsePackageMaterials(rootGameObject);
                replaceUsePackageMaterials(rootGameObject);
            }

            EditorSceneManager.SaveScene(scene);
            Debug.Log("xxx");
        }

        public static void replaceUsePackageMaterials(GameObject rootGameObject)
        {
            Terrain[] terrains = rootGameObject.GetComponentsInChildren<Terrain>(true);
            foreach (var terrain in terrains)
            {
                Material material = terrain.materialTemplate;
                if (material == null) continue;

                string materialPath = AssetDatabase.GetAssetPath(material);
                if (materialPath.StartsWith("Packages"))
                {
                    string rootPath = PathUtil.getFullPath(terrain.gameObject);
                    if (CheckGlobalTools.replaceDic.ContainsKey(materialPath))
                    {
                        terrain.materialTemplate = CheckGlobalTools.replaceDic[materialPath];
                        Debug.Log($"替换插件材质球： {materialPath} {rootPath}");
                    }
                    else
                    {
                        Debug.LogError($"材质球在插件内： {materialPath}  {rootPath}");
                    }
                }
            }

            EditorUtility.SetDirty(rootGameObject);
        }


        private static bool linkAllDynIamge(string assetPath, GameObject cloneGo, object obj1)
        {
            bool change = false;
            Transform stagesTransform = cloneGo.transform.Find("Stages");
            foreach (Transform subMapTransform in stagesTransform)
            {
                Transform centerPosTransform = subMapTransform.Find("centerPos");
                if (centerPosTransform == null)
                {
                    GameObject centerPosGo = new GameObject("centerPos");
                    centerPosGo.transform.SetParent(subMapTransform);
                    centerPosGo.transform.localPosition = Vector3.zero;
                    change = true;
                }

                foreach (Transform childTrans in subMapTransform)
                {
                    string childName = childTrans.name;
                    if (childName != "centerPos")
                    {
//                        int.TryParse(childTrans.name.Replace("bg", ""), out int layer);

//                        SpriteRenderer[] spriteRenderers = childTrans.gameObject.GetComponentsInChildren<SpriteRenderer>();
//                        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
//                        {
//                            if (spriteRenderer.sortingLayerName != "Default")
//                            {
//                                spriteRenderer.sortingLayerName = "Default";
//                                change = true;
//                            }
//                        }
                        int.TryParse(childTrans.name.Replace("bg", ""), out int layer);
                        Renderer[] spriteRenderers = childTrans.gameObject.GetComponentsInChildren<Renderer>(true);
                        foreach (Renderer spriteRenderer in spriteRenderers)
                        {
                            if (layer <= 5) //SortingLayer1
                            {
                                string fullPath = PathUtil.getFullPath(spriteRenderer.gameObject);
                                if (fullPath.Contains("top")) //中景上层
                                {
                                    if (spriteRenderer.sortingLayerName != "layer10")
                                    {
                                        spriteRenderer.sortingLayerName = "layer10";
                                        change = true;
                                    }
                                }
                                else //地图远景
                                {
                                    if (spriteRenderer.sortingLayerName != "layer1")
                                    {
                                        spriteRenderer.sortingLayerName = "layer1";
                                        change = true;
                                    }
                                }
                            }
                            else
                            {
                                if (spriteRenderer.sortingLayerName != "layer15")
                                {
                                    spriteRenderer.sortingLayerName = "layer15";
                                    change = true;
                                }
                            }
                        }
                    }
                }
            }

            return change;
        }
    }
}