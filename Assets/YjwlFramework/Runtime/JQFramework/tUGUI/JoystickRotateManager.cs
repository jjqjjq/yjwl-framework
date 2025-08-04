using JQCore.tUtil;
using JQCore;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;


namespace JQFramework.tUGUI
{
    public class JoystickRotateManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public bool IsMoveing = false;
        public RectTransform Joystick; //摇杆
        public RectTransform Joystick_contenrHl; //摇杆指向
        public Sys.VoidDelegateFF updateJoystickAction;
        public Sys.VoidDelegateFF stopJoystickAction;

        public Camera uiCamera;
        public Vector2 offset;
        public Vector2 chazhiOffset;

        public bool isEnableMouse;
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
            Joystick_contenrHl.gameObject.SetActive_EX(false);
            Joystick.gameObject.SetActive_EX(false);
            if (uiCamera)
            {
                SetUICamera(uiCamera);
            }
        }

        public void SetUICamera(Camera camera)
        {
            offset = camera.WorldToScreenPoint(transform.position);
        }


        private bool _isCtrling = false;

        void Update()
        {
            if (IsMoveing)
            {
                //这一帧没移动
                if (Time.frameCount == _lastDragFrameCount + 1)
                {
                    // Debug.Log($"StopDrag:{Time.frameCount}");
                    if (stopJoystickAction != null)
                    {
                        stopJoystickAction(0, 0);
                    }
                    chazhiOffset = Vector2.zero;
                }    
            }

          
#if UNITY_EDITOR
            if (!isEnableMouse) return;
            float horizontal = Input.GetAxis("Mouse X");
            float vertical = Input.GetAxis("Mouse Y");
            if (horizontal != 0 || vertical != 0)
            {
                if (updateJoystickAction != null)
                {
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
#endif
        }
        private Vector2 m_Rotation;
      
        private int _lastDragFrameCount;
        private Vector2 _startPos;
        
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            IsMoveing = true;
            Joystick.gameObject.SetActive_EX(true);
            Joystick_contenrHl.gameObject.SetActive_EX(true);

            _startPos = eventData.position;
            Joystick.anchoredPosition = eventData.position - offset;
            // Debug.Log($"eventPos:{eventData.position} offset:{offset} pos:{pos}");
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Debug.Log($"frameCount:{Time.frameCount} lastPos:{_startPos} currPos:{eventData.position}");
            _lastDragFrameCount = Time.frameCount;
            chazhiOffset = eventData.position - _startPos;
            //差值算法
            _startPos = eventData.position;
            
            if (updateJoystickAction != null)
            {
                updateJoystickAction(chazhiOffset.x, chazhiOffset.y);
            }

            Joystick_contenrHl.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90 - MathUtil.GetAngleByVector2(chazhiOffset)));
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            // Debug.Log("EndDrag");
            IsMoveing = false;
            Joystick.gameObject.SetActive_EX(false);
            Joystick_contenrHl.gameObject.SetActive_EX(false);
            if (stopJoystickAction != null)
            {
                stopJoystickAction(0, 0);
            }
        }
    }
}