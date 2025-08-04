/*----------------------------------------------------------------
// 文件名：BuildAtlasPacker.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/11/6 14:18:40
//----------------------------------------------------------------*/

using System;

namespace JQEditor.Build
{
    public class BuildAtlasPacker : BuildBase
    {
        public BuildAtlasPacker(string name, string saveKey) : base(name, saveKey)
        {
        }

        public override void build(Action endAction)
        {
            base.build(endAction);
            BuildAtlasUtil.packer(true, exeEndAction);
        }
    }
}