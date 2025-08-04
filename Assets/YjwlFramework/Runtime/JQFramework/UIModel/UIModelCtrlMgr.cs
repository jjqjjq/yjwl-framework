using System.Collections.Generic;
using JQCore.tLog;
using JQCore.tSingleton;

namespace JQFramework.UIModel
{
    public class UIModelCtrlMgr : JQSingleton<UIModelCtrlMgr>
    {
        private List<UIModelCtrl> _ctrlLib = new List<UIModelCtrl>();

        public UIModelCtrlMgr()
        {
            for (int i = 0; i < 20; i++)
            {
                _ctrlLib.Add(new UIModelCtrl(i));
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            for (int i = 0; i < _ctrlLib.Count; i++)
            {
                UIModelCtrl uiModelCtrl = _ctrlLib[i];
                uiModelCtrl.dispose();
            }
        }

        public UIModelCtrl Spwan()
        {
            for (int i = 0; i < _ctrlLib.Count; i++)
            {
                UIModelCtrl uiModelCtrl = _ctrlLib[i];
                if (!uiModelCtrl.isInUse())
                {
                    return uiModelCtrl;
                }
            }

            JQLog.LogWarning("同屏UI模型过多，额外创建新的UiModelCtrl");
            int index = _ctrlLib.Count + 1;
            UIModelCtrl newUiModelCtrl = new UIModelCtrl(index);
            _ctrlLib.Add(newUiModelCtrl);
            return newUiModelCtrl;
        }

        
        

        public void RemoveDisplsy(UIModelCtrl modelCtrl)
        {
            modelCtrl.release();
        }
    }
}