using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JQCore;
using JQCore.tCfg;
using JQCore.tEnum;
using JQCore.tLog;
using JQFramework;
using LitJson;
using UnityEngine.Device;
#if SDK_WEIXIN
using System.Diagnostics;
using JQCore.Log;
using JQCore.tPool;
using JQCore.tString;
using Newtonsoft.Json;
using UnityEngine;
using WeChatWASM;
#endif

namespace JQFramework.Platform
{
#if SDK_WEIXIN
    public class WxSdkMgr : ISdkMgr
    {
        private WXFileSystemManager _wxFileSystemManager;
        private WriteFileParam _writeFileParam;
        private ReadFileParam _readFileParam;
        private string _saveDataPath = $"{WX.env.USER_DATA_PATH}";
        private string _cloudEnvId;
        private SubscriptionsSetting _subscriptionsSetting;
        private Dictionary<string, string> _subscribeStorageDic = new Dictionary<string, string>();
        private string _currOpenId;
        private WxAdCtrl _wxAdCtrl;
        private string[] _shareCfgs;
        private bool _uploadFileFailed = false;

        public WxSdkMgr(params object[] args)
        {
            _cloudEnvId = args[0] as string;
            _shareCfgs = args[1] as string[];
            _writeFileParam = new WriteFileParam();
            _readFileParam = new ReadFileParam();
            WXBase.InitSDK(OnInitFish);
        }

        private void OnInitFish(int code)
        {
            //设置屏幕常亮
            WX.SetKeepScreenOn(new SetKeepScreenOnOption()
            {
                keepScreenOn = true
            });

            JQLog.Log("WX SDK 初始化完成:" + code);

            _wxFileSystemManager = WX.GetFileSystemManager();

            //云开发
            JQLog.Log("WX SDK Cloud:" + _cloudEnvId);
            WX.cloud.Init(new ICloudConfig()
            {
                env = _cloudEnvId,
            });

            UpdateSetting();

            UpdateQualitySettingByDeviceModelLevel();

        }

        //获得设备档位并设置性能
        private void UpdateQualitySettingByDeviceModelLevel()
        {
            WX.GetDeviceBenchmarkInfo(new GetDeviceBenchmarkInfoOption()
            {
                success = result =>
                {
                    int modelLevel = (int)result.modelLevel;//0（档位未知），1（高档机），2（中档机），3（低档机）
                    JQLog.Log("WX modelLevel:" + modelLevel);
                    switch (modelLevel)
                    {
                        case 0:
                        case 1:
                            JQCore.tUtil.UnityUtil.SetQualityLevel(0);
                            break;
                        case 2:
                            JQCore.tUtil.UnityUtil.SetQualityLevel(1);
                            break;
                        case 3:
                            JQCore.tUtil.UnityUtil.SetQualityLevel(2);
                            break;
                    }
                }
            });
        }


        private void UpdateSetting()
        {
            WX.GetSetting(new GetSettingOption()
            {
                withSubscriptions = true,
                success = result =>
                {
                    JQLog.Log(
                        $"GetSetting:{result} {result.subscriptionsSetting.mainSwitch} {result.subscriptionsSetting.itemSettings.Count}");
                    _subscriptionsSetting = result.subscriptionsSetting;
                },
            });
            string ss = WX.StorageGetStringSync("SubscribeInfo", string.Empty);
            _subscribeStorageDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(ss);
        }


        public string GetAccessToken()
        {
            throw new NotImplementedException();
        }

        public (float, float, float, float) GetSafeAreaInfo()
        {
            var info = WX.GetSystemInfoSync();
            double safeAreaLeft = info.safeArea.left;
            double safeAreaRight = info.screenWidth - info.safeArea.right;
            double safeAreaTop = info.safeArea.top;
            double safeAreaBottom = info.screenHeight - info.safeArea.bottom;
            JQLog.Log(
                $"x:{info.safeArea.top}, y:{info.safeArea.left}, width:{info.safeArea.width}, height:{info.safeArea.height} screen:{info.screenWidth} {info.screenHeight}");

            float safeLeftRate = (float)(safeAreaLeft / info.screenWidth);
            float safeRightRate = (float)(safeAreaRight / info.screenWidth);
            float safeTopRate = (float)(safeAreaTop / info.screenHeight);
            float safeBottomRate = (float)(safeAreaBottom / info.screenHeight);
            return (safeLeftRate, safeRightRate, safeTopRate, safeBottomRate);
        }

