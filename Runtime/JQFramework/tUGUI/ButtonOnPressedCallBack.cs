using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JQFramework.tUGUI
{
    

public class ButtonOnPressedCallBack: MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    //public bool IsClick = false;
    public Action<PointerEventData> StartCallBack;
    public Action<PointerEventData> EndCallBack;

    public void OnPointerDown(PointerEventData eventData)//鼠标按下事件
    {   
        if (StartCallBack != null)
        {
            StartCallBack(eventData);
        }
    }
    public void OnPointerUp(PointerEventData eventData)//鼠标抬起事件
    {
        if (EndCallBack != null)
        {
            EndCallBack(eventData);
        }
    }
    //public override void OnPointerExit(PointerEventData eventData)//移出按钮之外
    //{
    //    if (EndCallBack != null)
    //    {
    //        EndCallBack();
    //    }
    //}


    void OnDisable()
    {
        // 滚动窗回收时序可能造成组件被误删除, 暂时屏蔽
        //Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
          
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
}
