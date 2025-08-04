using System;

namespace JQEditor.Build
{
    public class BuildMainSubPackage : BuildBase
    {
        public BuildMainSubPackage(string name, string saveKey) : base(name, saveKey)
        {
        }

        public override void build(Action endAction)
        {
            base.build(endAction);
            //设置资源AB
            // MainSubPackageUtil.CopyMainPackageToStreamAsseting(null);
        }
    }
}