using System;
using JQFramework.tTimeCtrl.Clock;
using UnityEngine;

namespace JQFramework.tTimeCtrl.TimeLine
{

    public class TC_TimeLine : MonoBehaviour
    {
        /// <summary>
        /// The time scale of the timeline, computed from all observed clocks. For more information, see Clock.timeScale. 
        /// </summary>
        public float timeScale;

        public string globalClockKey;
        
        public TC_GlobalClock globalClock;
        public TC_LocalClock localClock;

        public Animator[] animatorArr;
        public Animation[] animationArr;
        public ParticleSystem[] particleSystemArr;
        public float[] particleSimulationSpeedArr;
        // public SkeletonAnimation[] skeletonAnimationArr;
        public AudioSource[] audioSourceArr;
        public float[] pitchArr;
        public bool isCached = false;


        private Action<float> _timeScaleChangeAction;

        void Awake()
        {
            timeScale = 1;
            if (!string.IsNullOrEmpty(globalClockKey))
            {
                CacheClocks(globalClockKey);
            }
            CacheComponents();
        }

        //资源制作时调用
        public void CacheComponents()
        {
            if (isCached) return;
            isCached = true;
            animatorArr = GetComponentsInChildren<Animator>(true);
            animationArr = GetComponentsInChildren<Animation>(true);
            particleSystemArr = GetComponentsInChildren<ParticleSystem>(true);
            particleSimulationSpeedArr = new float[particleSystemArr.Length];
            for (int i = 0; i < particleSystemArr.Length; i++)
            {
                ParticleSystem particleSystem = particleSystemArr[i];
                ParticleSystem.MainModule main = particleSystem.main;
                particleSimulationSpeedArr[i] = main.simulationSpeed;
            }
            // skeletonAnimationArr = GetComponentsInChildren<SkeletonAnimation>(true);
            audioSourceArr = GetComponentsInChildren<AudioSource>(true);
            pitchArr = new float[audioSourceArr.Length];
            for (int i = 0; i < audioSourceArr.Length; i++)
            {
                AudioSource audioSource = audioSourceArr[i];
                pitchArr[i] = audioSource.pitch;
            }
        }

        public void SetTimeScaleAction(Action<float> timeScaleChangeAction)
        {
            _timeScaleChangeAction = timeScaleChangeAction;
        }

        public void CacheClocks(string key)
        {
            globalClockKey = key;
            globalClock = TC_Timekeeper.GetClock(key);
            localClock = GetComponent<TC_LocalClock>();
            AdjustProperties();
        }

        void Update()
        {
            AdjustProperties();
        }

        private void AdjustProperties()
        {
            float currTimeScale = finalTimeScale;
            if (currTimeScale != timeScale)
            {
                timeScale = currTimeScale;
                CacheComponents();
                foreach (Animator animator in animatorArr)
                {
                    animator.speed = timeScale;
                }

                foreach (Animation animation in animationArr)
                {
                    foreach (AnimationState state in animation)
                    {
                        state.speed = timeScale;
                    }
                }
                
                for (int i = 0; i < particleSystemArr.Length; i++)
                {
                    ParticleSystem particleSystem = particleSystemArr[i];
                    if (particleSystem != null)
                    {
                        ParticleSystem.MainModule main = particleSystem.main;
                        float baseSimulationSpeed = particleSimulationSpeedArr[i];
                        main.simulationSpeed = baseSimulationSpeed * timeScale;
                    }
                }


                // foreach (SkeletonAnimation skeletonAnimation in skeletonAnimationArr)
                // {
                //     skeletonAnimation.timeScale = timeScale;
                // }

                for (int i = 0; i < audioSourceArr.Length; i++)
                {
                    AudioSource audioSource = audioSourceArr[i];
                    float basePitch = pitchArr[i];
                    audioSource.pitch = basePitch * timeScale;
                }

                _timeScaleChangeAction?.Invoke(timeScale);

            }
        }

        private float finalTimeScale
        {
            get
            {
                float scale = 1f;
                if (globalClock != null)
                {
                    if (globalClock.parentBlend == ClockBlend.Multiplicative)
                    {
                        scale *= globalClock.timeScale;
                    }
                    else
                    {
                        scale += globalClock.timeScale;
                    }
                }

                if (localClock != null)
                {
                    if (localClock.parentBlend == ClockBlend.Multiplicative)
                    {
                        scale *= localClock.timeScale;
                    }
                    else
                    {
                        scale += localClock.timeScale;
                    }
                }
                return scale;
            }
        }
    }
}
