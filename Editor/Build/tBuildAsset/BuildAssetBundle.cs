/*----------------------------------------------------------------
// 文件名：BuildAssetBundleAdd.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/10/26 17:12:53
//----------------------------------------------------------------*/

using System;
using JQEditor.Other;
using UnityEngine;

namespace JQEditor.Build
{
    public class BuildAssetBundle : BuildBase
    {
        private bool _isAll;

        public BuildAssetBundle(string name, string saveKey, bool isAll) : base(name, saveKey)
        {
            _isAll = isAll;
        }

        public override void build(Action endAction)
        {
            base.build(endAction);

            JQShaderPreprocessor.ClearShaderVariantCount();

            //打ab
            AssetBundleManifest assetBundleManifest = null;
            if (BuildAppInfo.ForceRebuild)
                BuildAssetBundleUtil.BuildAssetBundleAll();
            else
                BuildAssetBundleUtil.BuildAssetBundleAdd();
            JQShaderPreprocessor.printReport();
            exeEndAction();
        }
    }
}