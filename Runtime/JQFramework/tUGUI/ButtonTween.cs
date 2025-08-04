using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JQFramework.tUGUI
{

    public class ButtonTween : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        //public bool IsClick = false;
        public Action StartCallBack;
        public Action EndCallBack;
        private Animator animator;
        public void OnPointerDown(PointerEventData eventData)//鼠标按下事件
        {

            if (animator == null)
            {
                return;
            }
            animator.enabled = true;
            gameObject.GetComponent<Animator>().Play("Pressed", 0, 0);
        }
        public void OnPointerUp(PointerEventData eventData)//鼠标抬起事件
        {
            if (animator == null)
            {
                return;
            }
            gameObject.GetComponent<Animator>().Play("Normal", 0, 0);
        }
        public void Start()
        {
            if (animator == null)
            {
                animator = gameObject.GetComponent<Animator>();
            }
        }

        //public override void OnPointerExit(PointerEventData eventData)//移出按钮之外
        //{
        //    if (EndCallBack != null)
        //    {
        //        EndCallBack();
        //    }
        //}

        public void OnEnable()
        {
            if (animator != null)
            {
                animator.enabled = false;
            }
            transform.localScale = new Vector3(1, 1, 1);
            //animator.enabled = true;
        }
        void OnDisable()
        {
            // 滚动窗回收时序可能造成组件被误删除, 暂时屏蔽
            //Destroy(this);
            //Debug.LogError("22222222222222222222");

        }



        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
