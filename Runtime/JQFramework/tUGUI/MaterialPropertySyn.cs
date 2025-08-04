using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace JQFramework
{
    public enum ValueType
    {
        Float,
        Int,
        Bool
    }

    
    public abstract class MaterialPropertySyn : MonoBehaviour
    {

        public string propertyName;
        public ValueType valType;

        public float floatVal;
        public int intVal;
        public bool boolVal;

        [FormerlySerializedAs("_material")] public Material material;

        private int _propertyId;
        private int _lastIntVal;
        private float _lastFloatVal;
        private bool _lastBoolVal;
        
        private float defaultFloatVal;
        private int defaultIntVal;
        private bool defaultBoolVal;
        private void Awake()
        {
            defaultFloatVal = floatVal;
            defaultIntVal = intVal;
            defaultBoolVal = boolVal;
            _propertyId = Shader.PropertyToID(propertyName);
        }

        public void Reset()
        {
            floatVal = defaultFloatVal;
            intVal = defaultIntVal;
            boolVal = defaultBoolVal;
        }

        private void LateUpdate()
        {
            switch (valType)
            {
                case ValueType.Bool:
                    if (_lastBoolVal != boolVal)
                    {
                        _lastBoolVal = boolVal;
                        int index = boolVal ? 1 : 0;
                        material.SetInt(_propertyId, index);
                    }

                    break;
                case ValueType.Float:
                    if (_lastFloatVal != floatVal)
                    {
                        _lastFloatVal = floatVal;
                        material.SetFloat(_propertyId, floatVal);
                    }

                    break;
                case ValueType.Int:
                    if (_lastIntVal != intVal)
                    {
                        _lastIntVal = intVal;
                        material.SetInt(_propertyId, intVal);
                    }

                    break;
            }
        }
    }
}