using JQCore.tFileSystem;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace JQEditor.Build.tPlatform
{
    public static class PlatformSwitchUtil
    {
        [MenuItem("YjwlWindows/PlatformSwitch/Douyin", true)]
        public static bool SwitchDouyin1()
        {
#if !SDK_DOUYIN
            return true;
#endif
            return false;
        }

        [MenuItem("YjwlWindows/PlatformSwitch/Douyin")]
        public static void SwitchDouyin()
        {
            BuildApkUtil.RemoveDefineSymbols("SDK_", true);
            BuildApkUtil.AddDefineSymbols("SDK_DOUYIN");
            Client.Remove("com.qq.weixin.minigame");
            JQFileUtil.CopyDirectory("Platform/Douyin/ByteGame", "Assets/Plugins/ByteGame");
            AssetDatabase.Refresh();
        }


        [MenuItem("YjwlWindows/PlatformSwitch/Weixin", true)]
        public static bool SwitchWeixin1()
        {
#if !SDK_WEIXIN
            return true;
#endif
            return false;
        }

        private static bool showProgressBar = true;
        private static AddRequest addRequest;

        [MenuItem("YjwlWindows/PlatformSwitch/Weixin")]
        public static void SwitchWeixin()
        {
            BuildApkUtil.RemoveDefineSymbols("SDK_", true);
            BuildApkUtil.AddDefineSymbols("SDK_WEIXIN");
            addRequest = Client.Add("https://gitee.com/wechat-minigame/minigame-tuanjie-transform-sdk.git");

            EditorApplication.update += AddProgressWithBar;

            JQFileUtil.DeleteDirectory("Assets/Plugins/ByteGame");
            AssetDatabase.Refresh();
            
        }

        #region 进度条处理方法

        /// <summary>
        /// 添加包进度处理 - 带进度条
        /// </summary>
        private static void AddProgressWithBar()
        {
            if (addRequest == null) return;

            // 显示进度条
            if (showProgressBar)
            {
                EditorUtility.DisplayProgressBar("添加包", $"正在添加包:", 0.5f);
            }

            if (addRequest.IsCompleted)
            {
                EditorApplication.update -= AddProgressWithBar;

                // 清除进度条
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (addRequest.Status == StatusCode.Success)
                {
                    Debug.Log($"✅ 包添加成功: {addRequest.Result.name}@{addRequest.Result.version}");
                }
                else if (addRequest.Status >= StatusCode.Failure)
                {
                    Debug.LogError($"❌ 包添加失败: {addRequest.Error?.message}");
                }

                addRequest = null;
            }
        }

        #endregion
    }
}