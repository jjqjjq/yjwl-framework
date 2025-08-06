using System;
using System.Collections;
using System.Collections.Generic;
#if TextMeshPro
using TMPro;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace JQFramework
{
    /// <summary>
    /// 数字变化动画
    /// </summary>
    public class NumberChangeAnimator : MonoBehaviour
    {
#if TextMeshPro
        public TextMeshProUGUI text;
        
#else
        public Text text;
#endif

        public long startNum;
        public long currNum;
        public long targetNum;
        private float _startTime;
        public Slider slider;

        public float duration = 0.6f;

        // public float animationTime = 0f;
        public bool isPlaying = false;

        public Action<long> OnNumberChange;
        public Action OnAnimationEnd;

        public void SetSlider(Slider slider)
        {
            this.slider = slider;
        }


        // Update is called once per frame
        void Update()
        {
            if (currNum == targetNum) return;
            //动画结束
            if (!isPlaying)
            {
                _startTime = Time.time;
                startNum = currNum;
                isPlaying = true;
            }
            else //动画过程中
            {
                float passTime = Time.time - _startTime;
                if (passTime >= duration)
                {
                    isPlaying = false;
                    SetCurrNum(targetNum);
                    OnAnimationEnd?.Invoke();
                }
                else
                {
                    long num = (long)Mathf.Lerp(startNum, targetNum, passTime / duration);
                    SetCurrNum(num);
                }
            }
        }

        private void SetCurrNum(long num)
        {
            currNum = num;
            OnNumberChange?.Invoke(currNum);
            if (text != null)
            {
                text.text = currNum.ToString();
            }

            if (slider != null)
            {
                slider.value = currNum;
            }
        }
    }
}