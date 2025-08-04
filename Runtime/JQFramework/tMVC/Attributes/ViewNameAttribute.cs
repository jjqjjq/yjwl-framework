using System;

namespace JQFramework.tMVC.Attributes
{
    
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewNameAttribute: Attribute
    {
        public string Name { get; }

        public ViewNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}