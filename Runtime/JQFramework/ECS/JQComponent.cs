using JQCore.tEvent;
using JQCore.tString;
using JQCore.tTime;

namespace JQCore.ECS
{
    public class JQComponent
    {
        private JQEventDispatcher _irfEventDispatcher = null;

        public JQEventDispatcher eventDispatcher => _irfEventDispatcher;

        private int _executePriority;

        public int ExecutePriority => _executePriority;

        public int NameByteSum => _nameByteSum;

        private int _nameByteSum;

        protected JQEntity _entity;
        public JQEntity Entity => _entity;
        private string _name;
        protected GameTime _gameTime;
        
        public JQComponent(string name, int executePriority, bool needEvent = false)
        {
            _name = name;
            byte[] byteArray = name.ToByteArray();
            for (int i = 0; i < byteArray.Length; i++)
            {
                _nameByteSum += byteArray[i];
            }
            
            _executePriority = executePriority;
            if (needEvent)
            {
                _irfEventDispatcher = new JQEventDispatcher();
            }
        }

        public void SetEntity(JQEntity entity)
        {
            _entity = entity;
        }

        public virtual void onPause()
        {
            
        }

        public virtual void onStart()
        {
            
        }

        public void SetGameTime(GameTime gameTime)
        {
            _gameTime = gameTime;
        }
        

        public virtual void onReset()
        {
            _gameTime = null;
            if (_irfEventDispatcher!= null)
            {
                _irfEventDispatcher.EventDispose();
            }
        }

        public virtual void Dispose()
        {
            onReset();
            _irfEventDispatcher = null;
        }
    }
}