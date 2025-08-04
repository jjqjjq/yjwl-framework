using JQCore.tLog;
using YooAsset;

namespace JQCore.tYooAssets
{
    public class YooAssetAssetInfo
    {
        private AssetOperationHandle _assetOperationHandle;

        private readonly string _mainAssetPath;

        public YooAssetAssetInfo(string mainAssetPath)
        {
            // JQLog.Log("YooAssetAssetInfo init:" + mainAssetPath);
            _mainAssetPath = mainAssetPath;
        }

        public int UseCount { get; private set; }

        public bool DontRelease { get; private set; }

        public void SetAssetOperationHandle(AssetOperationHandle assetOperationHandle)
        {
            _assetOperationHandle = assetOperationHandle;
        }

        public void releaseAsset()
        {
            if (_assetOperationHandle != null) _assetOperationHandle.Release();

            _assetOperationHandle = null;
        }

        public void addUse()
        {
            UseCount++;
            // if (_mainAssetPath == "Scene/1002/scene_1002.scene")
            // {
            //     JQLog.LogError("addUse：" + UseCount);
            // }
        }


        public void reduceUse()
        {
            UseCount--;
            if (UseCount < 0) JQLog.LogError("减太多次了把？！：" + _mainAssetPath);
            // if (_mainAssetPath == "Scene/1002/scene_1002.scene")
            // {
            //     JQLog.LogError("reduceUse：" + UseCount);
            // }
        }

        public bool canClear()
        {
            if (DontRelease) return false;
            if (UseCount > 0) return false;
            return true;
        }

        public void setDontRelease()
        {
            DontRelease = true;
        }
    }
}