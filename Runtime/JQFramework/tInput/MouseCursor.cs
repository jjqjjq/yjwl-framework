using System.Collections;
using System.Collections.Generic;
using JQFramework.tMgr;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseCursor: MonoBehaviour
{
    public Camera uiCamera;
    public Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
    }

    //使一张Image的位置实时跟随鼠标
    void Update()
    {
        if (uiCamera == null) return;
        if (mainCamera == null) return;
        Vector2 clickPos = InputMgr.getInputUIPos(uiCamera);
        transform.localPosition = clickPos;
    }
}
