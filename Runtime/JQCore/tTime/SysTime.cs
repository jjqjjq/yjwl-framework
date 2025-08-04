
using UnityEngine;

namespace JQCore.tTime
{

    public static class SysTime
    {
        public static bool showFrameCount = false;
        public static int frameCount = 0;
        public static float deltaTime;
        public static float time;

        public static void update()
        {
            frameCount = Time.frameCount;
            deltaTime = Time.deltaTime;
            time = Time.time;
        }

        public static float finalDeltaTime(TimeScaleInfo timeScaleInfo)
        {
            float tempDeltaTime = timeScaleInfo != null ? deltaTime * timeScaleInfo.timeScale : deltaTime;
            return tempDeltaTime;
        }

        public static void initTimeScale(GlobalClockKey globalClockKey)
        {
            //TODO:变速计时
        }
    }
}
