using System;

namespace JQEditor.Build
{
    public class BuildRename: BuildBase
    {
        private bool _isInclude = false;
        
        public BuildRename(string name, string saveKey, bool isInclude) : base(name, saveKey)
        {
            _isInclude = isInclude;
        }
        
        public override void build(Action endAction)
        {
            base.build(endAction);
            BuildApkUtil.RenameResources(_isInclude, exeEndAction);
        }
    }
}