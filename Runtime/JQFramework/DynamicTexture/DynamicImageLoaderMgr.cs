using System;
using System.Collections.Generic;
using JQCore.tSingleton;
using UnityEngine.UI;

namespace JQCore.DynamicTexture
{
    public class DynamicImageLoaderMgr : JQSingleton<DynamicImageLoaderMgr>
    {
        private List<DynamicImageLoader> _dynamicLoaderList = new List<DynamicImageLoader>();

        public override void Dispose()
        {
            for (int i = 0; i < _dynamicLoaderList.Count; i++)
            {
                DynamicImageLoader dynamicImageLoader = _dynamicLoaderList[i];
                dynamicImageLoader.dispose();
            }
            _dynamicLoaderList.Clear();
        }

        private DynamicImageLoader getDynamicLoader(Image image)
        {
            for (int i = 0; i < _dynamicLoaderList.Count; i++)
            {
                DynamicImageLoader dynamicImageLoader = _dynamicLoaderList[i];
                if (dynamicImageLoader.Image == image)
                {
                    return dynamicImageLoader;
                }
            }

            return null;
        }

        /// <summary>
        /// 动态加载图片
        /// </summary>
        /// <param name="image"></param>
        /// <param name="pathUrl"></param>
        /// <param name="callbackFunc"></param>
        public void addDynamicLoaderFullPath(IDynamicTextureOwner dynamicTextureOwner, Image image, string pathUrl, Action<Image> callbackFunc)
        {
            DynamicImageLoader dynamicImageLoader = getDynamicLoader(image);
            if (dynamicImageLoader == null)
            {
                dynamicImageLoader = new DynamicImageLoader(dynamicTextureOwner, image);
                _dynamicLoaderList.Add(dynamicImageLoader);
            }

            dynamicImageLoader.loadDynamicImageFullPath(pathUrl, callbackFunc);
        }

        public void addDynamicLoader(IDynamicTextureOwner dynamicTextureOwner, Image image, string fileName, Action<Image> callbackFunc)
        {
            DynamicImageLoader dynamicImageLoader = getDynamicLoader(image);
            if (dynamicImageLoader == null)
            {
                dynamicImageLoader = new DynamicImageLoader(dynamicTextureOwner, image);
                _dynamicLoaderList.Add(dynamicImageLoader);
            }

            dynamicImageLoader.loadDynamicImage(fileName, callbackFunc);
        }

        public void removeDynamicLoader(Image image)
        {
            DynamicImageLoader dynamicImageLoader = getDynamicLoader(image);
            if (dynamicImageLoader != null)
            {
                dynamicImageLoader.dispose();
                _dynamicLoaderList.Remove(dynamicImageLoader);
            }
        }

        public void removeDynamicLoaderByParent(IDynamicTextureOwner dynamicTextureOwner)
        {
            List<DynamicImageLoader> removeList = new List<DynamicImageLoader>();

            for (int i = _dynamicLoaderList.Count - 1; i >= 0; i--)
            {
                DynamicImageLoader dynamicImageLoader = _dynamicLoaderList[i];
                if (dynamicImageLoader.DynamicTextureOwner == dynamicTextureOwner)
                {
                    removeList.Add(dynamicImageLoader);
                }
            }
            
            for (int i = 0; i < removeList.Count; i++)
            {
                DynamicImageLoader dynamicImageLoader = removeList[i];
                dynamicImageLoader.dispose();
                _dynamicLoaderList.Remove(dynamicImageLoader);
            }
        }
    }
}