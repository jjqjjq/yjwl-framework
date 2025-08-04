using System;
using System.Collections.Generic;
using JQCore;
using JQCore.tLog;
using JQCore.tSingleton;
using JQCore.tTime;
using JQFramework.tUtil;
using UnityEngine;

namespace JQFramework.tMgr
{
    public class InputMgr : JQSingleton<InputMgr>, ITick
    {
        private static int max = 512;
        private List<List<Action>> _codeList = new List<List<Action>>(max);
        private List<List<Action>> _mouseList = new List<List<Action>>(9);

        public InputMgr()
        {
            for (int i = 0; i < max; i++)
            {
                _codeList.Add(new List<Action>());
            }

            for (int i = 0; i < 9; i++)
            {
                _mouseList.Add(new List<Action>());
            }

            TickMgr.Instance.addTick(this);
        }

        public override void Dispose()
        {
            _codeList.Clear();
            _mouseList.Clear();
        }

        //获得ui坐标上的点击位置，屏幕中心为0,0
        //1920*1080 横屏 （竖屏未测试）
        public static Vector2 getInputUIPos(Camera uiCamera)
        {
            Vector2 screenSize = ViewUtil.getScreenUISize();
            Vector2 clickPos = getInputPosition();
            Vector2 viewPos = uiCamera.ScreenToViewportPoint(clickPos);
            Vector2 clickPos1 = (viewPos - Vector2.one * 0.5f) * screenSize;
            // JQLog.Log($"clickPos:{clickPos} viewPos:{viewPos} clickPos1:{clickPos1} screenSize:{screenSize}");
            return clickPos1;
        }

        public static Vector3 getInputPosition()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return Input.mousePosition;
#else
            if (Input.touches.Length > 0)
            {
                JQLog.Log($"touches pos:{Input.touches[0].position} ");
                return Input.touches[0].position;
            }

            return Vector3.zero;
#endif
        }

        public void addKeyCode(KeyCode keyCode, Action action)
        {
            List<Action> actionList = _codeList[(int)keyCode];
            actionList.Add(action);
        }

        public void removeKeyCode(KeyCode keyCode, Action action)
        {
            List<Action> actionList = _codeList[(int)keyCode];
            actionList.Remove(action);
        }

        public void addMouseCode(int mouseCode, Action action)
        {
            List<Action> actionList = _mouseList[mouseCode];
            actionList.Add(action);
        }

        public void removeMouseCode(int mouseCode, Action action)
        {
            List<Action> actionList = _mouseList[mouseCode];
            actionList.Remove(action);
        }

        public void onTick()
        {
            for (int i = 0; i < max; i++)
            {
                List<Action> actionList = _codeList[i];
                if (actionList.Count == 0) continue;
                if (Input.GetKeyDown((KeyCode)i))
                {
                    for (int j = 0; j < actionList.Count; j++)
                    {
                        actionList[j]();
                    }
                }
            }

            for (int i = 0; i < _mouseList.Count; i++)
            {
                List<Action> actionList = _mouseList[i];
                if (actionList.Count == 0) continue;
                bool success = false;
                switch (i)
                {
                    case 0:
                        success = Input.GetMouseButton((0));
                        break;
                    case 1:
                        success = Input.GetMouseButton((1));
                        break;
                    case 2:
                        success = Input.GetMouseButton((2));
                        break;
                    case 3:
                        success = Input.GetMouseButtonUp((0));
                        break;
                    case 4:
                        success = Input.GetMouseButtonUp((1));
                        break;
                    case 5:
                        success = Input.GetMouseButtonUp((2));
                        break;
                    case 6:
                        success = Input.GetMouseButtonDown((0));
                        break;
                    case 7:
                        success = Input.GetMouseButtonDown((1));
                        break;
                    case 8:
                        success = Input.GetMouseButtonDown((2));
                        break;
                }

                if (success)
                {
                    for (int j = 0; j < actionList.Count; j++)
                    {
                        actionList[j]();
                    }
                }
            }
        }
    }
}