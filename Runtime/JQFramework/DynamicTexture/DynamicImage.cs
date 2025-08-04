/*----------------------------------------------------------------
// 文件名：DynamicImage.cs
// 文件功能描述：
// 
// 创建者：JJQ
// 时间：2020/9/8 19:33:31
//----------------------------------------------------------------*/


using JQCore.tUtil;
using JQFramework.tUGUI;
using JQCore.tPool.Loader;
using JQCore.tPool.Manager;
#if UNITY_EDITOR
using JQCore.tLog;
using UnityEditor;

#endif
using UnityEngine;
using UnityEngine.UI;


namespace JQCore.DynamicTexture
{

    public class DynamicImage : Image
    {
        public Sprite defaultSprite;
        public string spritePath;
        public float leftBorder;
        public float bottomBorder;
        public float rightBorder;
        public float topBorder;
        private AssetLoader _assetLoader;
        private bool _isLoadFinish;

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
            if (sprite != editDefaultSprite) return;
#endif
            if (_assetLoader != null) return;
            if (string.IsNullOrEmpty(spritePath)) return;
            _assetLoader = AssetLoaderManager.getAssetLoader(spritePath);
            _assetLoader.AddCompleteListener(onLoadOneFinish);
            _isLoadFinish = false;
            // JQLog.LogError($"DynamicImage:{gameObject.name}");
            _assetLoader.Load();
        }

        private void onLoadOneFinish(AssetLoader loader)
        {
            _isLoadFinish = true;
            _assetLoader.RemoveCompleteListener(onLoadOneFinish);
            Sprite loadSprite = loader.getSprite(leftBorder, bottomBorder, rightBorder, topBorder);
            if (loadSprite != null)
            {
                sprite = loadSprite;
            }
            BackgroundImage backgroundImage = GetComponent<BackgroundImage>();
            backgroundImage?.updateFromTexture(sprite);
        }

        private void UnLoad()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
            UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject);
            if (prefabStage != null) return;
            if (sprite != editDefaultSprite)
            {
                sprite = editDefaultSprite;
            }
#endif
            if (_assetLoader != null)
            {
                if (_isLoadFinish)
                {
                    _assetLoader.reduceUse();
                }
                _assetLoader.RemoveCompleteListener(onLoadOneFinish);
                _assetLoader = null;
            }
        }


#if UNITY_EDITOR

        public void removeAllSprite()
        {
            GameObject root = gameObject.transform.root.gameObject;
            DynamicImage[] dynamicImages = root.GetComponentsInChildren<DynamicImage>(true);
            foreach (DynamicImage bigImage in dynamicImages)
            {
                bigImage.removeSprite();
            }
        }



        public void restoreAllSprite()
        {
            GameObject root = gameObject.transform.root.gameObject;
            DynamicImage[] dynamicImages = root.GetComponentsInChildren<DynamicImage>(true);
            foreach (DynamicImage bigImage in dynamicImages)
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
                    defaultSprite = AssetDatabase.LoadAssetAtPath($"Assets/{PathUtil.RES_FOLDER}/UI/ProgramLoad/CommonIcon/unknown.png", typeof(Sprite)) as Sprite;
                    if (defaultSprite == null)
                    {
                        JQLog.LogError("找不到默认图片");
                    }
                }
                return defaultSprite;
            }
        }



        public bool removeSprite()
        {
            if (sprite != editDefaultSprite && sprite != null)
            {
                spritePath = AssetDatabase.GetAssetPath(sprite).Replace($"Assets/{PathUtil.RES_FOLDER}/", "");
                leftBorder = sprite.border.x;
                bottomBorder = sprite.border.y;
                rightBorder = sprite.border.z;
                topBorder = sprite.border.w;
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
        //                        leftBorder = sprite.border.x;
        //                        bottomBorder = sprite.border.y;
        //                        rightBorder = sprite.border.z;
        //                        topBorder = sprite.border.w;
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
