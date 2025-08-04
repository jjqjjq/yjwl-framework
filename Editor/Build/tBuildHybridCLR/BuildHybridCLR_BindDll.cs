using System;

#if HYBRIDCLR
namespace JQEditor.Build
{
    public class BuildHybridCLR_BindDll:BuildBase
    {
        public BuildHybridCLR_BindDll(string name, string saveKey) : base(name, saveKey)
        {
            
        }
        
        public override void build(Action endAction)
        {
            base.build(endAction);
            BuildHybridCLRUtil.copyAndBindAllDllToLib(true, exeEndAction);
        }
    }
}
#endif