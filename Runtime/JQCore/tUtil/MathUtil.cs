using System;
using UnityEngine;

namespace JQCore.tUtil
{
    public static class MathUtil
    {
        //生成一个由数字和字母组合而成的长度为6的字符串随机种子
        public static string GenerateRandomSeed()
        {
            string seed = "";
            System.Random random = new System.Random();
            for (int i = 0; i < 6; i++)

            {
                int charInt = random.Next(0, 36);
                if (charInt < 10)
                {
                    //数字
                    seed += charInt.ToString();
                }
                else
                {
                    //字母
                    seed += (char)(charInt + 55);
                }
            }

            return seed;
        }
        
        /// <summary>
        /// 全方位移动算法
        /// </summary>
        /// <param name="pos">摇杆位置</param>
        /// <returns>移动方向</returns>
        public static float GetAngleByVector2(Vector2 pos)
        {
            float angle = Angle_180(pos, Vector2.up);
            return angle;
        }
        
        private static Vector3 v3, v3_1;
        private static Vector2 v2, v2_1;
        
        public static float Angle_180(Vector2 from, Vector2 to)
        {

            v3.x = from.x;
            v3.y = from.y;
            v3.z = 0;
            v3_1.x = to.x;
            v3_1.y = to.y;
            v3_1.z = 0;
            Vector3 v = Vector3.Cross(v3, v3_1);
            if (v.z > 0)
            {
                return Vector2.Angle(from, to);
            }
            else
            {
                return -Vector2.Angle(from, to);
            }
        }


        private const double PI = Math.PI;
        private const double PI_180 = Math.PI / 180;

        public static double sin(double angle)
        {
            angle = angle % 360;
            if (angle == 180)
            {
                return 0;
            }
            return Math.Sin(angle * PI_180);
        }
        
        public static double cos(double angle)
        {
            angle = angle % 360;
            if (angle == 90 || angle == 270)
            {
                return 0;
            }
            return Math.Cos(angle * PI_180);
        }
        
        public static double tan(double angle)
        {
            angle = angle % 360;
            if (angle == 0)
            {
                return 0;
            }
            return Math.Tan(angle * PI_180);
        }
        
        public static double asin(double angle)
        {
            return Math.Asin(angle) / PI_180;
        }
        
        public static double acos(double angle)
        {
            return Math.Acos(angle) / PI_180;
        }
        
        public static double atan(double angle)
        {
            return Math.Atan(angle) / PI_180;
        }
        
        public static double atan2(double y, double x)
        {
            return Math.Atan2(y, x) / PI_180;
        }
    }
}