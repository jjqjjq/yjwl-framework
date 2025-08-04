using System;
using JQCore.tLog;
using UnityEngine.Profiling;

namespace JQCore.tPerformance
{

    public static class ManagedHeapUtil
    {

        public static void printMemory()
        {
            JQLog.Log("GC");
            GC.Collect();
            long usedsize = Profiler.GetMonoUsedSizeLong();
            long heapsize = Profiler.GetMonoHeapSizeLong();
            //builder.AppendFormat("使用内存:{0},剩余内存{1},托管堆内存{2}",usedsize,reservedsize,heapsize);  
            //print(builder.ToString());  
            JQLog.LogFormat("代码内存占用:{0}M/{1}M", usedsize*1f/1024f/1024f, heapsize*1f/1024f/1024f);
            
        }

    }
}
