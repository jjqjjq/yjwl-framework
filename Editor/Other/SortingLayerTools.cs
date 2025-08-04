using UnityEditor;
using UnityEngine;

namespace JQEditor.Other
{
    public class SortingLayerTools
    {
        // [MenuItem("IrfTools/加工流程/子节点Z轴排序")]
        public static void sortZ()
        {
            GameObject gameObject = Selection.objects[0] as GameObject;
            sortChildZ(gameObject.transform);
        }

        private static void sortChildZ(Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y, -0.0001f * i);
                sortChildZ(child);
            }
        }

        private static void setSorting(string name, bool setDeep, int deep)
        {
            for (int j = 0; j < Selection.objects.Length; j++)
            {
                GameObject gameObject = Selection.objects[j] as GameObject;
                GameObject itemPrefab = null;
                if (setDeep)
                {
                    string path = AssetDatabase.GetAssetPath(gameObject);
                    if (path != null)
                    {
                        itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        gameObject = Object.Instantiate(itemPrefab);
                    }
                }

                SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>(true);
                for (int i = 0; i < spriteRenderers.Length; i++)
                {
                    SpriteRenderer spriteRenderer = spriteRenderers[i];
                    spriteRenderer.sortingLayerName = name;
                    if (setDeep)
                    {
                        spriteRenderer.sortingOrder = deep;
                    }
                }

                ParticleSystemRenderer[] particleSystemRenderers = gameObject.GetComponentsInChildren<ParticleSystemRenderer>(true);
                for (int i = 0; i < particleSystemRenderers.Length; i++)
                {
                    ParticleSystemRenderer renderer = particleSystemRenderers[i];
                    renderer.sortingLayerName = name;
                    if (setDeep)
                    {
                        renderer.sortingOrder = deep;
                    }
                }

                if (itemPrefab != null)
                {
                    PrefabUtility.ReplacePrefab(gameObject, itemPrefab);
                    Object.DestroyImmediate(gameObject);
                }
            }
        }

        //CY改写setSorting()为setSorting2，增加 GameObject target 参数
        private static void setSorting2(GameObject target, string name, bool setDeep, int deep)
        {
            GameObject gameObject = target as GameObject;
            GameObject itemPrefab = null;
            if (setDeep)
            {
                string path = AssetDatabase.GetAssetPath(gameObject);
                if (path != null)
                {
                    itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    gameObject = Object.Instantiate(itemPrefab);
                }
            }

            SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                SpriteRenderer spriteRenderer = spriteRenderers[i];
                spriteRenderer.sortingLayerName = name;
                if (setDeep)
                {
                    spriteRenderer.sortingOrder = deep;
                }
            }

            ParticleSystemRenderer[] particleSystemRenderers = gameObject.GetComponentsInChildren<ParticleSystemRenderer>(true);
            for (int i = 0; i < particleSystemRenderers.Length; i++)
            {
                ParticleSystemRenderer renderer = particleSystemRenderers[i];
                renderer.sortingLayerName = name;
                if (setDeep)
                {
                    renderer.sortingOrder = deep;
                }
            }

            if (itemPrefab != null)
            {
                PrefabUtility.ReplacePrefab(gameObject, itemPrefab);
                Object.DestroyImmediate(gameObject);
            }
        }
    }
}