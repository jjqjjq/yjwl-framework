using System.Collections.Generic;
using System.Text;
using JQCore;

namespace JQCore.tRes
{
    public class ResPathDic
    {
        Dictionary<string, string> _pathDic = new Dictionary<string, string>();
        Dictionary<string, string> _lowPathDic = new Dictionary<string, string>();

        public string getPath(StringBuilder stringBuilder, string formatStr, string suffixStr, bool isHigh, int id)
        {
            string idStr = Sys.Int2StrLib.IntToStr(id);

            Dictionary<string, string> dic = isHigh ? _pathDic : _lowPathDic;

            // string isHighStr = isHigh ? "" : "_low";
            string isHighStr = "";
            string path = null;
            dic.TryGetValue(idStr, out path);
            if (path == null)
            {
                path = stringBuilder.AppendFormat(formatStr, idStr).Append(isHighStr).Append(suffixStr).ToString();
                dic[idStr] = path;
                stringBuilder.Length = 0;
            }
            return path;
        }
    }
}