using JQCore.ECS.Interface;
using UnityEngine;

namespace JQFramework.UIModel
{
    public class UIModelInfo
    {
        public UIModelCtrl uiModel;

        public IDisplayComponent display;

        // public UIDisplayShow uiDisplayShow;
        public GameObject parent;

        public void Dispose()
        {
            UIModelCtrlMgr.Instance.RemoveDisplsy(uiModel);
            uiModel = null;
            if (display != null)
            {
                display.Dispose();
                display = null;
            }

            parent = null;
        }
    }
}