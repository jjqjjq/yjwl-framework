using System;
using JQCore.tLog;
using YooAsset;

namespace JQCore.tYooAssets
{
    public class YooAssetLog : ILogger
    {
        public void Log(string message)
        {
            JQLog.Log(message);
        }

        public void Warning(string message)
        {
            JQLog.LogWarning(message);
        }

        public void Error(string message)
        {
            JQLog.LogError(message);
        }

        public void Exception(Exception exception)
        {
            JQLog.LogException(exception);
        }
    }
}