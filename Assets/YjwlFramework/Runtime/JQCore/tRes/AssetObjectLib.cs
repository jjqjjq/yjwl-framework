using System.Collections.Generic;

using UnityEngine;
using Object = UnityEngine.Object;

namespace JQCore.tRes
{

    public class AssetObjectLib : MonoBehaviour
    {
        public Object[] assets;
        private Dictionary<string, Object> _objDic = new Dictionary<string, Object>();

        public void init()
        {
            if (_objDic.Count > 0) return;
            for (int i = 0; i < assets.Length; i++)
            {
                Object assetObj = assets[i];
                if (assetObj != null)
                {
                    _objDic[assetObj.name] = assetObj;
                }
            }
        }

        public Object GetAssetByIndex(string assetName)
        {
            // string assetName = LuaStrKeyMgr.Instance.getStrByIndex(assetIndex);
            return GetAsset(assetName);
        }

        public Object GetAsset(string assetName)
        {
            init();
            Object obj = null;
            _objDic.TryGetValue(assetName, out obj);
            return obj;
        }
    }
}
