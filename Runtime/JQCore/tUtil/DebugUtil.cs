//using Dest.Math;

using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR_64

#endif

namespace JQCore.tUtil
{

    public class DebugUtil
    {

        // Use this for initialization
        public static void DrawRect(Vector3 leftButtomPos, Vector3 rightTopPos, Color color, float duration = 2.0f)
        {
            Vector3 pointC = new Vector2(leftButtomPos.x, rightTopPos.y);
            Vector3 pointD = new Vector2(rightTopPos.x, leftButtomPos.y);
            Debug.DrawLine(leftButtomPos, pointC, color, duration);
            Debug.DrawLine(rightTopPos, pointC, color, duration);
            Debug.DrawLine(leftButtomPos, pointD, color, duration);
            Debug.DrawLine(rightTopPos, pointD, color, duration);

        }

//
//        public static void DrawCircleHandle(ref Circle2 circle, Color color)
//        {
//            int count = 40;
//            float delta = 2f*Mathf.PI/count;
//            Vector3 prev = circle.Eval(0);
//#if UNITY_EDITOR_64
//            Handles.color = color;
//            for (int i = 1; i <= count; ++i)
//            {
//                Vector3 curr = circle.Eval(i*delta);
//                Handles.DrawLine(prev, curr);
//                prev = curr;
//            }
//#endif
//        }

        public static void DrawLineHandle(Vector2 pointA, Vector2 pointB, Color color)
        {
#if UNITY_EDITOR_64
            Handles.color = color;
            Handles.DrawLine(pointA, pointB);
#endif
        }

        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float duration = 0.1f)
        {
            Debug.DrawLine(pointA, pointB, color, duration);
        }
//
//        public static void DrawCircle(ref Circle2 circle)
//        {
//            int count = 40;
//            float delta = 2f*Mathf.PI/count;
//            Vector3 prev = circle.Eval(0);
//            for (int i = 1; i <= count; ++i)
//            {
//                Vector3 curr = circle.Eval(i*delta);
//                Debug.DrawLine(prev, curr);
//                prev = curr;
//            }
//        }
//
//        public static void DrawCircle(Vector2 center, float radius)
//        {
//            Circle2 circle = new Circle2(ref center, radius);
//            int count = 40;
//            float delta = 2f*Mathf.PI/count;
//            Vector3 prev = circle.Eval(0);
//            for (int i = 1; i <= count; ++i)
//            {
//                Vector3 curr = circle.Eval(i*delta);
//                Debug.DrawLine(prev, curr);
//                prev = curr;
//            }
//        }
    }
}
