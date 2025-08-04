using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JQEditor.Tool
{
    public static class UnityEditorUtil
    {
        public static void ApplyPrefab(GameObject gameObject)
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage == null)
            {
                GameObject prefabGo = PrefabUtility.GetNearestPrefabInstanceRoot(gameObject);
                PrefabUtility.ApplyPrefabInstance(prefabGo, InteractionMode.AutomatedAction);
            }
            else
            {
                PrefabUtility.SaveAsPrefabAsset(prefabStage.prefabContentsRoot, prefabStage.assetPath);
            }
        }

    }
}
