#if UNITY_ANDROID

using JQCore.tLog;
using CodeStage.AntiCheat.Detectors;
using CodeStage.AntiCheat.Genuine.Android;
using CodeStage.AntiCheat.ObscuredTypes.Converters;
using CodeStage.AntiCheat.Storage;
using JQCore.tSingleton;
using Newtonsoft.Json;
using UnityEngine.Device;

namespace JQCore.tAntiCheat
{
    public class AntiCheatMgr : JQSingleton<AntiCheatMgr>,ISaveCtrl
    {
        public const string encryptionKey = "%￥#*Gfdwe3￥#@e3r43(&^%$#";

        private int _closeLeftTime;
        private ObscuredTypesNewtonsoftConverter _newtonsoftConverter;

        public override void Dispose()
        {
            ObscuredCheatingDetector.StopDetection();
            SpeedHackDetector.StopDetection();
            TimeCheatingDetector.StopDetection();
        }

        public void init()
        {
            // AppInstallationSourceValidator.GetAppInstallationSource();  获得安装来源
            ObscuredCheatingDetector.StartDetection(OnObscuredCheaterDetected);
            SpeedHackDetector.StartDetection(OnSpeedCheaterDetected);
            TimeCheatingDetector.StartDetection(OnTimeCheaterDetected);
            ObscuredFilePrefs.NotGenuineDataDetected += OnNotGenuineDataDetected;
            ObscuredFilePrefs.DataFromAnotherDeviceDetected += OnDataFromAnotherDeviceDetected;
#if ANDROID
            var source = AppInstallationSourceValidator.GetAppInstallationSource();
            if (source.DetectedSource != AndroidAppSource.AccessError)
            {
                JQLog.Log($"Installed from: {source.DetectedSource} (package name: {source.PackageName})");
            }
            else
            {
                JQLog.LogError("Failed to detect the installation source!");
            }
#endif
            // ObscuredFilePrefs initialize
            var deviceLockSettings = new DeviceLockSettings(DeviceLockLevel.Strict);
            var encryptionSettings = new EncryptionSettings(encryptionKey);
            var settings = new ObscuredFileSettings(encryptionSettings, deviceLockSettings);
            ObscuredFilePrefs.Init("android_setting.bin", settings, true);
            JQLog.Log($"{nameof(ObscuredFilePrefs)} file path: {ObscuredFilePrefs.FilePath}\n" +
                      $"{nameof(ObscurationMode)}: {ObscuredFilePrefs.CurrentSettings.EncryptionSettings.ObscurationMode}");


            // add it to default converters one time globally 
// ----------------------------------------------
            _newtonsoftConverter = new ObscuredTypesNewtonsoftConverter();
        }

        public T LoadData<T>(int index)
        {
            string json = ObscuredFilePrefs.Get("Save" + index, null);
            var output = JsonConvert.DeserializeObject<T>(json, _newtonsoftConverter);
            return output;
        }

        public void SaveData<T>(int index, T data)
        {
            var json = JsonConvert.SerializeObject(data, _newtonsoftConverter);
            ObscuredFilePrefs.Set("Save" + index, json);
            ObscuredFilePrefs.Save();
        }
        
        public void ClearData(int index)
        {
            ObscuredFilePrefs.DeleteKey("Save" + index);
            ObscuredFilePrefs.Save();
        }

        public bool IsDataExist(int index)
        {
            return ObscuredFilePrefs.HasKey("Save" + index);
        }

        private void OnNotGenuineDataDetected()
        {
            JQLog.LogError("OnNotGenuineDataDetected");
            TriggerAntiCheat();
        }

        private void OnDataFromAnotherDeviceDetected()
        {
            JQLog.LogError("OnDataFromAnotherDeviceDetected");
            TriggerAntiCheat();
        }


        private void OnObscuredCheaterDetected()
        {
            JQLog.LogError("OnObscuredCheaterDetected");
            TriggerAntiCheat();
        }

        private void OnSpeedCheaterDetected()
        {
            JQLog.LogError("OnSpeedCheaterDetected");
            TriggerAntiCheat();
        }

        private void OnTimeCheaterDetected(TimeCheatingDetector.CheckResult result,
            TimeCheatingDetector.ErrorKind error)
        {
            if (result == TimeCheatingDetector.CheckResult.WrongTimeDetected ||
                result == TimeCheatingDetector.CheckResult.CheatDetected)
            {
                JQLog.LogError($"OnTimeCheaterDetected: {result}, {error}");
                TriggerAntiCheat();
            }
        }

        private void TriggerAntiCheat()
        {
            Sys.viewTool.showTips("检测到作弊行为，5秒后关闭游戏");
            _closeLeftTime = 5;
            Sys.timerMgr.addRepeat(1, this, CountAppClose);
        }

        private void CountAppClose()
        {
            _closeLeftTime--;
            JQLog.LogError($"CountAppClose: {_closeLeftTime}");
            if (_closeLeftTime <= 0)
            {
                Sys.timerMgr.removeByCaller(this);
                Application.Quit();
            }
        }
    }
}
#endif