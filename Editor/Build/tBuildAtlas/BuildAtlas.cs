/*----------------------------------------------------------------
// 文件名：BuildAtlas.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/8/31 17:55:46
//----------------------------------------------------------------*/

using System;

namespace JQEditor.Build
{
    public class BuildAtlas : BuildBase
    {
        public BuildAtlas(string name, string saveKey) : base(name, saveKey)
        {
        }

        public override void build(Action endAction)
        {
            base.build(endAction);
            BuildAtlasUtil.build(true, exeEndAction);
        }
    }
}