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
#if UNITY_WEBGL      
            BuildApkUtil.uploadToCDN(exeEndAction);
#endif
        }
    }
}