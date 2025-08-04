/*----------------------------------------------------------------
// 文件名：BuildApk.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/8/31 18:02:58
//----------------------------------------------------------------*/

using System;
using JQEditor.MainSubPackage;

namespace JQEditor.Build
{
    public class BuildApk : BuildBase
    {
        public BuildApk(string name, string saveKey) : base(name, saveKey)
        {
        }

        public override void build(Action endAction)
        {
            base.build(endAction);
            MainSubPackageUtil.CopyMainPackageToStreamAsseting(null);
            BuildApkUtil.build(true, exeEndAction);
        }
    }
}