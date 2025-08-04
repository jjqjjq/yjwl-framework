using System.Collections.Generic;
using UnityEngine;

namespace JQCore.tUtil
{

    public class Math3DUtil
    {
        /// <summary>
        /// 计算两矩形重叠区域
        /// </summary>
        /// <param name="aRect"></param>
        /// <param name="bRect"></param>
        /// <returns></returns>
        public static Rect GetOverLapRect(Rect aRect, Rect bRect)
        {
            List<float> xList = new List<float>();
            xList.Add(aRect.x);
            xList.Add(aRect.x + aRect.width);
            xList.Add(bRect.x);
            xList.Add(bRect.x + bRect.width);

            List<float> yList = new List<float>();
            yList.Add(aRect.y);
            yList.Add(aRect.y + aRect.height);
            yList.Add(bRect.y);
            yList.Add(bRect.y + bRect.height);

            xList.Sort((a, b) => a.CompareTo(b));
            yList.Sort((a, b) => a.CompareTo(b));
            float[] xArr = xList.ToArray();
            float[] yArr = yList.ToArray();
            return new Rect(xArr[1], yArr[1], xArr[2]-xArr[1], yArr[2]-yArr[1]);
        }

        

        /// <summary>
        /// 计算矩形中心
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector2 GetRectCenter(Rect rect)
        {
            float x = rect.x + rect.width / 2f;
            float y = rect.y + rect.height / 2f;
            return new Vector2(x, y);
        }

        public static Vector2 GetRectRandom(Rect rect)
        {
            return new Vector2(0f, 0f);
        }
    }
}
