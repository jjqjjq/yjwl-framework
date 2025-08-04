namespace JQFramework.tTimeCtrl.Clock
{
    //组时钟

    public class TC_GlobalClock: TC_Clock
    {
        public string key;

        public void Awake()
        {
            if (!string.IsNullOrEmpty(key))
            {
                TC_Timekeeper.AddClock(this);
            }
        }

        public void SetKey(string k)
        {
            key = k;
            if (!string.IsNullOrEmpty(key))
            {
                TC_Timekeeper.AddClock(this);
            }
        }
    }
}
