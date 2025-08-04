/*----------------------------------------------------------------
// 文件名：HyBannerTog.cs
// 文件功能描述：
// 
// 创建者：JJQ
// 时间：2021/4/1 20:42:19
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JQFramework.tUGUI
{

    public class JQBannerTog
    {
        private GameObject _togGo;
        private GameObject _selectGo;
        public JQBannerTog(GameObject togPrfab, Transform togParentTrans)
        {
            _togGo = GameObject.Instantiate(togPrfab, togParentTrans);
            _togGo.SetActive(true);
            _selectGo = _togGo.transform.Find("pointSelect").gameObject;
            _selectGo.SetActive(false);
        }

        public void setSelect(bool select)
        {
            _selectGo.SetActive(select);
        }

        public void dispose()
        {
            if (_togGo != null)
            {
                UnityEngine.Object.Destroy(_togGo);
                _togGo = null;
                _selectGo = null;
            }
        }
    }
}
