using System.Collections.Generic;

namespace ET.ETFramework.Model.Client.Game
{
    public interface IReportEvent
    {
        public Dictionary<string, string> ToDictionary();
    }
}