        public void SetParam(string str, object obj)
        {
            throw new NotImplementedException();
        }

        public bool IsStreamingAssetsExist()
        {
            return true;
        }

        public SdkPlatform GetPlatform()
        {
            throw new NotImplementedException();
        }

        public void StartAntiAddiction(string accountName)
        {
            throw new NotImplementedException();
        }


        private bool _isAddedToMyMiniProgram;

        public void CheckIsAddedToMyMiniProgram(Action<bool> callback)
        {
            WX.CheckIsAddedToMyMiniProgram(new CheckIsAddedToMyMiniProgramOption()
            {
                success = (result =>
                {
                    _isAddedToMyMiniProgram = result.added;
                    callback(_isAddedToMyMiniProgram);
                    // Sys.gameDispatcher.TriggerEvent(GlobalEvent.IsAddedToMyMiniProgramChange, _isAddedToMyMiniProgram);
                })
            });
        }

        public void GetInviteCount(Action<int> action)
        {
            CallCloudFunction("getInviteCount", null, jsonData =>
            {
                int inviteCount = (int)jsonData["inviteCount"];
                action?.Invoke(inviteCount);
            });
        }

        #region 订阅消息

        private List<string> _normalTmplIds = new List<string>();
        private List<string> _sysTmplIds = new List<string>();

        /// 设置订阅消息
        public void RequestSubscribeMessage(string[] tmplIds)
        {
            _normalTmplIds.Clear();
            _sysTmplIds.Clear();
            foreach (string tmplId in tmplIds)
            {
                if (tmplId.StartsWith("SYS_MSG_TYPE_"))
                {
                    _sysTmplIds.Add(tmplId);
                }
                else
                {
                    _normalTmplIds.Add(tmplId);
                }
            }
        }


        //订阅系统消息
        private void RequestSysSubscribeMsg()
        {
            if (_sysTmplIds.Count == 0)
            {
                return;
            }

            string[] tmplIds = _sysTmplIds.ToArray();
            WX.RequestSubscribeSystemMessage(new RequestSubscribeSystemMessageOption()
            {
                msgTypeList = tmplIds,
                success = result =>
                {
                    JQLog.Log($"RequestSubscribeSysMsg:{result}");
                    SendSubscribeResult(tmplIds, result);
                },
                fail = result => { JQLog.LogError($"RequestSubscribeSystemMessage Fail:{result.errMsg}"); }
            });
            _sysTmplIds.Clear();
        }

        private void RequestSubscribeMsg()
        {
            if (_normalTmplIds.Count == 0)
            {
                return;
            }

            string[] tmplIds = _normalTmplIds.ToArray();
            WX.RequestSubscribeMessage(new RequestSubscribeMessageOption()
            {
                tmplIds = tmplIds,
                success = result =>
                {
                    JQLog.Log($"RequestSubscribeMsg:{result}");
                    SendSubscribeResult(tmplIds, result);
                },
                fail = result1 => { JQLog.LogError($"RequestSubscribeMessage Fail:{result1.errMsg}"); }
            });
            _normalTmplIds.Clear();
        }

        private void SendSubscribeResult(string[] tmplIds, Dictionary<string, string> result)
        {
            Dictionary<string, string> msgResultDic = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> keyValuePair in result)
            {
                JQLog.Log($"RequestSubscribeMessage:{keyValuePair.Key}:{keyValuePair.Value}");
                string key = keyValuePair.Key;
                string value = keyValuePair.Value;
                if (tmplIds.Contains(key))
                {
                    msgResultDic[key] = value;
                    JQLog.Log($"    kv:{key} {value}");
                }
            }

