using System.Collections.Generic;

namespace ET.ETFramework.Model.Client.Game
{
    public class WxReportEvent:IWxReportEvent
    {
        // public string game_name;
        // public string mode_name;
        public int level;
        public int challengeCount;
        // public int item_id;
        public int time;
        public int itemId;//道具Id
        public int changeVal;//数量变化
        public string reliveType;
        public int reliveCount;
        public int useItemCount;
        public int playAdCount;
        public string failType;
        public int failCount;
        public int consecutiveWins;
        // public int mode_level_error;
        // public int mode_level_score;

        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("level", level.ToString());
            dict.Add("challengeCount", challengeCount.ToString());
            dict.Add("time", time.ToString());
            dict.Add("itemId", itemId.ToString());
            dict.Add("changeVal", changeVal.ToString());
            dict.Add("reliveType", reliveType.ToString());
            dict.Add("reliveCount", reliveCount.ToString());
            dict.Add("useItemCount", useItemCount.ToString());
            dict.Add("failType", failType.ToString());
            dict.Add("failCount", failCount.ToString());
            dict.Add("consecutiveWins", consecutiveWins.ToString());
            dict.Add("playAdCount", playAdCount.ToString());
            return dict;
        }
    }
}