using System;
using UnityEngine;
using UnityEngine.UI;

namespace JQFramework.tUGUI
{

    public class ScollViewOnValueChange : MonoBehaviour
    {
        public ScrollRect sc;
        public Action callback;

        void Start()
        {
            sc.onValueChanged = new ScrollRect.ScrollRectEvent();
            sc.onValueChanged.AddListener(DragingSc);
        }
        void OnEnable()
        {
    
        }
        void DragingSc(Vector2 vector2)
        {
            if (callback != null)
            {
                callback();
            }

        }
        // Update is called once per frame
        void Update()
        {

        }

        void OnDestroy()
        {
            sc.onValueChanged.RemoveAllListeners();
        }
        void OnDisable()
        {
            sc.onValueChanged.RemoveAllListeners();
            Destroy(this);
        }
    }
}