            CallCloudFunction("requestSubscribe", msgResultDic, data => { UpdateSetting(); });
            foreach (KeyValuePair<string, string> keyValuePair in msgResultDic)
            {
                _subscribeStorageDic[keyValuePair.Key] = keyValuePair.Value;
            }

            string subscribeJson = JsonConvert.SerializeObject(_subscribeStorageDic);
            WX.StorageSetStringSync("SubscribeInfo", subscribeJson);
            Sys.gameDispatcher.TriggerEvent(FrameworkEvent.SubscribeUpdate);
        }

        public bool NeedShowSubscribeTips(string tmplId)
        {
            if (!_subscriptionsSetting.mainSwitch) return true;
            if (_subscriptionsSetting.itemSettings != null && _subscriptionsSetting.itemSettings.ContainsKey(tmplId)) return true;
            return false;
        }

        public bool IsSubscribed(string tmplId)
        {
            //勾线了记住选择，则使用wx的数据
            if (_subscriptionsSetting.itemSettings != null && _subscriptionsSetting.itemSettings.Count > 0)
            {
                if (_subscriptionsSetting.itemSettings.TryGetValue(tmplId, out string value))
                {
                    return value == "accept";
                }
            }

            _subscribeStorageDic.TryGetValue(tmplId, out string value1);
            if (value1 != null)
            {
                return value1 == "accept";
            }

            return false;
        }

        private void OnSubscribeTouchEnd(OnTouchStartListenerResult result)
        {
            RequestSysSubscribeMsg();
            RequestSubscribeMsg();
        }

        #endregion

        public void AddTouchEnd(string type)
        {
            switch (type)
            {
                case "Subscribe":
                    WX.OnTouchEnd(OnSubscribeTouchEnd);
                    break;
                default:
                    JQLog.LogError($"AddTouchEnd:{type} not found");
                    break;
            }
        }

        public void RemoveTouchEnd(string type)
        {
            switch (type)
            {
                case "Subscribe":
                    UpdateSetting();
                    WX.OffTouchEnd(OnSubscribeTouchEnd);
                    break;
                default:
                    JQLog.LogError($"AddTouchEnd:{type} not found");
                    break;
            }
        }

        public void Logout()
        {
        }

        public void GC()
        {
            MemoryUnloader.instance.addUnloadTask(WX.TriggerGC);
        }

        public void AddPermissions()
        {
            throw new NotImplementedException();
        }

        public bool IsNeedAddPermissions()
        {
            throw new NotImplementedException();
        }

        public void InitAfterPermission()
        {
            _wxAdCtrl = new WxAdCtrl(_shareCfgs);
            Sys.adCtrl = _wxAdCtrl;
        }

        public Task<bool> isLogined()
        {
            throw new NotImplementedException();
        }

        public void ReportEvent<T>(string eventId, T data)
        {
            JQLog.LogWarning($"ReportEvent:{eventId} {data}");
            //开通了再打开
            // IReportEvent iWxReportEvent = data as IReportEvent;
            // if (iWxReportEvent != null)
            // {
            //     WX.ReportEvent(eventId, iWxReportEvent.ToDictionary());
            // }
        }

        public void VibrateShort(int type)
        {
            if (!AppConfig.IsRelease) return;
            string typeStr = null;
            switch (type)
            {
                case 0:
                    typeStr = "light";
                    break;
                case 1:
                    typeStr = "medium";
                    break;
                case 2:
                    typeStr = "heavy";
                    break;
            }

            VibrateShortOption vibrateShortOption = new VibrateShortOption()
            {
                type = typeStr,
                success = (result => JQLog.Log($"VibrateShort:{result}")),
                fail = (result => JQLog.LogWarning($"VibrateShort:{result.errMsg}")),
            };
            WX.VibrateShort(vibrateShortOption);
        }


