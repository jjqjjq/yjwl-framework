using System;

namespace JQFramework.tMVC.Attributes
{
    
    [AttributeUsage(AttributeTargets.Class)]
    public class SubViewNameAttribute: Attribute
    {
        public string Name { get; }

        public SubViewNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}