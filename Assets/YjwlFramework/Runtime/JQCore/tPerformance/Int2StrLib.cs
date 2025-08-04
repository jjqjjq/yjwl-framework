
namespace JQCore.tPerformance
{

    public class Int2StrLib
    {
        private string[] strArr;
        public Int2StrLib(int max)
        {
            strArr = new string[max];
        }

        public void reset()
        {
            int length = strArr.Length;
            for (int i = 0; i < length; i++)
            {
                strArr[i] = null;
            }
        }

        public string IntToStr(int intVal)
        {
            if (0 <= intVal && intVal < strArr.Length)
            {
                string intStr = strArr[intVal];
                if (intStr == null)
                {
                    intStr = intVal.ToString();
                    strArr[intVal] = intStr;
                    return intStr;
                }
                return intStr;
            }
            return intVal.ToString();
        }

        public string IntToStr(uint intVal)
        {
            if (0<= intVal && intVal < strArr.Length)
            {
                string intStr = strArr[intVal];
                if (intStr == null)
                {
                    intStr = intVal.ToString();
                    strArr[intVal] = intStr;
                    return intStr;
                }
                return intStr;
            }
            return intVal.ToString();
        }
    }
}
