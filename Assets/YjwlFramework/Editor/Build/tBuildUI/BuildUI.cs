/*----------------------------------------------------------------
// 文件名：BuildUI.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/9/12 19:41:28
//----------------------------------------------------------------*/

using System;

namespace JQEditor.Build
{
    public class BuildUI : BuildBase
    {
        public BuildUI(string name, string saveKey) : base(name, saveKey)
        {
        }

        public override void build(Action endAction)
        {
            base.build(endAction);
            BuildUIUtil.build(true, exeEndAction);
        }
    }
}