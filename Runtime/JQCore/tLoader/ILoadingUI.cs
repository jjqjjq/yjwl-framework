using System;

namespace JQCore.tLoader
{
    public interface ILoadingUI
    {
        void open();
        void close();
        void setLoadingText(string text);
        void setLoadingProgress(long total, long remain);

        void setVersionText(string str);
        void setTipsText(string str);

        void setDebugText(string text);

        void showMessageBox(string titleStr, string textStr, string leftStr, Action leftAction, string rightStr, Action rightAction);
    }
}