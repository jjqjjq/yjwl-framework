using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using JQCore.tLog;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace JQCore.Log
{
    public class HyDebug : IJQLog
    {
        private static HyDebug _instance;
        private bool _canShowGUI;
        private int _errorCount;
        private ReporterMessageReceiver _reporterMessageReceiver;
        public bool EnableLog = true; //true  ;

        public GameObject reporterGo;
        private readonly StringBuilder stringBuilder = new();


        private readonly List<Reporter.Log> threadedLogs = new();

        private Action<string, string> _errorReporterAction;

        public bool CanShowGUI => _canShowGUI;

        public static HyDebug Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HyDebug();
                    JQLog.iLog = _instance;
                }

                return _instance;
            }
        }

        public void SetErrorReporter(Action<string, string> action)
        {
            _errorReporterAction = action;
        }

        public Reporter reporter { get; private set; }

        public bool hadError => _errorCount != 0;

        public void ResetErrorCount()
        {
            _errorCount = 0;
        }

        public void updateCanShowGUI(bool IsDebug)
        {
            SetCanShowGUI(IsDebug);
        }

        public void SetCanShowGUI(bool canShowGUI)
        {
            if (reporter != null)
            {
                reporter.setCanShowGUI(canShowGUI);
                _canShowGUI = canShowGUI;
            }
        }


        public void init(bool showLog)
        {
            Debug.Log("HyDebug:" + showLog);
            EnableLog = showLog;
            var oldReporter = reporterGo = GameObject.Find("Reporter");
            if (oldReporter != null)
            {
                Debug.LogError("Destroy:" + showLog);
                Object.Destroy(oldReporter);
            }

            if (reporterGo != null) //已经创建
            {
                if (!showLog)
                {
                    reporterGo.SetActive(false);
                }
                else
                {
                    reporterGo.SetActive(true);
                    Reporter.created = false;
//                    Debug.Log("GetComponent");
                    reporter = reporterGo.GetComponent<Reporter>();
                    _reporterMessageReceiver = reporterGo.GetComponent<ReporterMessageReceiver>();
                    reporter.Initialize();
                    //reporterCom.doShow();
                }
            }
            else if (showLog)
            {
                Reporter.created = false;
//                Debug.Log("Instantiate");
                var reporterPrefab = Resources.Load("Reporter") as GameObject;
                reporterGo = Object.Instantiate(reporterPrefab);
                reporter = reporterGo.GetComponent<Reporter>();
                _reporterMessageReceiver = reporterGo.GetComponent<ReporterMessageReceiver>();
                reporterGo.name = "Reporter";
            }

            //创建一个线程用于写日志
#if !SDK_WEIXIN
            var thread = new Thread(SetProcessFiles);
            thread.Start();
#endif
        }

        private void CaptureLogThread(LogType type, string condition, string stacktrace)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                _errorReporterAction?.Invoke(condition, stacktrace);
            }
            
            var frameCount = 0;
            try
            {
                frameCount = Time.frameCount;
            }
            catch (Exception e)
            {
            }

            var log = new Reporter.Log { condition = condition, stacktrace = stacktrace, logType = (Reporter._LogType)type, frameCount = frameCount };
            lock (threadedLogs)
            {
                threadedLogs.Add(log);
            }
        }

        private void SetProcessFiles()
        {
            while (true)
            {
                if (threadedLogs.Count > 0)
                    lock (threadedLogs)
                    {
                        for (var i = 0; i < threadedLogs.Count; i++)
                        {
                            var log = threadedLogs[i];
                            _reporterMessageReceiver.AddLog(log.logType, log.condition, log.stacktrace, log.frameCount);
                        }

                        threadedLogs.Clear();
                    }

                Thread.Sleep(30);
            }
        }


        public void WriteLog(string msg, LogType level, bool writeEditorLog)
        {
            switch (level)
            {
                case LogType.Log:
                    Log(msg);
                    break;
                case LogType.Warning:
                    LogWarning(msg);
                    break;
                case LogType.Error:
                    LogError(msg);
                    break;
            }
        }

        public void Release()
        {
        }

        public void UploadTodayLog()
        {
        }

        private string formatHandle(string formatStr, params object[] objs)
        {
            stringBuilder.AppendFormat(formatStr, objs);
            var content = stringBuilder.ToString();
            stringBuilder.Length = 0;
            return content;
        }

        public void LuaLog(string message)
        {
            if (_canShowGUI) Debug.Log(message);
            CaptureLogThread(LogType.Log, message, string.Empty);
        }


        public static void ClearConsole()
        {
#if UNITY_EDITOR
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var logEntries = assembly.GetType("UnityEditor.LogEntries");
            var clearConsoleMethod = logEntries.GetMethod("Clear");
            clearConsoleMethod.Invoke(new object(), null);
#endif
        }

        #region log

        public void Log(object message)
        {
            if (_canShowGUI) Debug.Log(message);
            CaptureLogThread(LogType.Log, message.ToString(), string.Empty);
        }

        public void Log(params object[] objs)
        {
            for (var i = 0; i < objs.Length; i++)
            {
                var obj = objs[i];
                stringBuilder.Append(obj);
            }

            var str = stringBuilder.ToString();
            stringBuilder.Length = 0;
            if (_canShowGUI) Debug.Log(str);
            CaptureLogThread(LogType.Log, str, string.Empty);
        }

        public void LogFormat(string formatStr, params object[] objs)
        {
            var content = formatHandle(formatStr, objs);
            if (_canShowGUI) Debug.Log(content);
            CaptureLogThread(LogType.Log, content, string.Empty);
        }

        #endregion

        #region warning

        public void LogWarning(object message)
        {
            if (_canShowGUI) Debug.LogWarning(message);
            CaptureLogThread(LogType.Warning, message.ToString(), string.Empty);
        }


        public void LogWarrningFormat(string formatStr, params object[] objs)
        {
            var content = formatHandle(formatStr, objs);
            if (_canShowGUI) Debug.LogWarning(content);
            CaptureLogThread(LogType.Warning, content, string.Empty);
        }

        #endregion

        #region error

        public void LogErrorSimple(object message)
        {
            _errorCount++;
            // HyXLua.ErrorLog(message.ToString(), 1);
            if (_canShowGUI) Debug.LogError(message);
            CaptureLogThread(LogType.Error, message.ToString(), string.Empty);
        }

        public void LogException(Exception e)
        {
            LogErrorFormat("[catch-error]:{0}\n{1}", e.ToString(), e.StackTrace);
        }

        public void LogError(object message)
        {
            _errorCount++;
            // HyXLua.ErrorLog(message.ToString(), 1);
            if (_canShowGUI) Debug.LogError(message);
            CaptureLogThread(LogType.Error, message.ToString(), new StackTrace().ToString());
        }

        public void LuaLogError(object message)
        {
            _errorCount++;
            // HyXLua.ErrorLog(message.ToString(), 2);
            if (_canShowGUI) Debug.LogError(message);
            CaptureLogThread(LogType.Error, message.ToString(), new StackTrace().ToString());
        }

        public void LogErrorFormat(string formatStr, params object[] objs)
        {
            var message = formatHandle(formatStr, objs);
            _errorCount++;
            if (_canShowGUI) Debug.LogError(message);
            CaptureLogThread(LogType.Error, message, new StackTrace().ToString());
        }

        #endregion
    }
}