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
    [RequireComponent(typeof(Image))]

    public class NumerousImage:MonoBehaviour
    {
        public Sprite[] spriteArr;
        private Image _image;
        private BackgroundImage _backgroundImage;

        void Awake()
        {
            _image = GetComponent<Image>();
            _backgroundImage = GetComponent<BackgroundImage>();
        }

        public bool SetSprite(string name)
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
            }
            for (int i = 0; i < spriteArr.Length; i++)
            {
                Sprite oneSprite = spriteArr[i];
                if (oneSprite.name == name)
                {
                    _image.sprite = oneSprite;
                    _image.SetNativeSize();
                    if (_backgroundImage != null)
                    {
                        _backgroundImage.updateFromTexture(oneSprite);
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
