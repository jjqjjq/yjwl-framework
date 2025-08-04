namespace JQCore.tSingleton
{
    public interface IJQSingleton
    {
        public void Dispose();

        public int GetDisposePriority();
    }
}