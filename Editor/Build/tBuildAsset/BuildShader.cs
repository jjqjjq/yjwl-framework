using System;

namespace JQEditor.Build
{
    public class BuildShader : BuildBase
    {
        public BuildShader(string name, string saveKey) : base(name, saveKey)
        {
        }

        public override void build(Action endAction)
        {
            base.build(endAction);
            //设置资源AB
            BuildAssetBundleUtil.CollectSVC(exeEndAction);
        }
    }
}