/*
*	作者：JJQ\Administrator
*	时间：2018-02-09 18:52:43
*/

using System.Text;

namespace JQCore.tPerformance
{
    public class JQStringBuilder
    {
        private StringBuilder _stringBuilder;

        public JQStringBuilder()
        {
            _stringBuilder = new StringBuilder(1024);
        }


        public JQStringBuilder(int capacity)
        {
            _stringBuilder = new StringBuilder(capacity);
        }

        public JQStringBuilder AppendFormat(string format, params object[] arr)
        {
            _stringBuilder.AppendFormat(format, arr);
            return this;
        }

        public override string ToString()
        {
            string str = _stringBuilder.ToString();
            _stringBuilder.Length = 0;
            return str;
        }

        public JQStringBuilder Append(object val)
        {
            _stringBuilder.Append(val);
            return this;
        }

        public JQStringBuilder Append(string val)
        {
            _stringBuilder.Append(val);
            return this;
        }
        public JQStringBuilder Append(int val)
        {
            _stringBuilder.Append(val);
            return this;
        }
        public JQStringBuilder Append(char val)
        {
            _stringBuilder.Append(val);
            return this;
        }

        public JQStringBuilder Gap()
        {
            _stringBuilder.Append('_');
            return this;
        }

        public JQStringBuilder Append(float val)
        {
            _stringBuilder.Append(val);
            return this;
        }

        public JQStringBuilder Append(uint val)
        {
            _stringBuilder.Append(val);
            return this;
        }
    }
}
