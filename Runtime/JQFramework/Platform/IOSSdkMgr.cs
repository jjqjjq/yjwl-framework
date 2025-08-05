using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
namespace JQFramework.Platform
{
    #if UNITY_IOS
    public class IOSSdkMgr:BaseSdkMgr, ISdkMgr
    {
        
        public IOSSdkMgr()
        {
        }

        
        
        public void logout()
        {
        }

    }
    #endif
}