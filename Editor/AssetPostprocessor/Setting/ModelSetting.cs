using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace JQEditor.tAssetPostprocessor
{
    public class ModelSetting
    {
        //设置模型默认参数 
        public static void ApplyFBXModelSettings(ModelImporter importer)
        {
            bool dirty = false;

            //模型网格压缩(仅节省硬盘，会覆盖)
            if (importer.meshCompression != ModelImporterMeshCompression.Off)
            {
                importer.meshCompression = ModelImporterMeshCompression.Off;
                dirty = true;
            }

            if (importer.optimizeMeshPolygons != true)
            {
                importer.optimizeMeshPolygons = true;
                dirty = true;
            }

            if (importer.optimizeMeshVertices != true)
            {
                importer.optimizeMeshVertices = true;
                dirty = true;
            }

            if (importer.meshOptimizationFlags != MeshOptimizationFlags.Everything)
            {
                importer.meshOptimizationFlags = MeshOptimizationFlags.Everything;
            }

            //动画曲线重采样
            if (importer.resampleCurves)
            {
                importer.resampleCurves = false;
                dirty = true;
            }

            if (importer.optimizeGameObjects)
            {
                importer.optimizeGameObjects = false;
                dirty = true;
            }

            //关键帧压缩
            if (importer.animationCompression != ModelImporterAnimationCompression.Optimal)
            {
                importer.animationCompression = ModelImporterAnimationCompression.Optimal;
                dirty = true;
            }

            if (importer.isReadable)
            {
                importer.isReadable = false;
                dirty = true;
            }

            if (importer.keepQuads)
            {
                importer.keepQuads = false;
                dirty = true;
            }

            if (importer.swapUVChannels)
            {
                importer.swapUVChannels = false;
                dirty = true;
            }

            if (importer.generateSecondaryUV == false)
            {
                importer.generateSecondaryUV = true;
                dirty = true;
            }

            if (importer.importBlendShapes)
            {
                importer.importBlendShapes = false;
                dirty = true;
            }

            /*
            if (importer.importNormals != ModelImporterNormals.None)
            {
                importer.importNormals = ModelImporterNormals.None;
                dirty = true;
            }
            */
            //
            // if (importer.materialImportMode !=  ModelImporterMaterialImportMode.None)
            // {
            //     importer.materialImportMode = ModelImporterMaterialImportMode.None;
            //     dirty = true;
            // }
            //
            // if (importer.materialLocation !=  ModelImporterMaterialLocation.InPrefab)
            // {
            //     importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
            //     dirty = true;
            // }

            // if (importer.animationType != ModelImporterAnimationType.Generic)
            // {
            //     importer.animationType = ModelImporterAnimationType.Generic;
            //     dirty = true;
            // }

            if (importer.importVisibility)
            {
                importer.importVisibility = false;
                dirty = true;
            }

            if (importer.weldVertices == false)
            {
                importer.weldVertices = true;
                dirty = true;
            }

            if (importer.importCameras)
            {
                importer.importCameras = false;
                dirty = true;
            }

            if (importer.importLights)
            {
                importer.importLights = false;
                dirty = true;
            }

            if (importer.addCollider)
            {
                importer.addCollider = false;
                dirty = true;
            }

#if UNITY_2019_3_OR_NEWER
            if (importer.sortHierarchyByName)
            {
                importer.sortHierarchyByName = false;
                dirty = true;
            }

            if (importer.avatarSetup != ModelImporterAvatarSetup.CreateFromThisModel)
            {
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                dirty = true;
            }
#endif

//        if (dirty)
//        {
//            Debug.Log("Modifying Model Import Settings, An Import will now occur and the settings will be checked to be OK again during that import");
//            importer.SaveAndReimport();
//        }
//        else
//        {
//            Debug.Log("Model Import Settings OK");
//        }
        }

        //设置模型默认参数 
        public static void ApplyEffectModelSettings(ModelImporter importer)
        {
            bool dirty = false;

            //模型网格压缩
            if (importer.meshCompression != ModelImporterMeshCompression.Low)
            {
                importer.meshCompression = ModelImporterMeshCompression.Low;
                dirty = true;
            }

            //动画曲线重采样
            if (importer.resampleCurves)
            {
                importer.resampleCurves = false;
                dirty = true;
            }

            //关键帧压缩
            if (importer.animationCompression != ModelImporterAnimationCompression.Off)
            {
                importer.animationCompression = ModelImporterAnimationCompression.Off;
                dirty = true;
            }

            if (importer.isReadable)
            {
                importer.isReadable = false;
                dirty = true;
            }

            if (importer.keepQuads)
            {
                importer.keepQuads = false;
                dirty = true;
            }

            if (importer.swapUVChannels)
            {
                importer.swapUVChannels = false;
                dirty = true;
            }

            if (importer.generateSecondaryUV)
            {
                importer.generateSecondaryUV = false;
                dirty = true;
            }

            if (importer.optimizeMesh == false)
            {
                importer.optimizeMesh = true;
                dirty = true;
            }

            if (importer.importBlendShapes)
            {
                importer.importBlendShapes = false;
                dirty = true;
            }

            if (importer.importNormals != ModelImporterNormals.Import)
            {
                importer.importNormals = ModelImporterNormals.Import;
                dirty = true;
            }

            if (importer.materialImportMode != ModelImporterMaterialImportMode.None)
            {
                importer.materialImportMode = ModelImporterMaterialImportMode.None;
                dirty = true;
            }

            if (importer.animationType != ModelImporterAnimationType.Generic)
            {
                importer.animationType = ModelImporterAnimationType.Generic;
                dirty = true;
            }

            if (importer.importVisibility)
            {
                importer.importVisibility = false;
                dirty = true;
            }

            if (importer.weldVertices == false)
            {
                importer.weldVertices = true;
                dirty = true;
            }

            if (importer.importCameras)
            {
                importer.importCameras = false;
                dirty = true;
            }

            if (importer.importLights)
            {
                importer.importLights = false;
                dirty = true;
            }

            if (importer.addCollider)
            {
                importer.addCollider = false;
                dirty = true;
            }
#if UNITY_2019_3_OR_NEWER
            if (importer.sortHierarchyByName)
            {
                importer.sortHierarchyByName = false;
                dirty = true;
            }

            if (importer.avatarSetup != ModelImporterAvatarSetup.CreateFromThisModel)
            {
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                dirty = true;
            }
#endif
//        if (dirty)
//        {
//            Debug.Log("Modifying Model Import Settings, An Import will now occur and the settings will be checked to be OK again during that import");
//            importer.SaveAndReimport();
//        }
//        else
//        {
//            Debug.Log("Model Import Settings OK");
//        }
        }

        //保留挂载点
        public static void ApplyExtraExposedTransformPaths(ModelImporter importer, GameObject obj)
        {
            if (importer.optimizeGameObjects == false)
            {
                Transform[] childTransforms = obj.GetComponentsInChildren<Transform>();
                List<string> mounts = new List<string>();

                string[] oldextraExposeds = importer.extraExposedTransformPaths;
                bool isChangeExposed = false;
                if (oldextraExposeds.Length == 0)
                {
                    isChangeExposed = true;
                }

                foreach (Transform childTransform in childTransforms)
                {
                    if (childTransform.name.Contains("m_"))
                    {
                        mounts.Add(childTransform.name);
                        if (isChangeExposed == false)
                        {
                            for (int i = 0; i < oldextraExposeds.Length; i++)
                            {
                                if (childTransform.name != oldextraExposeds[i])
                                {
                                    isChangeExposed = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (isChangeExposed)
                {
                    importer.optimizeGameObjects = true;
                    importer.extraExposedTransformPaths = mounts.ToArray();
                    importer.SaveAndReimport(); //不要调用这句
                }
            }
        }
    }
}