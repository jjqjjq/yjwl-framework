/*----------------------------------------------------------------
// 文件名：NumerousImage.cs
// 文件功能描述：
// 
// 创建者：JJQ
// 时间：2020/10/9 17:52:16
//----------------------------------------------------------------*/

using UnityEngine;
using UnityEngine.UI;

namespace JQFramework.tUGUI
{
    [RequireComponent(typeof(SpriteRenderer))]

    public class NumerousSpriteRenderer:MonoBehaviour
    {
        public Sprite[] spriteArr;
        private SpriteRenderer _spriteRenderer;

        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public bool SetSprite(string name)
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
            for (int i = 0; i < spriteArr.Length; i++)
            {
                Sprite oneSprite = spriteArr[i];
                if (oneSprite.name == name)
                {
                    _spriteRenderer.sprite = oneSprite;
                    return true;
                }
            }
            return false;
        }
    }
}
