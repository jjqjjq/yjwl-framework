#if !UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace JQFramework
{
    [Preserve]
    public class SkipSplash
    {
        private const bool enableSkipSplash = true;
        private const string SETTINGS_PATH = "SkipSplashSettings";
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void BeforeSplashScreen()
        {
            // 从资源加载设置
            if (!enableSkipSplash)
            {
                Debug.Log("[SkipSplash] 功能已禁用，不跳过启动画面");
                return;
            }
            
            Debug.Log("[SkipSplash] 功能已启用，跳过启动画面");
            
#if UNITY_WEBGL
            Application.focusChanged += OnFocusChanged;
#else
            System.Threading.Tasks.Task.Run(AsyncSkip);
#endif
        }

#if UNITY_WEBGL
        private static void OnFocusChanged(bool hasFocus)
        {
            Application.focusChanged -= OnFocusChanged;
            if (hasFocus)
            {
                SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
            }
        }
#else
        private static void AsyncSkip()
        {
            SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
        }
#endif
    }
}
#endif