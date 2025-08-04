using System;
using JQEditor.Util;

namespace JQEditor.Build
{
    public class BuildUpdateSvn : BuildBase
    {
        public BuildUpdateSvn(string name, string saveKey) : base(name, saveKey)
        {
        }

        public override void build(Action endAction)
        {
            base.build(endAction);
            SVNUtils.RevertAndUpdateSvnDirectory(exeEndAction);
        }
    }
}