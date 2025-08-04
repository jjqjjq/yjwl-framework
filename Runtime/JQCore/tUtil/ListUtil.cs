using System.Collections.Generic;
using System.Text;

namespace JQCore.tUtil
{

    public static class ListUtil
    {
        
        public static bool CheckArrayContains<T>(T[] arr, T checkInt)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                T temp = arr[i];
                if (temp!=null && temp.Equals(checkInt))
                {
                    return true;
                }
            }
            return false;
        }

        public static T[] AddRange<T>(List<T> list1, List<T> list2)
        {
            int count = 0;
            if (list1 != null)
            {
                count += list1.Count;
            }
            if (list2 != null)
            {
                count += list2.Count;
            }
            T[] arr = new T[count];
            int index = 0;
            if (list1 != null)
            {
                for (int i = 0; i < list1.Count; i++)
                {
                    T temp = list1[i];
                    arr[index] = temp;
                    index++;
                }
            }
            if (list2 != null)
            {
                for (int i = 0; i < list2.Count; i++)
                {
                    T temp = list2[i];
                    arr[index] = temp;
                    index++;
                }
            }
            return arr;
        }

        /// <summary>
        /// 移除过滤内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="origonArr"></param>
        /// <param name="filterArr"></param>
        /// <returns></returns>
        public static void filter<T>(List<T> origonArr, T[] filterArr)
        {
            for (int i = 0; i < filterArr.Length; i++)
            {
                T temp = filterArr[i];
                origonArr.Remove(temp);
            }
        }


        #region data transform
        public static string ListToStr(List<string> uintList)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < uintList.Count; i++)
            {
                stringBuilder.Append(uintList[i]);
                if (i != uintList.Count - 1)
                {
                    stringBuilder.Append(',');
                }
            }
            return stringBuilder.ToString();
        }


        public static string ListToStr(List<uint> uintList)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < uintList.Count; i++)
            {
                stringBuilder.Append(uintList[i]);
                if (i != uintList.Count - 1)
                {
                    stringBuilder.Append(',');
                }
            }
            return stringBuilder.ToString();
        }
        public static string ListToStr(List<int> uintList)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < uintList.Count; i++)
            {
                stringBuilder.Append(uintList[i]);
                if (i != uintList.Count - 1)
                {
                    stringBuilder.Append(',');
                }
            }
            return stringBuilder.ToString();
        }

        
        public static uint[] ListToArr(List<int> uintList)
        {
            uint[] array = new uint[uintList.Count];
            for (int i = 0; i < uintList.Count; i++)
            {
                array[i] = (uint)uintList[i];
            }
            return array;
        }

        
        public static uint[] ListToArr(List<short> uintList)
        {
            uint[] array = new uint[uintList.Count];
            for (int i = 0; i < uintList.Count; i++)
            {
                array[i] = (uint)uintList[i];
            }
            return array;
        }

        public static string ListToStr(List<short> uintList)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < uintList.Count; i++)
            {
                stringBuilder.Append(uintList[i]);
                if (i != uintList.Count - 1)
                {
                    stringBuilder.Append(',');
                }
            }
            return stringBuilder.ToString();
        }
        
        public static uint[] StrToArr(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new uint[0];
            }
            string[] dataIdStr = str.Split(',');
            uint[] uintArr = new uint[dataIdStr.Length];

            for (int i = 0; i < dataIdStr.Length; i++)
            {
                uintArr[i] = uint.Parse(dataIdStr[i]);
            }
            return uintArr;
        }
        
        public static List<uint> StrToList(string str)
        {
            List<uint> dataIdList = new List<uint>();
            if (string.IsNullOrEmpty(str))
            {
                return dataIdList;
            }
            string[] dataIdStr = str.Split(',');
            for (int i = 0; i < dataIdStr.Length; i++)
            {
                dataIdList.Add(uint.Parse(dataIdStr[i]));
            }
            return dataIdList;
        }
        public static string RemoveFromList(string listStr, uint addOne)
        {
            List<uint> list = StrToList(listStr);
            if (list.Contains(addOne))
            {
                list.Remove(addOne);
                return ListToStr(list);
            }
            return listStr;
        }

        public static string AddToList(string listStr, uint addOne)
        {
            List<uint> list = StrToList(listStr);
            if (!list.Contains(addOne))
            {
                list.Add(addOne);
                return ListToStr(list);
            }
            return listStr;
        }
        #endregion
    }
}
