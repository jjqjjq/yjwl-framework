/*----------------------------------------------------------------
// 文件名：BgCanvasScaler.cs
// 文件功能描述：
// 
// 创建者：JJQ
// 时间：2020/12/3 19:55:11
//----------------------------------------------------------------*/

using UnityEngine;
using UnityEngine.UI;

namespace JQFramework.tUGUI
{

    public class BackgroundImage : BackgroundUIObj
    {
        void Awake()
        {
            Image img = GetComponent<Image>();
            updateFromTexture(img.sprite);
        }

        public void updateFromTexture(Sprite sprite)
        {
            if (sprite == null) return;
            RectTransform rectTransform = transform as RectTransform;
            BackgroundFit(sprite.bounds.size.x, sprite.bounds.size.y, rectTransform);
        }
    }



}
