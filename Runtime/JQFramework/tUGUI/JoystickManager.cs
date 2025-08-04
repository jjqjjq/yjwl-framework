
using JQCore;
using JQCore.tUtil;
using UnityEngine;
using UnityEngine.EventSystems;


namespace JQFramework.tUGUI
{
    public class JoystickManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public bool IsMoveing = false;
        public RectTransform Joystick; //摇杆
        public RectTransform Joystick_contenrHl; //摇杆指向
        public Sys.VoidDelegateFF updateJoystickAction;
        public Sys.VoidDelegateFF stopJoystickAction;

        public Camera uiCamera;
        public Vector2 offset;
        public bool isFixedJoystick = false;

        // private static float resolutionW = 1920;
        // private static float resolutionH = 1080;

        // private float proportionW;
        // private float proportionH;

        private void Awake()
        {
            // proportionW = Screen.width / resolutionW;
            // proportionH = Screen.height / resolutionH;
            // proportionW = 1;
            // proportionH = 1;
            SetJoystickVisible(false);
            if (uiCamera)
            {
                SetUICamera(uiCamera);
            }
        }

        public void SetUICamera(Camera camera)
        {
            offset = camera.WorldToScreenPoint(transform.position);
        }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR

        private bool _isCtrling = false;

        void Update()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            if (horizontal != 0 || vertical != 0)
            {
                if (updateJoystickAction != null)
                {
                    // Log.Error($"horizontal:{horizontal} vertical:{vertical}");
                    updateJoystickAction(horizontal, vertical);
                }

                _isCtrling = true;
            }
            else
            {
                if (_isCtrling)
                {
                    if (stopJoystickAction != null)
                    {
                        stopJoystickAction(0, 0);
                    }
                }

                _isCtrling = false;
            }
        }
#endif

        private Vector2 _startPos;


        public void OnBeginDrag(PointerEventData eventData)
        {
            IsMoveing = true;
            SetJoystickVisible(true);

            Vector2 pos = eventData.position - offset;
            _startPos = pos;
            if (!isFixedJoystick)
            {
                Joystick.anchoredPosition = pos;
            }
            // Debug.Log($"eventPos:{eventData.position} offset:{offset} pos:{pos}");
        }

        //摇杆变速度
        private float joystickValSpeed = 4f;
        
        private float currX;
        private float currY;
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pos = eventData.position - offset;
            pos = pos - _startPos;
            
            if (updateJoystickAction != null)
            {
                pos = pos.normalized;
                
                if(pos.x > 0)
                {
                    currX += Time.deltaTime * joystickValSpeed;
                    currX = Mathf.Clamp(currX, 0, pos.x);
                }
                else
                {
                    currX -= Time.deltaTime * joystickValSpeed;
                    currX = Mathf.Clamp(currX, pos.x, 0);
                }
                if(pos.y > 0)
                {
                    currY += Time.deltaTime * joystickValSpeed;
                    currY = Mathf.Clamp(currY, 0, pos.y);
                }
                else
                {
                    currY -= Time.deltaTime * joystickValSpeed;
                    currY = Mathf.Clamp(currY, pos.y, 0);
                }
             
                // Log.Error($"currX:{currX} currY:{currY}");
                updateJoystickAction(currX, currY);
                // Log.Error($"pos:{pos}");
                // updateJoystickAction(pos.x, pos.y);
            }

            Joystick_contenrHl.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90 - MathUtil.GetAngleByVector2(pos)));
        }

        private void SetJoystickVisible(bool visible)
        {
            if (!isFixedJoystick)
            {
                Joystick.gameObject.SetActive_EX(visible);
                Joystick_contenrHl.gameObject.SetActive_EX(visible);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            IsMoveing = false;
            SetJoystickVisible(false);
            if (stopJoystickAction != null)
            {
                stopJoystickAction(0, 0);
            }
        }
    }
}