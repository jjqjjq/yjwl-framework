using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using JQCore.tFileSystem;
using UnityEngine;

public class ReporterMessageReceiver : MonoBehaviour
{
    public static string datePatt = @"HH:mm:ss";

    private static readonly StringBuilder stringBuilder = new();
    private bool inited;
    private Reporter reporter;
    private LogWriter writer;

    private void Start()
    {
        checkWriter();
    }

    private void OnPreStart()
    {
        checkWriter();
        //To Do : this method is called before initializing reporter, 
        //we can for example check the resultion of our device ,then change the size of reporter
        if (reporter == null)
            reporter = gameObject.GetComponent<Reporter>();

        if (Screen.width < 1000)
            reporter.size = new Vector2(32, 32);
        else
            reporter.size = new Vector2(48, 48);

        reporter.UserData = "Put user date here like his account to know which user is playing on this device";
    }

    private void OnHideReporter()
    {
        //TO DO : resume your game
    }

    private void OnShowReporter()
    {
        //TO DO : pause your game and disable its GUI
    }

    private void checkWriter()
    {
        if (!inited)
        {
            reporter = gameObject.GetComponent<Reporter>();
            writer = new LogWriter();
            inited = true;
        }
    }

    private static string formatHandle(string formatStr, params object[] objs)
    {
        stringBuilder.AppendFormat(formatStr, objs);
        var content = stringBuilder.ToString();
        stringBuilder.Length = 0;
        return content;
    }


    private void OnLog(Reporter.Log log)
    {
        AddLog(log.logType, log.condition, log.stacktrace, log.frameCount);
    }

    public void AddLog(Reporter._LogType type, string condition, string stacktrace, int frameCount)
    {
        var dtString = DateTime.Now.ToString(datePatt);
        checkWriter();

        string str = null;

        switch (type)
        {
            case Reporter._LogType.Log:
                str = stringBuilder.Append("[LOG-").Append(dtString).Append("-").Append(frameCount).Append("]").Append(condition).Append("            \n").ToString();
                break;
            case Reporter._LogType.Warning:
                str = stringBuilder.Append("[WARNING-").Append(dtString).Append("-").Append(frameCount).Append("]").Append(condition).Append("            \n")
                    .Append(stacktrace).ToString();
                break;
            case Reporter._LogType.Error:
                str = stringBuilder.Append("[ERROR-").Append(dtString).Append("-").Append(frameCount).Append("]").Append(condition).Append("            \n")
                    .Append(stacktrace).ToString();
                break;
            case Reporter._LogType.Exception:
                str = stringBuilder.Append("[EXCEPTION-").Append(dtString).Append("-").Append(frameCount).Append("]").Append(condition).Append("            \n")
                    .Append(stacktrace).ToString();
                break;
            default:
                str = condition;
                break;
        }

        writer.Write(str);
        stringBuilder.Length = 0;
    }

    private void Destory()
    {
        if (writer != null) writer.Release();
    }


    public class LogWriter
    {
        private static readonly object m_locker = new();
        private readonly FileStream m_fs;
        private readonly string m_logFileName = "log_{0}.txt";
        private readonly string m_logFilePath;
        private readonly string m_logPath = Application.persistentDataPath + "/log/";
        private readonly StreamWriter m_sw;

        /// <summary>
        ///     默认构造函数。
        /// </summary>
        public LogWriter()
        {
            if (!Directory.Exists(m_logPath)) Directory.CreateDirectory(m_logPath);
            deleteTooOldLog(3);

            m_logFilePath = string.Concat(m_logPath, string.Format(m_logFileName, DateTime.Now.ToString("yyyyMMdd_HHmmss")));
            try
            {
                //HyDebug.Log(m_logFilePath);
                m_fs = new FileStream(m_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                m_sw = new StreamWriter(m_fs);
            }
            catch
            {
                //UnityEngine.Debug.LogError(ex.Message); 这样会冲突的
            }
        }

        private void deleteTooOldLog(int days)
        {
            var now = DateTime.Now;
            var logFileList = new List<string>();
            JQFileUtil.getAllFile(ref logFileList, m_logPath, null, false);
            for (var i = 0; i < logFileList.Count; i++)
            {
                var fileFullName = logFileList[i];
                var fileName = JQFileUtil.getCurrFolderOrFileName(fileFullName, false);
                var fileNameArrays = fileName.Split('_');
                var dateStr = fileNameArrays[1];
                var fileDateTime = DateTime.ParseExact(dateStr, "yyyyMMdd", CultureInfo.CurrentCulture);
                var sp = now.Subtract(fileDateTime);
                if (sp.Days > days) JQFileUtil.deleteFile(fileFullName);
            }
        }

        /// <summary>
        ///     释放资源。
        /// </summary>
        public void Release()
        {
            lock (m_locker)
            {
                if (m_sw != null)
                {
                    m_sw.Close();
                    m_sw.Dispose();
                }

                if (m_fs != null)
                {
                    m_fs.Close();
                    m_fs.Dispose();
                }
            }
        }

        /// <summary>
        ///     写日志。
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Write(string msg)
        {
            lock (m_locker)
            {
                if (m_sw != null)
                {
                    m_sw.WriteLine(msg);
                    m_sw.Flush();
                }
            }
        }
    }
}