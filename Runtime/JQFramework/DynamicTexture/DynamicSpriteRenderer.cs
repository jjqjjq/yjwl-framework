/*----------------------------------------------------------------
// 文件名：DynamicSpriteRenderer.cs
// 文件功能描述：
// 
// 创建者：JJQ
// 时间：2021/1/5 10:19:30
//----------------------------------------------------------------*/
using System;
using JQCore.tUtil;
using JQCore.tPool.Loader;
using JQCore.tPool.Manager;
#if UNITY_EDITOR
using UnityEditor;

#endif
using UnityEngine;


namespace JQCore.DynamicTexture
{
    [RequireComponent(typeof(SpriteRenderer))]

    public class DynamicSpriteRenderer:MonoBehaviour
    {
        public Sprite defaultSprite;
        public string spritePath;
        private AssetLoader _assetLoader;
        private SpriteRenderer _spriteRenderer;


        void Awake()
        {
            Load();
        }

//        void OnEnable()
//        {
//            Load();
//        }

        //        void OnDisable()
        //        {
        //            UnLoad();
        //        }

        void OnDestroy()
        {
            UnLoad();
        }
        void OnApplicationQuit()
        {
            UnLoad();
        }
        
        private void Load()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
            UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject);
            if (prefabStage != null) return;
#endif
            if (sprite != defaultSprite) return;
            if (_assetLoader != null) return;
            _assetLoader = AssetLoaderManager.getAssetLoader(spritePath);
            _assetLoader.AddCompleteListener(onLoadOneFinish);
            _assetLoader.Load();
        }

        private void onLoadOneFinish(AssetLoader loader)
        {
            _assetLoader.RemoveCompleteListener(onLoadOneFinish);
            Sprite loadSprite = loader.getSprite(0, 0, 0, 0);
            if (loadSprite != null)
            {
                sprite = loadSprite;
            }
        }

        private void UnLoad()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
            UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject);
            if (prefabStage != null) return;
#endif
            if (_assetLoader != null)
            {
                if (sprite != defaultSprite)
                {
                    _assetLoader.reduceUse();
                    sprite = defaultSprite;
                }
                _assetLoader.RemoveCompleteListener(onLoadOneFinish);
                _assetLoader = null;
            }
        }

        public Color color
        {
            get
            {
                if (_spriteRenderer == null)
                {
                    _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                }
                return _spriteRenderer.color;
            }
            set
            {
                if (_spriteRenderer == null)
                {
                    _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                }
                _spriteRenderer.color = value;
            }
        }

        public Sprite sprite
        {
            get
            {
                if (_spriteRenderer == null)
                {
                    _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                }
                return _spriteRenderer.sprite;
            }
            set
            {
                if (_spriteRenderer == null)
                {
                    _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                }
                _spriteRenderer.sprite = value;
            }
        }

#if UNITY_EDITOR

        public void restoreAllSprite()
        {
            GameObject root = gameObject.transform.root.gameObject;
            DynamicSpriteRenderer[] dynamicImages = root.GetComponentsInChildren<DynamicSpriteRenderer>(true);
            foreach (DynamicSpriteRenderer bigImage in dynamicImages)
            {
                bigImage.restoreSprite();
            }
        }
        
        private Sprite editDefaultSprite
        {
            get
            {
                if (defaultSprite == null)
                {
                    defaultSprite = AssetDatabase.LoadAssetAtPath($"Assets/{PathUtil.RES_FOLDER}/UI/CommonIcon/unknown.png", typeof(Sprite)) as Sprite;
                }
                return defaultSprite;
            }
        }


        public bool removeSprite()
        {
            if (sprite != editDefaultSprite && sprite != null)
            {
                spritePath = AssetDatabase.GetAssetPath(sprite).Replace($"Assets/{PathUtil.RES_FOLDER}/", "");
                sprite = editDefaultSprite;
                return true;
            }
            if (sprite == null)
            {
                sprite = editDefaultSprite;
                return true;
            }
            return false;
        }




        public bool restoreSprite()
        {
            if (sprite != editDefaultSprite)
            {
                string oldPath = AssetDatabase.GetAssetPath(sprite);
                if (oldPath.Equals(spritePath))//当前sprite和记录path是一样的
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(spritePath))
            {
                if (spritePath.Contains($"Assets/{PathUtil.RES_FOLDER}/"))
                {
                    spritePath = spritePath.Replace($"Assets/{PathUtil.RES_FOLDER}/", "");
                }

                Sprite readySprite = AssetDatabase.LoadAssetAtPath($"Assets/{PathUtil.RES_FOLDER}/" + spritePath, typeof(Sprite)) as Sprite;
                if (readySprite != null)
                {
                    sprite = readySprite;
                }
                else
                {
                    Debug.LogError("error:" + spritePath + "  name:" + name);
                }
            }

            return true;
        }
//
//        public void checkJpg()
//        {
//            //替换sprite
//            if (sprite != null)
//            {
//                string url = AssetDatabase.GetAssetPath(sprite);
//                if (url.Contains("png"))
//                {
//                    string jpgUrl = url.Replace("png", "jpg");
//                    Sprite jpgSprite = AssetDatabase.LoadAssetAtPath(jpgUrl, typeof(Sprite)) as Sprite;
//                    if (jpgSprite != null)
//                    {
//                        sprite = jpgSprite;
//                    }
//                }
//            }
//
//            //替换路径
//            if (!string.IsNullOrEmpty(spritePath))
//            {
//                if (spritePath.Contains("png"))
//                {
//                    string jpgUrl = spritePath.Replace("png", "jpg");
//                    if (!spritePath.Contains($"Assets/{PathUtil.RES_FOLDER}/"))
//                    {
//                        jpgUrl = $"Assets/{PathUtil.RES_FOLDER}/{jpgUrl}";
//                    }
//                    Sprite jpgSprite = AssetDatabase.LoadAssetAtPath(jpgUrl, typeof(Sprite)) as Sprite;
//                    if (jpgSprite != null)
//                    {
//                        spritePath = jpgUrl;
//                    }
//                }
//
//                if (spritePath.Contains($"Assets/{PathUtil.RES_FOLDER}/"))
//                {
//                    spritePath = spritePath.Replace($"Assets/{PathUtil.RES_FOLDER}/", "");
//                }
//            }
//        }
#endif
    }
}
