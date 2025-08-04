using System;
using JQCore.tPool.Loader;
using UnityEngine;
using UnityEngine.UI;

namespace JQCore.DynamicTexture
{
    public class DynamicImageLoader
    {
        private IDynamicTextureOwner _dynamicTextureOwner;
        private Image _image;
        // public Color color;
        private Sprite _sprite;
        private Action<Image> _callbackFunc;
        private AssetLoader _assetLoader;
        private string _pathUrl;
        
        public DynamicImageLoader(IDynamicTextureOwner dynamicTextureOwner, Image image)
        {
            _dynamicTextureOwner = dynamicTextureOwner;
            _image = image;
            // this.color = image.color;
        }
        
        public IDynamicTextureOwner DynamicTextureOwner
        {
            get { return _dynamicTextureOwner; }
        }

        public Image Image
        {
            get { return _image; }
        }
        
        public void loadDynamicImageFullPath(string pathUrl, Action<Image> callbackFunc)
        {
            unloadDynamicImage();
            _pathUrl = pathUrl;
            _callbackFunc = callbackFunc;
            _image.enabled = false;
            AssetLoaderUtil.LoadAsset(pathUrl, onLoadDynamicImage);
        }

        public void loadDynamicImage(string fileName, Action<Image> callbackFunc)
        {
            loadDynamicImageFullPath("UI/ProgramLoad/UITextureLoaderCode/" + fileName, callbackFunc);
        }

        private void onLoadDynamicImage(AssetLoader assetLoader)
        {
            if (_image == null)
            {
                return;
            }
            _assetLoader = assetLoader;
            Sprite sprite = assetLoader.getSprite();
            if (sprite != null)
            {
                _sprite = sprite;
                _image.sprite = sprite;
                _image.enabled = true;
            }
            _callbackFunc?.Invoke(_image);
            DynamicImageLoaderMgr.Instance.removeDynamicLoader(_image);

        }

        private void unloadDynamicImage()
        {
            if (_assetLoader != null)
            {
                if (_sprite != null)
                {
                    _assetLoader.reduceUse();
                    _sprite = null;
                }

                _assetLoader = null;
            }

            if (_pathUrl != null)
            {
                AssetLoaderUtil.CancelLoadAsset(_pathUrl, onLoadDynamicImage);
                _pathUrl = null;
            }
        }

        public void dispose()
        {
            unloadDynamicImage();
            _dynamicTextureOwner = null;
            _image = null;
            _callbackFunc = null;
            _sprite = null;
        }
        
    }
}