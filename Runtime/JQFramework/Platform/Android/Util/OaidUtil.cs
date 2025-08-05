using JQCore.tLog;
using UnityEngine;

namespace JQFramework.Platform
{
#if UNITY_ANDROID
    public static class OaidUtil
    {
        private static AndroidJavaObject jo;

        public static void Register()
        {
            jo = new AndroidJavaObject("com.yjwl.android.module.OaidWrapper");
            jo.Call("register");
        }

        public static string getIMEI()
        {
            if (jo == null)
            {
                JQLog.LogError($"getIMEI失败，未注册");
                return null;
            }
            return jo.Call<string>("getIMEI");
        }

        public static string getAndroidID()
        {
            if (jo == null)
            {
                JQLog.LogError($"getAndroidID失败，未注册");
                return null;
            }
            return jo.Call<string>("getAndroidID");
            
        }
        
        public static string getPseudoID()
        {
            if (jo == null)
            {
                JQLog.LogError($"getPseudoID失败，未注册");
                return null;
            }
            return jo.Call<string>("getPseudoID");
            
        }
        
        public static string getGUID()
        {
            if (jo == null)
            {
                JQLog.LogError($"getGUID失败，未注册");
                return null;
            }
            return jo.Call<string>("getGUID");
            
        }
        
        

        public static string getOAID()
        {
            if (jo == null)
            {
                JQLog.LogError($"获取oaid失败，未注册");
                return null;
            }
            return jo.Call<string>("getOAID");
        }
    }
#endif
}