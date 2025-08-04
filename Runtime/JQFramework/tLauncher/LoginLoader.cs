using System;
using JQCore.tLoader;

namespace JQFramework.tLauncher
{
    public class LoginLoader:BaseLoader
    {
        private Action _action;
        
        public LoginLoader(Action action):base("LoginLoader")
        {
            _action = action;
            initTotal(1);
        }
        
        public override void start()
        {
            base.start();
            _action?.Invoke();
            finishOne();
            finishAll();
        }
        
    }
}