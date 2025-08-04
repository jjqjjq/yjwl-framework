using System;

#if HYBRIDCLR
namespace JQEditor.Build
{
    public class BuildHybridCLR_AOT:BuildBase
    {
        public BuildHybridCLR_AOT(string name, string saveKey) : base(name, saveKey)
        {
            
        }
        
        public override void build(Action endAction)
        {
            base.build(endAction);
            BuildHybridCLRUtil.buildAOT(true, exeEndAction);
        }
    }
}
#endif