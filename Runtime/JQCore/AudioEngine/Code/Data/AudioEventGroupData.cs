/*----------------------------------------------------------------
// 文件名：AudioEventGroupData.cs
// 文件功能描述：
//
// 创建者：JJQ
// 时间：2021/2/4 16:49:08
//----------------------------------------------------------------*/

using JQCore.tJson;

namespace JQCore.AudioEngine.Code.Data
{
    public class AudioEventGroupData
    {
        public AudioEventData[] audioEventDatas;
        public string eventName;
        public uint id;

        public AudioEventGroupData(JSONObject jsonObject)
        {
            id = JSONUtil.getUintField(jsonObject, "id");
            eventName = JSONUtil.getStrField(jsonObject, "name");

            var subJsonObject = jsonObject.GetField("subEvents");
            audioEventDatas = new AudioEventData[subJsonObject.Count];
            for (var i = 0; i < subJsonObject.Count; i++)
            {
                var eventJsonObj = subJsonObject.list[i];
                var audioEventData = new AudioEventData(eventName, eventJsonObj);
                audioEventDatas[i] = audioEventData;
            }
        }
    }
}