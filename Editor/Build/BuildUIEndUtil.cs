/*----------------------------------------------------------------
// 文件名：BuildUIEndUtil.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/10/20 14:38:30
//----------------------------------------------------------------*/

using System;
using JQCore.DynamicTexture;
using UnityEngine;

namespace JQEditor.Build
{
    public static class BuildUIEndUtil
    {
        public static void build(string name, string folderName, Action exeEndAction)
        {
            exeEndAction?.Invoke();
            //            CheckCommonTools.SearchAndDo(name, $"Assets/{folderName}/UI/Views", setAllBigImg, null, exeEndAction);
        }


        private static bool setAllBigImg(string assetPath, GameObject cloneGo, object obj1)
        {
            var change = false;
            DynamicImage[] images = cloneGo.GetComponentsInChildren<DynamicImage>(true);
            for (var i = 0; i < images.Length; i++)
            {
                DynamicImage image = images[i];
//                image.checkJpg();
                bool success = image.restoreSprite();
                if (success) change = true;
            }

            return change;
        }
    }
}