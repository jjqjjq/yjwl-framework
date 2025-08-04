/*----------------------------------------------------------------
// 文件名：BuildUIUtil.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/10/20 14:34:25
//----------------------------------------------------------------*/

using System;
using UnityEngine;

namespace JQEditor.Build
{
    public static class BuildUIUtil
    {
        public static void build(bool showUI, Action exeEndAction = null)
        {
            exeEndAction?.Invoke();
//            CheckCommonTools.SearchAndDo("UI批处理", $"Assets/{PathUtil.RES_FOLDER}/UI/Views", setAllBigImg, null, exeEndAction, showUI);
        }


        private static bool setAllBigImg(string assetPath, GameObject cloneGo, object obj1)
        {
            var change = false;

            return change;
        }
    }
}