        #region 数据存储

        private void CallCloudFunction(string functionName, Dictionary<string, string> paramsDic,
            Action<JsonData> successCallback = null, Action failCallback = null)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            WX.cloud.CallFunction(new CallFunctionParam()
            {
                name = functionName,
                data = paramsDic,
                success = successResult =>
                {
                    stopwatch.Stop();
                    JQLog.LogWarning($"WX.CallFunction:{functionName}:{stopwatch.ElapsedMilliseconds}ms");

                    JQLog.Log($"WCF[{functionName}]:{successResult.result}");
                    if (successCallback == null) return;
                    if (!string.IsNullOrEmpty(successResult.result))
                    {
                        JsonData jsonData = JsonMapper.ToObject(successResult.result);
                        successCallback.Invoke(jsonData);
                    }
                    else
                    {
                        successCallback.Invoke(null);
                    }
                },
                fail = failResult =>
                {
                    JQLog.LogError($"WCF[{functionName}]:{failResult.errMsg}");
                    if (failCallback != null)
                    {
                        failCallback.Invoke();
                    }
                }
            });
        }

        private void ReadFile(string filePath, Action<string> successCallback, Action failCallback = null)
        {
            JQLog.Log($"ReadFile:{filePath}");
            _readFileParam.filePath = filePath;
            _readFileParam.encoding = "utf8";
            _readFileParam.success = response =>
            {
                string dataStr = response.stringData;
                JQLog.Log($"ReadFile success:{dataStr}");
                successCallback.Invoke(dataStr);
            };
            _readFileParam.fail = response =>
            {
                if (failCallback != null)
                {
                    failCallback.Invoke();
                }
                else
                {
                    JQLog.LogError($"ReadFile fail:{response.errCode} {response.errMsg}");
                }
            };
            _wxFileSystemManager.ReadFile(_readFileParam);
        }

        public void LoadDataAsync(string key, Action<string> callback)
        {
            //1.先从网上拿存档文件ID
            Dictionary<string, string> paramsDic = new Dictionary<string, string>();
            EnterOptionsGame enterOptionsGame = WX.GetEnterOptionsSync();
            if (enterOptionsGame.query.ContainsKey("SharerId"))
            {
                string sharerId = enterOptionsGame.query["SharerId"];
                paramsDic["SharerId"] = StringUtil.DecryptDES(sharerId);
            }

            CallCloudFunction("login", paramsDic, jsonData =>
            {
                string openId = jsonData["openId"].ToString();
                _currOpenId = openId;
                _wxAdCtrl.OpenId = openId;
                JQLog.Log($"LoadDataAsync Callback  openId:{openId}");


                //2.拿到文件ID则下载存档
                if (jsonData.ContainsKey("fileId"))
                {
                    string fileId = jsonData["fileId"].ToString();
                    JQLog.Log($"DownloadFile: fileId:{fileId} openId:{openId}");

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    WX.cloud.DownloadFile(new DownloadFileParam()
                    {
                        fileID = fileId,
                        cloudPath = $"{openId}.save",
                        success = successResult =>
                        {
                            stopwatch.Stop();
                            JQLog.LogWarning($"WX.DownloadFile:{stopwatch.ElapsedMilliseconds}ms");
                            JQLog.Log($"DownloadFile Success:{successResult}");
                            ReadFile(successResult.tempFilePath, callback);
                        },
                        fail = failResult =>
                        {
                            JQLog.LogError($"DownloadFile Fail:{failResult}");
                            ReadFile($"{_saveDataPath}/{key}", callback,
                                () => { callback?.Invoke(null); });
                        }
                    });
                }
                //3.拿不到则看看本地有没有
                else
                {
                    ReadFile($"{_saveDataPath}/{key}", callback,
                        () => { callback?.Invoke(null); });
                }
            });

            //4.本地有则返回，没有则初始化存档
        }

        // public void LoadDataAsync(string key, Action<string> callback)
        // {
        //     _readFileParam.filePath = $"{_saveDataPath}/{key}";
        //     _readFileParam.encoding = "utf8";
        //     _readFileParam.success = response =>
        //     {
        //         string dataStr = response.stringData;
        //         JQLog.Log($"LoadDataAsync:{dataStr}");
        //         callback.Invoke(dataStr);
        //     };
        //     _readFileParam.fail = response => { JQLog.LogError($"LoadDataAsync fail:{response.errCode} {response.errMsg}"); };
        //     _wxFileSystemManager.ReadFile(_readFileParam);
        // }

        public void SaveDataAsync(string key, string value)
        {
            string filePath = $"{_saveDataPath}/{key}";
            _writeFileParam.filePath = filePath;
            _writeFileParam.encoding = "utf8";
            var bytes = Encoding.UTF8.GetBytes(value);
            _writeFileParam.data = bytes;
            _writeFileParam.success = response => { JQLog.Log($"SaveDataAsync:{filePath} {value}"); };
            _writeFileParam.fail = response => { JQLog.LogError($"SaveDataAsync fail:{response.errCode} {response.errMsg}"); };
            _wxFileSystemManager.WriteFile(_writeFileParam);
        }

        public void SaveCloudDataAsync(string key)
        {
            string filePath = $"{_saveDataPath}/{key}";
            if (!_uploadFileFailed)
            {
                WX.cloud.UploadFile(new UploadFileParam()
                {
                    filePath = filePath,
                    cloudPath = $"Save/{_currOpenId}.saveData",
                    success = successResult =>
                    {
                        JQLog.Log($"WCF[UploadFile]:{successResult}");
                        Dictionary<string, string> paramsDic = new Dictionary<string, string>();
                        paramsDic["fileId"] = successResult.fileID;
                        CallCloudFunction("saveCloudData", paramsDic);
                    },
                    fail = failResult =>
                    {
                        _uploadFileFailed = true;
                        JQLog.LogError($"WCF[UploadFile]:{failResult.errMsg}");
                    }
                });
            }
        }

        public void ClearData(string key)
        {
            string filePath = $"{_saveDataPath}/{key}";
            JQLog.Log($"ClearData:{filePath}");
            JQLog.Log($"GetSavedFileList:");
            _wxFileSystemManager.GetSavedFileList(new GetSavedFileListOption()
            {
                success = result =>
                {
                    foreach (FileItem fileItem in result.fileList)
                    {
                        JQLog.LogWarning($"File:{fileItem.filePath}");
                    }
                }
            });

            JQLog.Log($"GetFileInfo:");
            _wxFileSystemManager.GetFileInfo(new GetFileInfoOption()
            {
                filePath = filePath,
                success = response => { JQLog.Log($"size:{response.size}"); },
                fail = response => { JQLog.LogError($"File:{response.errCode} {response.errMsg}"); }
            });

            JQLog.Log($"Access:");
            string result = _wxFileSystemManager.AccessSync(filePath);
            JQLog.Log($"Access:{result}");
            _wxFileSystemManager.Access(new AccessParam()
            {
                path = filePath,
                success = response => { JQLog.Log($"Access success:{response.callbackId}"); },
                fail = response => { JQLog.LogError($"Access fail:{response.errCode} {response.errMsg}"); }
            });

            string xx = _wxFileSystemManager.ReadFileSync(filePath, "utf8");
            JQLog.Log($"xx:{xx}");

            _wxFileSystemManager.RemoveSavedFile(new RemoveSavedFileOption()
            {
                filePath = filePath,

                success = response => { JQLog.Log($"RemoveSavedFile:{_saveDataPath}/{key}"); },
                fail = response => { JQLog.LogError($"RemoveSavedFile fail:{response.errCode} {response.errMsg}"); }
            });
        }

        public bool IsDataExist(string key)
        {
            string result = _wxFileSystemManager.AccessSync($"{_saveDataPath}/{key}");
            return result.Equals("access:ok");
        }

        #endregion
    }
#endif
}