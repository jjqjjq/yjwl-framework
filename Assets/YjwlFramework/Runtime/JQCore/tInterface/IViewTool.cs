using System;

namespace JQCore
{
    public interface IViewTool
    {
        void showTips(string showMsg, float height = 75f);

        void showError(int errorCode);

        void ShowMessageBoxA(string descStr, string comfirmBtnStr = "取消", Action comfirmBtnAction = null);

        void ShowMessageBoxB(string titleStr, string descStr, string leftBtnStr = "取消", Action leftBtnAction = null,
            string rightBtnStr = "确定", Action rightBtnAction = null);

        public void ShowMessageBoxC(string titleStr, string descStr, float preferredHeight);
    }
}