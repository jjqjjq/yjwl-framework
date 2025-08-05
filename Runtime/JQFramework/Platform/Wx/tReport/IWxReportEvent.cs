using System.Collections.Generic;

namespace ET.ETFramework.Model.Client.Game
{
    public interface IWxReportEvent
    {
        public Dictionary<string, string> ToDictionary();
    }
}