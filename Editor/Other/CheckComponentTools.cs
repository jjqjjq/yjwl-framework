using JQCore.tUtil;
using UnityEditor;
using UnityEngine;

namespace JQEditor.Other
{
    public class CheckComponentTools : Editor
    {
        //[MenuItem("IrfCheck/组件/1、 查找下所有存在空组件的gameObject")]
        public static void ShowMissingScript()
        {
            //获取路径下所有的gameObject GUID，存入数组
            string[] lookFor = new string[] { $"Assets/{PathUtil.RES_FOLDER}" };
            string[] dataArray = AssetDatabase.FindAssets("t:gameObject", lookFor);

            //遍历数组里的每个gameObject，并且筛选出prefab作处理
            foreach (string data in dataArray)
            {
                string path = AssetDatabase.GUIDToAssetPath(data);
                if (!path.EndsWith(".prefab")) continue;

                GameObject handleGameObject = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                var components = handleGameObject.GetComponentsInChildren<Component>();

                foreach (var c in components)
                {
                    if (c == null)
                    {
                        Debug.LogError(path + "  中存在空组件！");
                    }
                }
            }

            Debug.Log("检查完成！");
        }


        // [MenuItem("IrfCheck/组件/2、 删除Res下所有gameObject中的空组件")]
        public static void ShowMissingScript1()
        {
            //获取路径下所有的gameObject GUID，存入数组
            string[] lookFor = new string[] { $"Assets/{PathUtil.RES_FOLDER}" };
            string[] dataArray = AssetDatabase.FindAssets("t:gameObject", lookFor);

            //遍历数组里的每个gameObject，并且筛选出prefab作处理
            foreach (string data in dataArray)
            {
                string path = AssetDatabase.GUIDToAssetPath(data);
                if (!path.EndsWith(".prefab")) continue;

                GameObject item = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                var components = item.GetComponentsInChildren<Component>();

                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] == null)
                    {
                        GameObject handleGameObject = PrefabUtility.InstantiatePrefab(item) as GameObject;
                        Transform[] handleTransforms = handleGameObject.GetComponentsInChildren<Transform>(true);

                        foreach (Transform handleTransform in handleTransforms)
                        {
                            SerializedObject so = new SerializedObject(handleTransform.gameObject);
                            var soProperties = so.FindProperty("m_Component");
                            var components2 = handleTransform.gameObject.GetComponents<Component>();
                            int propertyIndex = 0;
                            foreach (var c in components2)
                            {
                                if (c == null)
                                {
                                    soProperties.DeleteArrayElementAtIndex(propertyIndex);
                                    Debug.Log(handleGameObject.name + "的节点" + handleTransform.name + "  中的空组件删除成功！");
                                }

                                ++propertyIndex;
                            }

                            so.ApplyModifiedProperties();
                        }

                        PrefabUtility.ReplacePrefab(handleGameObject, item, ReplacePrefabOptions.Default);
                        DestroyImmediate(handleGameObject);
                    }
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("全部删除完成！");
        }
    }
}