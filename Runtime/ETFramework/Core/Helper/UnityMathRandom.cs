#if UNITY

using System;
using System.Collections.Generic;
using JQCore.tLog;
using Random = Unity.Mathematics.Random;

namespace ET
{
    public class UnityMathRandom
    {
        private Random _random = new Random();
        private Dictionary<string, int> _randomStateDic;
        private uint _seed;
        private string _lastKey;


        public void SetState(string key)
        {
            if (this._lastKey == key)
            {
                return;
            }

            Save();

            bool success = _randomStateDic.TryGetValue(key, out int intState);
            uint state = this.IntToUint(intState);
            if (!success)
            {
                state = this._seed;
                this._randomStateDic[key] = this.UintToInt(state);
            }

            // Log.Console($"InitState key:{key} state:{state}");
            _random.InitState(state);
            this._lastKey = key;
        }

        public void Save()
        {
            //把上一个key的state保存起来
            if (!string.IsNullOrEmpty(_lastKey))
            {
                this._randomStateDic[this._lastKey] = UintToInt(this._random.state);
                this._lastKey = null;
            }
        }

        public void SetSeed(int seed, Dictionary<string, int> randomStateDic)
        {
            _seed = this.IntToUint(seed);
            _randomStateDic = randomStateDic;
        }

        private uint IntToUint(int value)
        {
            if (value < 0)
                return (uint)(int.MaxValue + value + 1);
            else
                return (uint)value + int.MaxValue + 1;
        }

        private int UintToInt(uint value)
        {
            if (value > int.MaxValue)
                return (int)(value - int.MaxValue - 1);
            else
                return (int)value - int.MaxValue - 1;
        }

        //传入一个数组， 从数组中随机选择N个
        public List<T> RandomArray<T>(List<T> array, int count)
        {
            count = Math.Min(count, array.Count);
            List<T> result = new List<T>();
            List<T> temp = new List<T>(array);
            for (int i = 0; i < count; i++)
            {
                int index = RandomNumber(0, temp.Count);
                result.Add(temp[index]);
                temp.RemoveAt(index);
            }

            return result;
        }

        public List<int> RandomArray(int numMax, int count)
        {
            List<int> array = new List<int>();
            for (int i = 0; i < numMax; i++)
            {
                array.Add(i);
            }

            return RandomArray(array, count);
        }

        public ulong RandUInt64(uint seed)
        {
            int r1 = RandInt32();
            int r2 = RandInt32();
            return ((ulong)r1 << 32) & (ulong)r2;
        }

        public int RandInt32()
        {
            return this._random.NextInt();
        }

        public uint RandUInt32()
        {
            return this._random.NextUInt();
        }

        public long RandInt64()
        {
            uint r1 = RandUInt32();
            uint r2 = RandUInt32();
            return (long)(((ulong)r1 << 32) | r2);
        }

        /// <summary>
        /// 获取lower与Upper之间的随机数,包含下限，不包含上限 [lower, upper)
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public int RandomNumber(int lower, int upper)
        {
            // uint oldState = this._random.state;
            int value = this._random.NextInt(lower, upper);
            // Log.Console($"oldState:{oldState} state:{this._random.state} value:{value}");
            return value;
        }

        /// <summary>
        /// 计算概率是否成功
        /// </summary>
        /// <returns></returns>
        public bool RandomRate(int rate)
        {
            return RandomNumber(0, 10000) < rate;
        }

        public bool RandomBool()
        {
            return this._random.NextInt(2) == 0;
        }

        public T RandomArray<T>(T[] array)
        {
            return array[RandomNumber(0, array.Length)];
        }

        public T RandomArray<T>(List<T> array)
        {
            return array[RandomNumber(0, array.Count)];
        }
        
        //表现随机
        public List<T> ShuffleListDisplay<T>(List<T> arr)
        {
            if (arr == null || arr.Count < 2)
            {
                return null;
            }

            List<T> temp = new List<T>(arr);
            for (int i = 0; i < temp.Count - 1; i++)
            {
                int index = UnityEngine.Random.Range(i, temp.Count);
                (temp[index], temp[i]) = (temp[i], temp[index]);
            }
            return temp;
        }

        /// <summary>
        /// 打乱数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr">要打乱的数组</param>
        public void ShuffleList<T>(List<T> arr)
        {
            if (arr == null || arr.Count < 2)
            {
                return;
            }

            Random random = this._random;
            for (int i = 0; i < arr.Count - 1; i++)
            {
                int index = random.NextInt(i, arr.Count);
                (arr[index], arr[i]) = (arr[i], arr[index]);
            }
        }

        public float RandFloat01()
        {
            int a = RandomNumber(0, 1000000);
            return a / 1000000f;
        }


        //传入概率数组，返回随机选择的索引
        public int RandomIndex(int[] rateArr)
        {
            int sum = 0;
            foreach (int rate in rateArr)
            {
                sum += rate;
            }

            int rand = RandomNumber(0, sum);
            int index = 0;
            for (int i = 0; i < rateArr.Length; i++)
            {
                if (rand < rateArr[i])
                {
                    index = i;
                    break;
                }
                else
                {
                    rand -= rateArr[i];
                }
            }

            return index;
        }
    }
}
#endif