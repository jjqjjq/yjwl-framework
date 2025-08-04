/*----------------------------------------------------------------
// 文件名：BackgroundSprite.cs
// 文件功能描述：
// 
// 创建者：JJQ
// 时间：2021/2/5 10:54:31
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JQFramework.tUGUI
{

    public class BackgroundSprite : BackgroundUIObj
    {
        void Awake()
        {
            updateFromTexture();
        }

        public void updateFromTexture()
        {
            RectTransform rectTransform = transform as RectTransform;
            BackgroundFit(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y, rectTransform);
        }
    }
}
