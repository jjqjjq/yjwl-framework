/*----------------------------------------------------------------
// 文件名：HyBanner.cs
// 文件功能描述：
// 
// 创建者：JJQ
// 时间：2021/4/1 14:42:30
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using JQCore;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace JQFramework.tUGUI
{

    public class JQBanner : EventTrigger
    {
        private float _speed = 5;
        private int _curIndex = 0;
        private bool _autoMove = true; //是否自动滚动
        private float _autoMoveInteract = 3f; //自动滚动间隔

        private float _nextAutoMoveTime;
        private bool _isMoving = false; //是否移动中
        private float _targetPos = 0;
        public bool _isDraging = false;
        private float _currPos;

        private ScrollRect _scrollRect;
        private GameObject _itemPrefab;
        private Transform _itemParentTans;
        private GameObject _togPrefab;
        private Transform _togParentTrans;
        
        private Sys.VoidDelegateIntGameobject _itemCreateAction;
        private JQBannerItem[] _itemArr;
        private List<JQBannerTog> _togList = new List<JQBannerTog>();
        private JQBannerTog _currTog;
        private int _maxIndex;

        public void setSpeed(float speed)
        {
            _speed = speed;
        }

        public void setInteractTime(float autoMoveInteract)
        {
            _autoMoveInteract = autoMoveInteract;
        }

        public void setAutoMove(bool autoMove)
        {
            _autoMove = autoMove;
        }

        public void OnDestroy()
        {
            _scrollRect = null;
            _itemPrefab = null;
            _itemParentTans = null;
            _togPrefab = null;
            _togParentTrans = null;
            _itemCreateAction = null;
            for (int i = 0; i < _itemArr.Length; i++)
            {
                JQBannerItem item = _itemArr[i];
                item.dispose();
            }
            _itemArr = null;

            for (int i = 0; i < _togList.Count; i++)
            {
                JQBannerTog tog = _togList[i];
                tog.dispose();
            }
            _togList.Clear();
            _togList = null;
            _currTog = null;
        }


        public override void OnBeginDrag(PointerEventData eventData)
        {
            _isDraging = true;
            _isMoving = false;
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            _isDraging = false;
            float pos = _scrollRect.horizontalNormalizedPosition;
            int index = 0;
            for (int i = 0; i <= _maxIndex; i++)
            {
                if ((i - 0.5f) / _maxIndex <= pos && (i + 0.5f) / _maxIndex > pos)
                {
                    index = i;
                    break;
                }
            }
            setIndex(index, false);
        }



        public void init(ScrollRect scrollRect, GameObject itemPrefab, RectTransform itemParentTrans,
            GameObject togPrefab, RectTransform togParentTrans, int itemCount, Sys.VoidDelegateIntGameobject itemCreateAction)
        {
            _scrollRect = scrollRect;
            _itemPrefab = itemPrefab;
            _itemPrefab.SetActive(false);
            _itemParentTans = itemParentTrans;
            _togPrefab = togPrefab;
            _togPrefab.SetActive(false);
            _togParentTrans = togParentTrans;
            _itemCreateAction = itemCreateAction;
            Vector2 itemSize = (scrollRect.transform as RectTransform).sizeDelta;
            List<JQBannerItem> itemList = new List<JQBannerItem>();
            switch (itemCount)
            {
                case 1:
                    createItem(1, itemList);
                    createItem(1, itemList);
                    createItem(1, itemList);
                    break;
                case 2:
                    createItem(2, itemList);
                    createItem(1, itemList);
                    createItem(2, itemList);
                    createItem(1, itemList);
                    break;
                default:
                    createItem(itemCount, itemList);
                    for (int i = 1; i < itemCount; i++)
                    {
                        createItem(i, itemList);
                    }
                    break;
            }

            for (int i = 0; i < itemCount; i++)
            {
                JQBannerTog jqBannerTog = new JQBannerTog(_togPrefab, _togParentTrans);
                _togList.Add(jqBannerTog);
            }

            _itemArr = itemList.ToArray();
            _maxIndex = itemList.Count - 1;
            itemParentTrans.sizeDelta = new Vector2(itemSize.x * itemList.Count, itemSize.y);
            setIndex(1, true);
        }

        public void Refresh(Sys.VoidDelegateIntGameobject itemUpdateAction)
        {
            for (int i = 0; i < _itemArr.Length; i++)
            {
                JQBannerItem jqBannerItem = _itemArr[i];
                itemUpdateAction(jqBannerItem.Index, jqBannerItem.ItemGo);
            }
        }

        private void createItem(int itemIndex, List<JQBannerItem> itemList)
        {
            JQBannerItem jqBannerItem = new JQBannerItem(itemIndex, _itemPrefab, _itemParentTans);
            itemList.Add(jqBannerItem);
            if (_itemCreateAction != null)
            {
                _itemCreateAction(itemIndex, jqBannerItem.ItemGo);
            }
        }

        private void setIndex(int index, bool rightNow)
        {
            if (rightNow)
            {
                _scrollRect.horizontalNormalizedPosition = 1f * index / _maxIndex;
            }
            else
            {
                _targetPos = 1f * index / _maxIndex;
                _isMoving = true;
            }

            Debug.Log("setIndex:" + index);
            if (_currTog != null)
            {
                _currTog.setSelect(false);
            }
            JQBannerItem item = _itemArr[index];
            _currTog = _togList[item.Index - 1];
            _currTog.setSelect(true);
            _curIndex = index;

        }

        public void MoveToThatPart(int partIndex)
        {
            _targetPos = partIndex / _maxIndex;
            _isMoving = true;
        }

        public int onRightMove()
        {
            if (_curIndex < _maxIndex)
            {
                setIndex(_curIndex + 1, false);
            }
            return _curIndex;
        }

        public int onLeftMove()
        {
            if (_curIndex > 0)
            {
                setIndex(_curIndex - 1, false);
            }

            return _curIndex;
        }


        // Start is called before the first frame update
        void Start()
        {
            _nextAutoMoveTime = Time.time + _autoMoveInteract;
        }

        // Update is called once per frame
        void Update()
        {
            //            if (_currPos != -1f)
            //            {
            //                _scrollRect.horizontalNormalizedPosition = _currPos;
            //                _currPos = -1f;
            //            }
            //            Debug.Log(" _scrollRect.horizontalNormalizedPosition:" + _scrollRect.horizontalNormalizedPosition);
            if (_isMoving)
            {
                _scrollRect.horizontalNormalizedPosition = Mathf.Lerp(_scrollRect.horizontalNormalizedPosition,
                _targetPos, _speed * Time.deltaTime);
            }

            if (_autoMove && Time.time > _nextAutoMoveTime && !_isMoving && !_isDraging)
            {
                _nextAutoMoveTime = Time.time + _autoMoveInteract;
                onRightMove();
            }
        }

        void LateUpdate()
        {
            if (_isMoving)
            {
                if (Mathf.Abs(_scrollRect.horizontalNormalizedPosition - _targetPos) < 0.001)
                {
                    _isMoving = false;
                    //整理一下内容
                    if (_curIndex == 0)
                    {
                        JQBannerItem lastItem = _itemArr[_itemArr.Length - 1];
                        for (int i = _itemArr.Length - 2; i >= 0; i--)
                        {
                            _itemArr[i + 1] = _itemArr[i];
                        }
                        _itemArr[0] = lastItem;
                        lastItem.SetFirst();
                        setIndex(_curIndex + 1, true);
                        return;
                    }

                    if (_curIndex == _maxIndex)
                    {
                        JQBannerItem firstItem = _itemArr[0];
                        for (int i = 1; i < _itemArr.Length; i++)
                        {
                            _itemArr[i - 1] = _itemArr[i];
                        }
                        _itemArr[_itemArr.Length - 1] = firstItem;
                        firstItem.SetLast();
                        setIndex(_curIndex - 1, true);
                        return;
                    }
                }

            }
        }
    }
}
