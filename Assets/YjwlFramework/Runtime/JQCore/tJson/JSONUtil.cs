namespace JQCore.tJson
{
    public static class JSONUtil
    {
        public static string getStrField(JSONObject jsonObject, string key)
        {
            string val = null;
            jsonObject.GetField(ref val, key);
            return val;
        }

        public static bool getBoolField(JSONObject jsonObject, string key, bool defaultVal = false)
        {
            bool val = defaultVal;
            jsonObject.GetField(ref val, key);
            return val;
        }

        public static int getIntField(JSONObject jsonObject, string key, int defaultVal = -1)
        {
            int val = defaultVal;
            jsonObject.GetField(ref val, key);
            return val;
        }

        public static uint getUintField(JSONObject jsonObject, string key, uint defaultVal = 0)
        {
            uint val = defaultVal;
            jsonObject.GetField(ref val, key);
            return val;
        }

        public static float getFloatField(JSONObject jsonObject, string key, float defaultVal = 0f)
        {
            float val = defaultVal;
            jsonObject.GetField(ref val, key);
            return val;
        }
    }
}