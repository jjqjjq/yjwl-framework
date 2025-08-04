using System;

namespace JQEditor.Build
{
    public class BuildUploadCdn : BuildBase
    {
        public BuildUploadCdn(string name, string saveKey) : base(name, saveKey)
        {
        }

        public override void build(Action endAction)
        {
            base.build(endAction);
#if SDK_WEIXIN      
            BuildApkUtil.uploadToCDN(exeEndAction);
#endif
        }
    }
}