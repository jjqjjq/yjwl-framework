/*----------------------------------------------------------------
// 文件名：AssetLoader.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/9/8 23:05:34
//----------------------------------------------------------------*/

using System;
using JQCore.tEvent;
using JQCore.tYooAssets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.Video;
using YooAsset;
using Object = UnityEngine.Object;

namespace JQCore.tPool.Loader
{
    public class AssetLoader
    {
        public const string COMPLETE = "AssetLoaderComplete";

        // protected LuaEventDispatcher _luaEventDispatcher = new LuaEventDispatcher();
        // private IrfResource _resource;
        private YooAssetAssetInfo _assetInfo;

        protected JQEventDispatcher _irfEventDispatcher = new();

        private bool _isLoading;

        // private Object[] _allObj;
        private SceneOperationHandle _sceneOperationHandle;
        private Sprite _textureSprite;
        protected string _url;

        public string Url => _url;

        private Object mainObject { get; set; }

        public void SetUrl(string url)
        {
            _url = url;
            _assetInfo = YooAssetMgr.GetAsset(_url);
        }

        public bool canClear()
        {
            if (_isLoading) return false;
            if (_assetInfo == null) return false;
            return _assetInfo.canClear();
        }

        public void reset()
        {
            _irfEventDispatcher.EventDispose();
            // _luaEventDispatcher.EventDispose();
            _assetInfo = null;
            // _allObj = null;
            mainObject = null;
            _sceneOperationHandle = null;
            _isLoading = false;
            _textureSprite = null;
            _url = null;
        }

        public void LoadScene()
        {
            //            HyDebug.Log($"load prefab:"+_key);

            if (_sceneOperationHandle != null) //加载完毕
            {
                TriggerEvent(COMPLETE, this);
                return;
            }

            if (_isLoading) //正在加载
                return;


            _isLoading = true;
            // IrfResourcesMgr.LoadResource(_url, OnLoadFinish);
            YooAssetMgr.LoadSceneAsync(_url, LoadSceneMode.Single, OnSceneLoadFinish);
        }

        public SceneOperationHandle GetSceneOperationHandle()
        {
            _assetInfo.addUse();
            return _sceneOperationHandle;
        }

        private void OnSceneLoadFinish(SceneOperationHandle resource)
        {
            // _allObj = resource.AllObjs;
            _sceneOperationHandle = resource;
            _isLoading = false;
            TriggerEvent(COMPLETE, this);
        }

        public void Load()
        {
            //            HyDebug.Log($"load prefab:"+_key);

            if (mainObject != null) //加载完毕
            {
                TriggerEvent(COMPLETE, this);
                return;
            }

            if (_isLoading) //正在加载
                return;

            _isLoading = true;
            // IrfResourcesMgr.LoadResource(_url, OnLoadFinish);
            YooAssetMgr.LoadAssetAsync(_url, OnLoadFinish);
        }


        private void OnLoadFinish(AssetOperationHandle handle)
        {
            // _allObj = resource.AllObjs;
            _assetInfo.SetAssetOperationHandle(handle);
            mainObject = handle.AssetObject;
            _textureSprite = null;
            _isLoading = false;
            TriggerEvent(COMPLETE, this);
        }
        // private void OnLoadFinish(IrfResource resource)
        // {
        //     _resource = resource;
        //     _allObj = resource.AllObjs;
        //     _mainObj = resource.MainObject;
        //     _textureSprite = null;
        //     _isLoading = false;
        //     TriggerEvent(COMPLETE, this);
        //
        // }

        public void TriggerEvent(string eventType, AssetLoader t)
        {
            _irfEventDispatcher.TriggerEvent(eventType, t, true);
            // _luaEventDispatcher.TriggerEvent(eventType, t);
            // _luaEventDispatcher.EventDispose(eventType);
        }

        public void AddCompleteListener(Action<AssetLoader> handler)
        {
            _irfEventDispatcher.AddEventListener(COMPLETE, handler);
        }

        public void RemoveCompleteListener(Action<AssetLoader> handler)
        {
            _irfEventDispatcher.RemoveEventListener(COMPLETE, handler);
        }


        public void reduceUse()
        {
            _assetInfo.reduceUse();
        }

        // public Object[] getObjects()
        // {
        //     _resource.addUse();
        //     return _allObj;
        // }
        //
        // public string[] getObjsNames()
        // {
        //     return _resource.AllNames;
        // }

        public SpriteAtlas getSpriteAtlas()
        {
            if (mainObject is SpriteAtlas)
            {
                _assetInfo.addUse();
                return mainObject as SpriteAtlas;
            }
            return null;
        }

        public Sprite getSprite(float leftBorder = 0, float bottomBorder = 0, float rightBorder = 0, float topBorder = 0)
        {
            if (mainObject is Sprite)
            {
                _assetInfo.addUse();
                return mainObject as Sprite;
            }

            if (mainObject is Texture2D)
            {
                _assetInfo.addUse();
                if (_textureSprite == null)
                {
                    var texture2 = mainObject as Texture2D;
//                    HyDebug.LogError($"url:{_url}  width:{texture2.width}  height{texture2.height}");
                    _textureSprite = Sprite.Create(texture2, new Rect(0, 0, texture2.width, texture2.height), new Vector2(0.5f, 0.5f), 100, 1, SpriteMeshType.Tight, new Vector4(leftBorder, bottomBorder, rightBorder, topBorder));
                }

                return _textureSprite;
            }

            return null;
        }

        public Texture getTexture()
        {
            if (mainObject is Texture)
            {
                _assetInfo.addUse();
                return mainObject as Texture;
            }

            return null;
        }

        public ShaderVariantCollection getShaderVariantCollection()
        {
            if (mainObject is ShaderVariantCollection)
            {
                _assetInfo.addUse();
                return mainObject as ShaderVariantCollection;
            }

            return null;
        }

        public TextAsset getText()
        {
            if (mainObject is TextAsset)
            {
                _assetInfo.addUse();
                return mainObject as TextAsset;
            }

            return null;
        }

        public VideoClip getVideoClip()
        {
            if (mainObject is VideoClip)
            {
                _assetInfo.addUse();
                return mainObject as VideoClip;
            }

            return null;
        }

        public string getTextStr()
        {
            if (mainObject is TextAsset)
            {
                _assetInfo.addUse();
                return (mainObject as TextAsset).text;
            }

            return null;
        }

        public AudioClip getAudioClip()
        {
            if (mainObject is AudioClip)
            {
                _assetInfo.addUse();
                return mainObject as AudioClip;
            }

            return null;
        }

        public object getObj()
        {
            if (mainObject != null)
            {
                _assetInfo.addUse();
                return mainObject;
            }

            return null;
        }
    }
}