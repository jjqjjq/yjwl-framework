using UnityEngine;

namespace JQFramework.tUGUI
{
    public class BackgroundUIObj:MonoBehaviour
    {
        private static float resolutionW = -1;
        private static float resolutionH = -1;
        private static float screenRate = 1f * Screen.width / Screen.height;
    
        protected static void BackgroundFit(float width, float height, RectTransform rectTransform)
        {
            screenRate = 1f * Screen.width / Screen.height;
            if (resolutionW == -1)
            {
                RectTransform canvasRectTransform = GameObject.Find("Canvas").transform as RectTransform;
                resolutionW = canvasRectTransform.sizeDelta.x;
                resolutionH = canvasRectTransform.sizeDelta.y;
            }
            float imgRate = width / height;
            
            if (imgRate < screenRate)
            {
                float currImgW = screenRate * resolutionH;
                float rightImgH = currImgW / imgRate;
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(1f, 0.5f);
                rectTransform.sizeDelta = new Vector2(0, rightImgH);
            }
            else
            {
                float currImgH = resolutionW / screenRate;
                float rightImgW = currImgH * imgRate;
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 1f);
                rectTransform.sizeDelta = new Vector2(rightImgW, 0);
            }
        }
        
    }
}