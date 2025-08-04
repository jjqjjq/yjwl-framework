using System;
using System.Collections;
using JQCore.tLog;
using UnityEngine;

namespace JQCore.tCoroutine
{
    public class JQCoroutineHandler : MonoBehaviour
    {
        protected static JQCoroutineHandler m_Instance;

        public static JQCoroutineHandler instance
        {
            get
            {
                if (m_Instance == null)
                {
                    var o = new GameObject("CoroutineHandler");
                    DontDestroyOnLoad(o);
                    m_Instance = o.AddComponent<JQCoroutineHandler>();
                }

                return m_Instance;
            }
        }

        private void Awake()
        {
            m_Instance = this;
        }

        public static Coroutine Start(IEnumerator coroutine)
        {
            return instance.StartCoroutine(coroutine);
        }

        public void OnDisable()
        {
            if (m_Instance)
                Destroy(m_Instance.gameObject);
        }
    }
}