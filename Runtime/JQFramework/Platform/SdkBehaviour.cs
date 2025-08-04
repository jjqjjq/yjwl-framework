using System;
using System.Collections;
using System.Collections.Generic;
using AppStart.framework.Sdk;
using JQCore;
using JQCore.tLog;
using UnityEngine;

public class SdkBehaviour : MonoBehaviour
{
    // // Start is called before the first frame update
    // //apk回调
    // public void SDKKitOnResponse(string sResponse)
    // {
    //     if (sResponse.StartsWith("10100000_"))
    //     {
    //         JQLog.LogError("QuitGameTips");
    //         // HyXLua.QuitGameTips(QuitGame);
    //     }
    //     else
    //     {
    //         //App.sdkMgr.AnySDKCallBack(sResponse);
    //     }
    // }
    // public void SetBattery(string value)
    // {
    //     JQLog.LogError("SetBattery");
    //     // HyXLua.SetBattery(value);
    // }
    // public void GetSDKChannelIdCallBack(string channelId)
    // {
    //     JQLog.Log("GetSDKChannelIdCallBack-SDK:" + channelId);
    //     //App.sdkMgr.SetSDKChannelId(channelId);
    //     /// 初始化sdk类型
    //     App.appStart.sdkLoader.DoEnd();
    // }
    //
    //
    // /// <summary>
    // /// Sdk登出回调
    // /// </summary>
    // /// <param name="msg"></param>
    // public void ShowTencentLoginView(string msg)
    // {
    //     if (msg == "true")
    //     {
    //         JQLog.LogError("LogoutLua");
    //         // HyXLua.LogoutLua(true);
    //     }
    // }
    //
    // // 异形屏幕适配接口
    // public void ScreenAdaption(string deviceScreenWH)
    // {
    //     Debug.LogWarning("SdkBehavior.ScreenAdaption " + deviceScreenWH);
    //     JQLog.LogError("SetDeviceSafeArea");
    //     // HyXLua.SetDeviceSafeArea(deviceScreenWH);
    // }
    //
    // // 屏幕旋转回调
    // public void ScreenRotation(string directionDesc)
    // {
    //     Debug.LogWarning("SdkBehavior.directionDesc " + directionDesc);
    //     JQLog.LogError("SetScreenRotation");
    //     // HyXLua.SetScreenRotation(directionDesc);
    // }
    //
    // //显示退出警告
    // public void ShowQuitView()
    // {
    //     JQLog.Log("ShowQuitView");
    //     JQLog.LogError("ShowQuitView");
    //     // HyXLua.ShowQuitView();
    // }
    //
    // //退出游戏
    // public void QuitGame()
    // {
    //     JQLog.Log("Application quit called");
    //     JQLog.Log("用户确认退出游戏");
    //     Application.Quit();
    // }
    //
    //
    //
}