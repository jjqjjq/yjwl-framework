using System.Text;
using JQCore.tRes;

namespace JQFramework.tUtil
{
    public enum ResourceType
    {
        MODEL = 1,
        SCENE = 2,
        MATERIAL = 3,
        EFFECT = 4,
    }


    public static class GamePathUtil
    {
        private static StringBuilder _stringBuilder = new StringBuilder();

        private static ResPathDic _modelDic = new ResPathDic();
        private static ResPathDic _sceneDic = new ResPathDic();
        private static ResPathDic _materialDic = new ResPathDic();
        private static ResPathDic _effectDic = new ResPathDic();

        //根据资源类型直接获得路径
        public static string GetUrl(ResourceType type, int id, bool isHigh = false)
        {
            switch (type)
            {
                case ResourceType.SCENE:
#if TUANJIE_WEIXINMINIGAME
                    return _sceneDic.getPath(_stringBuilder, "Scene/{0}/scene_{0}", ".scene", true, id);
#else
                    return _sceneDic.getPath(_stringBuilder, "Scene/{0}/scene_{0}", ".unity", true, id);
#endif
                case ResourceType.MODEL:
                    return _modelDic.getPath(_stringBuilder, "Model/Prefab/model_{0}", ".prefab", isHigh, id);
                case ResourceType.MATERIAL:
                    return _materialDic.getPath(_stringBuilder, "Model/Material/model_{0}", ".mat", isHigh, id);
                case ResourceType.EFFECT:
                    return _effectDic.getPath(_stringBuilder, "Effect/effect_{0}", ".prefab", true, id);
            }

            return null;
        }
    }
}