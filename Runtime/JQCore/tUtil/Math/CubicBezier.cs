using System.Collections.Generic;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine;

namespace JQCore.tUtil
{
    public static class CubicBezier
    {
        public static List<Vector3> GetPoints(List<CubicBezierControlPoint> paths, int segments)
        {
            segments = Mathf.Max(segments, 1);

            var points = new List<Vector3>();
            float floatSegments = segments;

            for (var i = 1; i < paths.Count; i++)
            {
                var p0 = paths[i - 1].Anchore;
                var p1 = paths[i - 1].Handle2;
                var p2 = paths[i].Handle1;
                var p3 = paths[i].Anchore;

                for (var j = 0; j <= segments; j++)
                {
                    points.Add(GetPoint(p0, p1, p2, p3, j / floatSegments));
                }
            }

            return points;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0">A点</param>
        /// <param name="p1">A点Handle</param>
        /// <param name="p2">B点Handle</param>
        /// <param name="p3">B点</param>
        /// <param name="t">0-1进度</param>
        /// <returns></returns>
        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            var oneMinusT = 1f - t;
            return oneMinusT * oneMinusT * oneMinusT * p0 +
                   3f * oneMinusT * oneMinusT * t * p1 +
                   3f * oneMinusT * t * t * p2 +
                   t * t * t * p3;
        }
    }
}