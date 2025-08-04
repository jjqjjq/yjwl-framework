using System;
using JQEditor.MainSubPackage;

namespace JQEditor.Build
{
    public class BuildCopyAB : BuildBase
    {
        public BuildCopyAB(string name, string saveKey) : base(name, saveKey)
        {
        }

        public override void build(Action endAction)
        {
            base.build(endAction);
            MainSubPackageUtil.CopyMainPackageToStreamAsseting(null);
        }
    }
}