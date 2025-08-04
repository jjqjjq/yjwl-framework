/*----------------------------------------------------------------
// 文件名：BuildUIEnd.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/9/14 10:15:23
//----------------------------------------------------------------*/

using System;
using JQCore.tUtil;

namespace JQEditor.Build
{
    public class BuildUIEnd : BuildBase
    {
        public BuildUIEnd(string name, string saveKey) : base(name, saveKey)
        {
        }


        public override void build(Action endAction)
        {
            base.build(endAction);
            BuildUIEndUtil.build(_name, PathUtil.RES_FOLDER, exeEndAction);
        }
    }
}