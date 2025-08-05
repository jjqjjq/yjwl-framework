using System.Collections.Generic;

namespace ET.ETFramework.Model.Client.Game
{
    public class WxErrorReportEvent:IWxReportEvent
    {
        public int level { get; set; }
        public string error_msg { get; set; }
        public string error_stackTrace { get; set; }

        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["level"] = level.ToString();
            dict["error_msg"] = error_msg;
            dict["error_stackTrace"] = error_stackTrace;
            return dict;
        }
    }
}