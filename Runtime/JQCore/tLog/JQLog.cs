using System;
using UnityEngine;

namespace JQCore.tLog
{
    public static class JQLog
    {
        public static IJQLog iLog = null;

        public static void Log(object message)
        {
            if (iLog == null)
            {
                Debug.Log(message);
                return;
            }

            iLog.Log(message);
        }

        public static void SetErrorReporter(Action<string, string> errorReporterAction)
        {
            iLog.SetErrorReporter(errorReporterAction);
        }

        public static void Log(params object[] objs)
        {
            if (iLog == null)
            {
                Debug.Log(objs);
                return;
            }

            iLog.Log(objs);
        }

        public static void LogFormat(string formatStr, params object[] objs)
        {
            if (iLog == null)
            {
                Debug.LogFormat(formatStr, objs);
                return;
            }

            iLog.LogFormat(formatStr, objs);
        }

        public static void LogWarning(object message)
        {
            if (iLog == null)
            {
                Debug.LogWarning(message);
                return;
            }

            iLog.LogWarning(message);
        }

        public static void LogWarrningFormat(string formatStr, params object[] objs)
        {
            if (iLog == null)
            {
                Debug.LogWarningFormat(formatStr, objs);
                return;
            }

            iLog.LogWarrningFormat(formatStr, objs);
        }

        public static void LogErrorSimple(object message)
        {
            if (iLog == null)
            {
                Debug.LogError(message);
                return;
            }

            iLog.LogErrorSimple(message);
        }

        public static void LogException(Exception e)
        {
            if (iLog == null)
            {
                Debug.LogException(e);
                return;
            }

            iLog.LogException(e);
        }

        public static void LogError(object message)
        {
            if (iLog == null)
            {
                Debug.LogError(message);
                return;
            }

            iLog.LogError(message);
        }

        public static void LogErrorFormat(string formatStr, params object[] objs)
        {
            if (iLog == null)
            {
                Debug.LogErrorFormat(formatStr, objs);
                return;
            }

            iLog.LogErrorFormat(formatStr, objs);
        }
    }
}