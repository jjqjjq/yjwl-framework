
using JQCore.AudioEngine.Code;
using JQCore.tLoader;
using JQCore.tLog;
using JQCore.tPool.Loader;
using JQCore.tRes;

namespace JQFramework.tLauncher
{
    public class LauncherAudioLoader:BaseLoader
    {
        private const string resUrl1 = "Sound/HyAudioMixerResLib.prefab";
        private AssetObjectLib mixerResLib;
        private const string resUrl2 = "Sound/HyAudioResLib.prefab";
        private AssetObjectLib audioResLib;
        private const string resUrl3 = "Sound/SoundBank.json";
        private string configJson;
        
        public LauncherAudioLoader():base("LauncherAudioLoader")
        {
            initTotal(3);
        }

        public override void start()
        {
            base.start();
            Step1();
        }

        private void Step1()
        {
            JQLog.Log("加载音频资源1");
            UrlPrefabLoaderUtil.LoadAsset(resUrl1, onLoadCallback1, true);
        }
        
        private void onLoadCallback1(PrefabLoader prefabLoader)
        {
            mixerResLib = prefabLoader.prefab.GetComponent<AssetObjectLib>();
            UrlPrefabLoaderUtil.CancelLoadAsset(resUrl1, onLoadCallback1);
            finishOne();
            step2();
        }

        private void step2()
        {
            JQLog.Log("加载音频资源2");
            UrlPrefabLoaderUtil.LoadAsset(resUrl2, onLoadCallback2, true);
        }
        
        private void onLoadCallback2(PrefabLoader prefabLoader)
        {
            audioResLib = prefabLoader.prefab.GetComponent<AssetObjectLib>();
            UrlPrefabLoaderUtil.CancelLoadAsset(resUrl2, onLoadCallback2);
            finishOne();
            step3();
        }
        
        private void step3()
        {
            JQLog.Log("加载音频资源3");
            AssetLoaderUtil.LoadAsset(resUrl3, onLoadCallback3, true);
        }
        
        private void onLoadCallback3(AssetLoader assetLoader)
        {
            configJson = assetLoader.getTextStr();
            AssetLoaderUtil.CancelLoadAsset(resUrl3, onLoadCallback3);
            
            HyAudioEngine.Init(configJson, audioResLib, mixerResLib);
            
            //给战斗音效添加变速器
            // GameObject skillAudioRootGo = HyAudioObjectMgr.GetAudioBus("Master/SFX/JN").busGo;
            // TC_TimeLine tcTimeLine = skillAudioRootGo.AddMissingComponent<TC_TimeLine>();
            // if (tcTimeLine.globalClockKey != EConst.timeCtrlType.Battle)
            // {
            //     tcTimeLine.CacheClocks(EConst.timeCtrlType.Battle);
            // }
            // tcTimeLine.enabled = true;
            
            finishOne();
            finishAll();
        }
    }
}