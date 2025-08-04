using System;

namespace JQCore
{
    public interface ISaveCtrl
    {
        void LoadData(int index, Action<string> callback);

        void SaveData<T>(int index, T data);

        void SaveCloudData(int index);
        
        void ClearData(int index);
        bool IsDataExist(int index);
    }
}