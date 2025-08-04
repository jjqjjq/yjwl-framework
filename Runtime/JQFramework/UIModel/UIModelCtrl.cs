using System.Collections.Generic;
using JQCore.tEnum;
using JQCore.tMgr;
using JQCore.tUtil;
using JQFramework.tMgr;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace JQFramework.UIModel
{
    public class UIModelCtrl
    {
        private int _index;
        private RawImage _linkRawImage = null;
        private List<RawImage> _otherRawImages = null;
        private GameObject _gameObject;
        private Transform _transform;
        private Transform _camObjTrans;
        // private CameraShake _cameraShake;
        private Camera _camComp;
        private RenderTexture _rtObj = null;
        private float3 _rotate = new float3(0, 180, 0);
        private float3 _pos;
        private bool _isLayerChange;

        public UIModelCtrl(int index)
        {
            _index = index;
            _gameObject = UnityUtil.NewGameObject("UIModel-" + index);
            _transform = _gameObject.transform;
            _transform.SetParent_EX(HierarchyLayerMgr.uiModel);
            _transform.SetLocalPos_EX(-100 - 100 * index, -100, 0);
            _transform.SetLocalRotation_EX(0, 0, 0);

            _camObjTrans = CommonResMgr.Instance.SpawnTransform(EFrameworkAsset.CommonRes_UIModelCamera);
            // _cameraShake = _camObjTrans.GetComponent<CameraShake>();
            _camComp = _camObjTrans.GetComponent<Camera>();
            _camObjTrans.SetParent_EX(_transform);
            _camObjTrans.SetLocalScale_EX(1, 1, 1);
            _camObjTrans.SetLocalRotation_EX(0, 0, 0);
            _gameObject.SetActive_EX(false);
        }

        public bool isInUse()
        {
            return _rtObj != null;
        }

        // public void shake(int shakeId)
        // {
        //     CameraShakePreset shakePreset = (CameraShakePreset)CommonResMgr.Instance.GetAsset("CameraShakePreset_" + shakeId);
        //     _cameraShake.Shake(shakePreset);
        // }
        //
        public void setCameraTrans(Vector3 pos, Quaternion rotate)
        {
            _transform.SetPositionAndRotation(pos, rotate);
        }

        public void addSubObj(Transform displayTrans, float3 rotate, float3 pos)
        {
            _rotate = rotate;
            _pos = pos;
            refreshSubObj(displayTrans);
        }

        private void refreshSubObj(Transform displayTrans)
        {
            displayTrans.SetParent_EX(_transform);
            displayTrans.SetLocalPos_EX(_pos.x, _pos.y, _pos.z);
            displayTrans.SetLocalRotation_EX(_rotate.x, _rotate.y, _rotate.z);
        }

        public void setAsBattleMgr()
        {
            _isLayerChange = true;
            _camComp.cullingMask =
                LayerMgr.GetMaskValue(LayerMgr.NormalLayers);
        }

        public void setParam(string name, RawImage rawImage, float cameraHeight, float cameraFarDis,
            float orthographicSize = 0, bool closeMSAA = true, float cameraAngle = 0, int depthBuffer = 16)
        {
            _linkRawImage = rawImage;
            float width = rawImage.rectTransform.rect.width;
            float height = rawImage.rectTransform.rect.height;
            int msaa = closeMSAA ? 1 : 2;//urp最多支持2倍MSAA

            //低品质强制关闭MSAA
            //TODO: 画质高低处理

            //相机位置
            _camObjTrans.SetLocalPos_EX(0, cameraHeight, -cameraFarDis);
            _camObjTrans.SetLocalRotation_EX(cameraAngle, 0, 0);

            //相机参数设置
            _camComp.aspect = width / height;
            _camComp.SetMSAA_EX(msaa > 1);

            if (orthographicSize > 0)
            {
                _camComp.orthographic = true;
                _camComp.orthographicSize = orthographicSize;
            }
            else
            {
                _camComp.orthographic = false;
                _camComp.fieldOfView = 45; //??
            }

            _rtObj = UnityUtil.GetTemporary_EX(name + "_" + _index, Mathf.FloorToInt(width), Mathf.FloorToInt(height), depthBuffer, msaa);
            _linkRawImage.texture = _rtObj;
            _linkRawImage.gameObject.SetActive_EX(true);

            _camComp.targetTexture = _rtObj;
            _gameObject.SetActive_EX(true);
        }

        public void addOtherRawImg(RawImage rawImage)
        {
            if (_rtObj != null)
            {
                rawImage.texture = _rtObj;
                rawImage.gameObject.SetActive_EX(true);
                if (_otherRawImages == null)
                {
                    _otherRawImages = new List<RawImage>();
                }

                _otherRawImages.Add(rawImage);
            }
        }

        public void release()
        {
            if (_gameObject)
            {
                _gameObject.SetActive_EX(false);
            }

            if (_rtObj)
            {
                UnityUtil.ReleaseTemporary_Ex(_rtObj);
                _rtObj = null;
            }

            if (_linkRawImage)
            {
                _linkRawImage.texture = null;
                _linkRawImage.gameObject.SetActive_EX(false);
                _linkRawImage = null;
            }

            if (_otherRawImages != null)
            {
                for (int i = 0; i < _otherRawImages.Count; i++)
                {
                    RawImage otherRawImage = _otherRawImages[i];
                    otherRawImage.texture = null;
                    otherRawImage.gameObject.SetActive_EX(false);
                }

                _otherRawImages = null;
            }

            _camComp.targetTexture = null;

            if (_isLayerChange)
            {
                _camComp.cullingMask = LayerMgr.GetMaskValue(new[] { ELayer.UIWorld });
                _isLayerChange = false;
            }

            _rotate = new float3(0, 180, 0);
        }

        public void dispose()
        {
            release();
            if (_camObjTrans != null)
            {
                CommonResMgr.Instance.DeSpawn(EFrameworkAsset.CommonRes_UIModelCamera, _camObjTrans.gameObject);
                _camObjTrans = null;
            }

            if (_gameObject != null)
            {
                UnityUtil.Destroy(_gameObject);
                _gameObject = null;
            }
        }
    }
}