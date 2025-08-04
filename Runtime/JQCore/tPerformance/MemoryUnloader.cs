/*----------------------------------------------------------------
// 文件名：AssetBundleUnloader.cs
// 文件功能描述：
// 
// 创建者：JJQ
// 时间：2021/4/14 11:01:04
//----------------------------------------------------------------*/
using System;
using System.Collections;
using JQCore.tLog;
using JQCore.tPerformance;
using JQCore.tPool.Manager;
using JQCore.tYooAssets;
using UnityEngine;

namespace JQCore
{

    public class MemoryUnloader: MonoBehaviour
    {
        public static MemoryUnloader instance;

        private bool _isWaitingUnload = false;
        private Action _callbackLuaFun = null;
        private bool _isUnloading = false;

        void Awake()
        {
            instance = this;
        }

        void Update()
        {
            if (!_isUnloading && _isWaitingUnload)
            {
                _isWaitingUnload = false;
                _isUnloading = true;
                StartCoroutine(doUnload());
            }
        }

        public void addUnloadTask(Action callbackLuaFun)
        {
            if (_isUnloading) return;
            _isWaitingUnload = true;
            _callbackLuaFun = callbackLuaFun;
        }


        private IEnumerator doUnload()
        {
            JQLog.Log("[资源清理]");
            PrefabLoaderPoolManager.ClearAllNotInUse(); //清理外部引用计数为0的PrefabPool
            AssetLoaderManager.ClearAllUnUse();
            YooAssetMgr.ClearAllUnUse();
            YooAssetMgr.UnloadAssets();
            yield return null;
            ManagedHeapUtil.printMemory();
            JQLog.Log("[资源清理]end");
            if (_callbackLuaFun!=null)
            {
                JQLog.Log("UnloadMemoryFinish");
                _callbackLuaFun();
                // HyXLua.UnloadMemoryFinish(_callbackLuaFun);
                _callbackLuaFun = null;
            }
            _isUnloading = false;
        }
    }
}
