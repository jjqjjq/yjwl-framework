/*----------------------------------------------------------------
// 文件名：BackgroundRawImage.cs
// 文件功能描述：
// 
// 创建者：JJQ
// 时间：2020/12/3 21:45:28
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JQFramework.tUGUI
{
    [RequireComponent(typeof(RawImage))]

    public class BackgroundRawImage : BackgroundUIObj
    {
        void Awake()
        {
            RawImage img = GetComponent<RawImage>();
            Texture texture = img.texture;
            updateFromTexture(texture);
        }

        public void updateFromTexture(Texture texture)
        {
            if (texture == null) return;
            RectTransform rectTransform = transform as RectTransform;
            BackgroundFit(texture.width, texture.height, rectTransform);
        }
    }
}
