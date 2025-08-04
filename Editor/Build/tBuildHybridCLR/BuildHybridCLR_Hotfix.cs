using System;

#if HYBRIDCLR
namespace JQEditor.Build
{
    public class BuildHybridCLR_Hotfix:BuildBase
    {
        public BuildHybridCLR_Hotfix(string name, string saveKey) : base(name, saveKey)
        {
            
        }
        
        public override void build(Action endAction)
        {
            base.build(endAction);
            BuildHybridCLRUtil.buildHotfix(true, exeEndAction);
        }
    }
}
#endif