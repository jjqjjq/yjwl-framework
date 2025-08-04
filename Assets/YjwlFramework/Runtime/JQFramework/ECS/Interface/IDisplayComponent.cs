using JQCore.tEvent;
using JQFramework.UIModel;
using UnityEngine;

namespace JQCore.ECS.Interface
{
    public interface IDisplayComponent
    {
        Transform transform { get; }
        GameObject gameObject { get; }
        JQEventDispatcher eventDispatcher { get; }
        void setUIModelCtrl(UIModelCtrl modelCtrl);
        void ready(IEntityVo vo);
        void Dispose();
    }
}