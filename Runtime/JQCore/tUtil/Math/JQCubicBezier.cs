using UnityEngine;

namespace JQCore.tUtil
{
    public static class JQCubicBezier
    {

        public static (Vector3, Vector3) CaculatePerpendicularPoint(Vector3 startPos, Vector3 endPos, float perpendicularDistance)
        {
            Vector3 posA = startPos;
            Vector3 posB = endPos;
                
            // 计算AB方向向量
            Vector3 direction = (posB - posA).normalized;
        
            // 计算垂直向量（在XZ平面上）
            Vector3 perpendicular = new Vector3(direction.y, -direction.x, 0);
        
            // 计算垂直点
            Vector3 perpendicularPos1 = posA + perpendicular * perpendicularDistance;
            Vector3 perpendicularPos2 = posA - perpendicular * perpendicularDistance;
                
            return (perpendicularPos1, perpendicularPos2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0">A点</param>
        /// <param name="p1">A点Handle</param>
        /// <param name="p2">B点</param>
        /// <param name="p3">B点Handle</param>
        /// <param name="t">0-1进度</param>
        /// <returns></returns>
        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            var oneMinusT = 1f - t;
            return oneMinusT * oneMinusT * oneMinusT * p0 +
                   3f * oneMinusT * oneMinusT * t * p1 +
                   3f * oneMinusT * t * t * p3 +
                   t * t * t * p2;
        }
    }
}