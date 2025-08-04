/*----------------------------------------------------------------
// 文件名：BuildLua.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2020/9/16 15:05:49
//----------------------------------------------------------------*/

using System;
using System.ComponentModel;
using System.Security.Cryptography;

namespace JQEditor.Build
{
    public class BuildLua : BuildBase
    {
        private static ICryptoTransform cryp;

        /// <summary>
        ///     必需的设计器变量。
        /// </summary>
        private IContainer components = null;

        public BuildLua(string name, string saveKey) : base(name, saveKey)
        {
        }


        public override void build(Action endAction)
        {
            base.build(endAction);
            BuildLuaUtil.build(true, exeEndAction);
        }
    }
}