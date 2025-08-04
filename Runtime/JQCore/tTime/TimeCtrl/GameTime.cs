using JQCore.tTime;
using UnityEngine;

namespace JQCore.tTime
{
    public interface ITimeOwner
    {
    }

    public class GameTime
    {
        private const int targetFrameRate = 10; //每秒10帧计算
        public const float staticFrameTime = 1 / targetFrameRate;

        private ITimeOwner _owner;
        private float _time;
        private float _tempDeltaTime;
        private float _deltaTime;
        private int _frameCount;
        private float _timeScale = 1;
        
        public float Time => _time;
        public float DeltaTime => _deltaTime;
        public int FrameCount => _frameCount;
        public float TimeScale => _timeScale;
        private float _missionLockTime;

        public GameTime(ITimeOwner owner, GlobalClockKey globalClockKey)
        {
            _owner = owner;
            _time = 0;
            _tempDeltaTime = 0;
            _deltaTime = 0;
            _frameCount = 0;
            _timeScale = 1;
            //todo: TimeCtrlMgr
            // if globalClockKey then
            // Singleton(EClass.TimeCtrlMgr):addGameTime(globalClockKey, self)
            // end
        }


        public void start()
        {
            _tempDeltaTime = 0;
            _time = 0;
            _frameCount = 0;
        }

        public void stop()
        {
            _tempDeltaTime = 0;
            _time = 0;
            _frameCount = 0;
        }

        public int getPassTick()
        {
            _tempDeltaTime +=  SysTime.deltaTime * _timeScale;
            int passTick = Mathf.FloorToInt(_tempDeltaTime / staticFrameTime);
            _tempDeltaTime -= passTick * staticFrameTime;
            return passTick;
        }

        public void addOneTick()
        {
            _frameCount++;
            _deltaTime = staticFrameTime;
            _time += staticFrameTime;
        }

        public void onTick()
        {
            _frameCount = SysTime.frameCount;
            _deltaTime = SysTime.deltaTime * _timeScale;
            _time += _deltaTime;

        }

        public void Dispose()
        {
            _owner = null;
        }
        
        #region LockCtrl

        public void SetMissionLockTime(float addTime)
        {
            float time = _time + addTime;
            if (time > _missionLockTime)
            {
                _missionLockTime = time;
            }
        }

        public bool IsMissionPlaying()
        {
            if (_time < _missionLockTime) return true;
            return false;
        }

        #endregion
    }
}