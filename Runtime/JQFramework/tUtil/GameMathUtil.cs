using System;
using Unity.Mathematics;
using UnityEngine;

namespace JQFramework.tUtil
{
    public static class GameMathUtil
    {
        public static float3 up = new float3(0f, 1f, 0f);
        public static float3 forward = new float3(0f, 0f, 1f);

        
        public static float3 mul(this quaternion rotation, float3 point)
        {
            float4 rotation1 = rotation.value;
            float num1 = rotation1.x * 2f;
            float num2 = rotation1.y * 2f;
            float num3 = rotation1.z * 2f;
            float num4 = rotation1.x * num1;
            float num5 = rotation1.y * num2;
            float num6 = rotation1.z * num3;
            float num7 = rotation1.x * num2;
            float num8 = rotation1.x * num3;
            float num9 = rotation1.y * num3;
            float num10 = rotation1.w * num1;
            float num11 = rotation1.w * num2;
            float num12 = rotation1.w * num3;
            float3 vector3;
            vector3.x = (float) ((1.0 - ((double) num5 + (double) num6)) * (double) point.x + ((double) num7 - (double) num12) * (double) point.y + ((double) num8 + (double) num11) * (double) point.z);
            vector3.y = (float) (((double) num7 + (double) num12) * (double) point.x + (1.0 - ((double) num4 + (double) num6)) * (double) point.y + ((double) num9 - (double) num10) * (double) point.z);
            vector3.z = (float) (((double) num8 - (double) num11) * (double) point.x + ((double) num9 + (double) num10) * (double) point.y + (1.0 - ((double) num4 + (double) num5)) * (double) point.z);
            return vector3;
        }
        

        public static quaternion round(quaternion rotate, int digits = 3)
        {
            return new float4(round(rotate.value.x, digits), round(rotate.value.y, digits), round(rotate.value.z, digits), round(rotate.value.w, digits));
        }

        public static float3 round(float3 rotate, int digits = 3)
        {
            return new float3(round(rotate.x, digits), round(rotate.y, digits), round(rotate.z, digits));
        }

        public static Vector3 round(Vector3 rotate, int digits = 3)
        {
            return new Vector3(round(rotate.x, digits), round(rotate.y, digits), round(rotate.z, digits));
        }

        private static float round(float x, int digits)
        {
            return MathF.Round(x, digits);
        }
    }
}