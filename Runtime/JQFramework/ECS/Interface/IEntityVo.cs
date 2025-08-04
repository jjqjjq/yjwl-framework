using System;
using System.Collections.Generic;
using JQFramework.ECS;
using Unity.Mathematics;
using UnityEngine;

namespace JQCore.ECS.Interface
{
    public interface IEntityVo
    {
        Action uiCallBack { get; }
    }
}