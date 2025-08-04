
using JQCore.tLog;
using JQCore.tUtil;
using UnityEditor;
using UnityEngine;

namespace JQEditor.Other
{
    public static class ModelTool
    {
        // [MenuItem("IrfTemp/特效SortingGroup批处理")]
        public static void AddSortingGroupToEffect()
        {
            string[] lookFor = new string[] { $"Assets/{PathUtil.RES_FOLDER}/Effect_al" };
            string[] sss = AssetDatabase.FindAssets("t:Prefab", lookFor);
            foreach (string guid in sss)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                JQLog.Log("Path:" + path);
                GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                if (!prefab.name.StartsWith("effect_"))
                {
                    continue;
                }

                GameObject handleGameObject = Object.Instantiate(prefab);
                /*
                string[] xxx = prefab.name.Split('_');
                int effectId = int.Parse(xxx[1]);
                if (6900 <= effectId && effectId <= 6999)
                {

                }
                else
                {
                    Renderer[] rendererArr = handleGameObject.GetComponentsInChildren<Renderer>(true);
                    CheckSortingGroup(handleGameObject, rendererArr); //设置SortingGroup
                }*/
                Renderer[] rendererArr = handleGameObject.GetComponentsInChildren<Renderer>(true);
                CheckSortingGroup(handleGameObject, rendererArr); //设置SortingGroup  
                PrefabUtility.ReplacePrefab(handleGameObject, prefab);
                Object.DestroyImmediate(handleGameObject);
            }
        }


        #region SortingGroup

        //[MenuItem("IrfTemp/Model/设置SortingGroup(单个)")]
        public static void CheckSortingGroup()
        {
            GameObject obj = Selection.activeGameObject;
            Renderer[] rendererArr = obj.GetComponentsInChildren<Renderer>(true);
            CheckSortingGroup(obj, rendererArr);
            //SortSprite(obj, rendererArr);
        }

        private static void CheckSortingGroup(GameObject obj, Renderer[] rendererArr)
        {
//        Dictionary<string, int> layerCountDic = new Dictionary<string, int>();
//        for (int i = 0; i < rendererArr.Length; i++)
//        {
//            Renderer spriteRenderer = rendererArr[i];
//            if (spriteRenderer.gameObject.name != "shadow")
//            {
//                int sortLayerId = spriteRenderer.sortingLayerID;
//                string str = API.Int2StrLib.IntToStr(sortLayerId);
//                if (!layerCountDic.ContainsKey(str))
//                {
//                    layerCountDic[str] = 0;
//                }
//                int count = layerCountDic[sortLayerId.ToString()];
//                count++;
//                layerCountDic[str] = count;
//            }
//        }
//
//        int maxLayerId = -1;
//        int maxCount = -1;
//        foreach (KeyValuePair<string, int> keyValuePair in layerCountDic)
//        {
//            string sortingLayerIdStr = keyValuePair.Key;
//            int count = keyValuePair.Value;
//            if (maxLayerId == -1 && maxCount == -1)
//            {
//                maxLayerId = int.Parse(sortingLayerIdStr);
//                maxCount = count;
//            }
//            else if (count > maxCount)
//            {
//                maxLayerId = int.Parse(sortingLayerIdStr);
//                maxCount = count;
//            }
//        }
//        SortingGroup sortingGroup = obj.GetComponent<SortingGroup>();
//        if (sortingGroup != null)
//        {
//            if (maxLayerId != -1)
//            {
//                sortingGroup.sortingLayerID = maxLayerId;
//            }
//        }
        }

        #endregion
    }
}