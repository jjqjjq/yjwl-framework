/*----------------------------------------------------------------
// 文件名：HyBannerItem.cs
// 文件功能描述：
// 
// 创建者：JJQ
// 时间：2021/4/1 16:26:20
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JQFramework.tUGUI
{

    public class JQBannerItem
    {
        private GameObject _itemGo;
        private int _index;

        public GameObject ItemGo => _itemGo;

        public int Index => _index;

        public JQBannerItem(int index, GameObject itemPrfab, Transform itemParentTrans)
        {
            _itemGo = GameObject.Instantiate(itemPrfab, itemParentTrans);
            _itemGo.SetActive(true);
            _itemGo.name = index.ToString();
            _index = index;
        }

        public void SetFirst()
        {
            Debug.Log("SetAsFirstSibling:"+_itemGo.name);
            _itemGo.transform.SetAsFirstSibling();
        }

        public void SetLast()
        {
            Debug.Log("SetAsLastSibling:" + _itemGo.name);
            _itemGo.transform.SetAsLastSibling();
        }

        public void dispose()
        {
            if (_itemGo != null)
            {
                UnityEngine.Object.Destroy(_itemGo);
                _itemGo = null;
            }
        }
    }
}
