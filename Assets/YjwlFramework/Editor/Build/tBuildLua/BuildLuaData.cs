/*----------------------------------------------------------------
// 文件名：BuildLuaData.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/10/16 14:19:23
//----------------------------------------------------------------*/

using System;

namespace JQEditor.Build
{
    public class BuildLuaData : BuildBase
    {
        public BuildLuaData(string name, string saveKey) : base(name, saveKey)
        {
        }

        public override void build(Action endAction)
        {
            base.build(endAction);
            BuildLuaDataUtil.build(exeEndAction);
//            BuildLuacUtil.buildLua(exeEndAction);
        }
    }
}