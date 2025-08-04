using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace JQEditor.Tool
{
    public class SystemProcessUtil
    {
        public static void ExecuteProcess(string filePath, string command, string workPath = "", int seconds = 0)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            Process process = new Process();//创建进程对象
            process.StartInfo.WorkingDirectory = workPath;
            process.StartInfo.FileName = filePath;
            process.StartInfo.Arguments = command;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = false;//不重定向输出
            try
            {
                if (process.Start())
                {
                    if (seconds == 0)
                    {
                        process.WaitForExit(); //无限等待进程结束
                    }
                    else
                    {
                        process.WaitForExit(seconds); //等待毫秒
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                process.Close();
            }
        }
    }
}
