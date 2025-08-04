using JQCore;
using JQCore.tLog;
using JQCore.tUtil;
using UnityEngine;

namespace JQFramework.tMVC
{
    public static class SafeAreaUtil
    {
        #region 屏幕自适应，刘海屏和旋转

        //刘海屏适配
        public static float safeAreaLeft = 0;
        public static float safeAreaRight = 0;
        public static float safeAreaTop = 0;
        public static float safeAreaBottom = 0;

        //屏幕当前旋转方向 "left"/"right"
        public static ScreenOrientation currScreenOrientation;

        public static (float, float, float, float) GetSafeAreaInfo()
        {
            Rect safeArea = Screen.safeArea;
            double safeAreaLeft = safeArea.x;
            double safeAreaRight = Screen.width - (safeArea.x + safeArea.width);
            double safeAreaBottom = safeArea.y;
            double safeAreaTop = Screen.height - (safeArea.y + safeArea.height);
            JQLog.Log(
                $"x:{safeArea.x}, y:{safeArea.y}, width:{safeArea.width}, height:{safeArea.height} screen:{Screen.width} {Screen.height}");

            float safeLeftRate = (float)(safeAreaLeft / Screen.width);
            float safeRightRate = (float)(safeAreaRight / Screen.width);
            float safeTopRate = (float)(safeAreaTop / Screen.height);
            float safeBottomRate = (float)(safeAreaBottom / Screen.height);
            return (safeLeftRate, safeRightRate, safeTopRate, safeBottomRate);
        }


        public static void AdaptPhone_UI(RectTransform panelFunctionRectTrans)
        {
            // JQLog.LogError($"safeAreaHeight:{safeAreaLeft} {safeAreaRight} {safeAreaTop} {safeAreaBottom}");
            
            panelFunctionRectTrans.SetLocalPos_EX((safeAreaLeft - safeAreaRight) * 0.5f,
                (safeAreaBottom - safeAreaTop) * 0.5f, 0);
            panelFunctionRectTrans.SetRectTransformAnchoredPosition3D_EX((safeAreaLeft - safeAreaRight) * 0.5f,
                (safeAreaBottom - safeAreaTop) * 0.5f, 0);
            panelFunctionRectTrans.SetSizeDelta_EX(-safeAreaLeft - safeAreaRight, -safeAreaTop - safeAreaBottom);
        }

        public static void SetDeviceSafeArea(float leftRate, float rightRate, float topRate, float bottomRate)
        {
            float width;
            float height;

            if (Sys.GAME_HEIGHT < Sys.GAME_WIDTH) //横屏
            {
                if (Screen.width < Screen.height) //系统信息与设置不一致
                {
                    width = Screen.height;
                    height = Screen.width;
                }
                else
                {
                    width = Screen.width;
                    height = Screen.height;
                }
            }
            else //竖屏
            {
                if (Screen.width > Screen.height) //系统信息与设置不一致
                {
                    width = Screen.height;
                    height = Screen.width;
                }
                else
                {
                    width = Screen.width;
                    height = Screen.height;
                }
            }


            float uiWidth = Sys.GAME_HEIGHT * width / height;
            float uiHeight = Sys.GAME_HEIGHT;
            safeAreaLeft = uiWidth * leftRate;
            safeAreaRight = uiWidth * rightRate;
            safeAreaTop = uiHeight * topRate;
            safeAreaBottom = uiHeight * bottomRate;
            JQLog.Log(
                $"SetDeviceSafeArea safeAreaLeft：{safeAreaLeft} safeAreaRight:{safeAreaRight} safeAreaTop:{safeAreaTop} safeAreaBottom:{safeAreaBottom}");
        }

        public static bool IsOrientationChanged()
        {
            return Screen.orientation != currScreenOrientation;
        }

        public static void ChangeOrientation()
        {
            switch (Screen.orientation)
            {
                case ScreenOrientation.LandscapeLeft:
                    Screen.orientation = ScreenOrientation.LandscapeRight;
                    break;
                case ScreenOrientation.LandscapeRight:
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                    break;
                case ScreenOrientation.Portrait:
                    Screen.orientation = ScreenOrientation.PortraitUpsideDown;
                    break;
                case ScreenOrientation.PortraitUpsideDown:
                    Screen.orientation = ScreenOrientation.Portrait;
                    break;
            }
        }

        #endregion
    }
}