using System.Collections.Generic;
using UnityEditor;

namespace JQEditor.Check.tCheckTexture
{
    public class CheckTextureInfo
    {
        //引用者路径列表
        public List<string> refPathList = new List<string>();

        public string path;

        public CheckTextureInfo(string path)
        {
            this.path = path;
        }

        public int refCount
        {
            get { return refPathList.Count; }
        }

        public void addRefPath(string path)
        {
            if (!refPathList.Contains(path))
            {
                refPathList.Add(path);
            }
        }

        public string firstRefViewPath
        {
            get
            {
                return refPathList[0];
            }
        }

        public bool isMainViewRef()
        {
            for (int i = 0; i < refPathList.Count; i++)
            {
                string viewPath = refPathList[i];
                if (isMainAtlas(viewPath))
                {
                    return true;
                }
            }
            return false;
        }

        private bool isMainAtlas(string viewPath)
        {
            if (viewPath == null) return false;
            if (viewPath.Contains("Main/MainView.prefab")) return true;
            if (viewPath.Contains("ItemContainer")) return true;
            if (viewPath.Contains("HeadContainer")) return true;
            if (viewPath.Contains("CommercialPushView")) return true;
            if (viewPath.Contains("HUDItem")) return true;
            if (viewPath.Contains("BattleAttr")) return true;
            if (viewPath.Contains("BattleNumber")) return true;
            if (viewPath.Contains("BattleSkill")) return true;
            return false;
        }
    }
}