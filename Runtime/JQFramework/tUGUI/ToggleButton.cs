using System;
using System.Collections;
using System.Collections.Generic;
using JQCore.tUtil;
using JQFramework.tUtil;
using UnityEngine;
using UnityEngine.UI;

namespace JQFramework
{
    [RequireComponent(typeof(Button))]
    public class ToggleButton : MonoBehaviour
    {
        public delegate void OnToggle(bool isOn);

        public OnToggle onToggle;
        public GameObject OnGo;
        public GameObject OffGo;
        public bool IsOn = false;
        private Button _button;

        // Start is called before the first frame update
        void Awake()
        {
            _button = GetComponent<Button>();
            ViewUtil.AddBtnListener(_button, OnButtonClick);
        }

        private void Start()
        {
            UpdateRender();
        }

        public void SetIsOn(bool isOn, bool isNotify = false)
        {
            IsOn = isOn;
            UpdateRender();
            if (isNotify)
            {
                onToggle?.Invoke(IsOn);
            }
        }

        private void UpdateRender()
        {
            if (OnGo != null)
            {
                OnGo.SetActive_EX(IsOn);
            }

            if (OffGo != null)
            {
                OffGo.SetActive_EX(!IsOn);
            }
        }

        private void OnButtonClick()
        {
            IsOn = !IsOn;
            UpdateRender();

            onToggle?.Invoke(IsOn);
        }
    }
}