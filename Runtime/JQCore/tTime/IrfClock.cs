using System;
using System.Collections.Generic;
using UnityEngine;

namespace JQCore.tTime
{

    public class JQClock
    {
        private int _frameCount;
        private float _deltaTime;
        private float _gameTime;


        public void onTick()
        {
            _gameTime += Time.deltaTime;
            _deltaTime = Time.deltaTime;
            _frameCount++;
        }

        public float currTime
        {
            get { return _gameTime; }
        }

        public float deltaTime
        {
            get { return _deltaTime; }
        }
        
        public int frameCount
        {
            get { return _frameCount; }
        }
    }
}
