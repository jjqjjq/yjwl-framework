using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ET;
using JQCore.tLog;
using JQCore.tSingleton;
using MongoDB.Bson.Serialization;

namespace JQFramework.tMgr
{
    public class StaticDataMgr : JQSingleton<StaticDataMgr>
    {
        private Dictionary<Type, byte[]> _configBytes = new Dictionary<Type, byte[]>();
        private Dictionary<Type, string> _configJsons = new Dictionary<Type, string>();

        private readonly Dictionary<Type, ISingleton> allConfig = new Dictionary<Type, ISingleton>();

        public void clear()
        {
            _configBytes.Clear();
            _configJsons.Clear();
            foreach (ISingleton iSingleton in allConfig.Values)
            {
                iSingleton.Destroy();
            }

            allConfig.Clear();
        }

        public override void Dispose()
        {
            _configBytes.Clear();
            _configJsons.Clear();
            foreach (ISingleton iSingleton in allConfig.Values)
            {
                iSingleton.Destroy();
            }

            allConfig.Clear();
        }

        public int AddOneType(Type type, byte[] bytes)
        {
            _configBytes[type] = bytes;
            return _configBytes.Count;
        }
        
        public int AddOneType(Type type, string json)
        {
            _configJsons[type] = json;
            return _configJsons.Count;
        }

        public void Load()
        {
            // using ListComponent<Task> listTasks = ListComponent<Task>.Create();
            foreach (Type type in _configBytes.Keys)
            {
                byte[] oneConfigBytes = _configBytes[type];
                LoadOneInThread(type, oneConfigBytes);
            } 
        }
        
        public void LoadByJson()
        {
            // using ListComponent<Task> listTasks = ListComponent<Task>.Create();
            foreach (Type type in _configJsons.Keys)
            {
                string configJson = _configJsons[type];
                LoadOneInThread(type, configJson);
            } 
        }

        //      public async ETTask LoadAsync()
        //      {
        //          using ListComponent<Task> listTasks = ListComponent<Task>.Create();
        // HyDebug.LogError("count:"+_configBytes.Count);
        //          foreach (Type type in _configBytes.Keys)
        //          {
        //              byte[] oneConfigBytes = _configBytes[type];
        //              Task task = Task.Run(() => LoadOneInThread(type, oneConfigBytes));
        //              listTasks.Add(task);
        //          }
        //
        //          await Task.WhenAll(listTasks.ToArray());
        //      }
        private void LoadOneInThread(Type configType, string oneConfigJson)
        {
            IMerge category = Activator.CreateInstance(configType) as IMerge;
            try
            {
                object deserialize = BsonSerializer.Deserialize(oneConfigJson, configType);
                category.Merge(deserialize);
                category.ProtoEndInit();
            }
            catch (Exception e)
            {
                JQLog.LogError($"oneConfigJson:{oneConfigJson} configType:{configType} e:{e}");
            }
            
            
            lock (this)
            {
                ISingleton singleton = category as ISingleton;
                singleton.Register();
                this.allConfig[configType] = singleton;
            }
        }


        private void LoadOneInThread(Type configType, byte[] oneConfigBytes)
        {
            // JQLog.LogError("type:"+configType + " bytes:"+oneConfigBytes.Length);
            object category = SerializeHelper.Deserialize(configType, oneConfigBytes, 0, oneConfigBytes.Length);

            lock (this)
            {
                ISingleton singleton = category as ISingleton;
                singleton.Register();
                this.allConfig[configType] = singleton;
            }
        }
    }
}