using System;

namespace Waving.Di
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ForTypesAttribute : Attribute
    {
        public Type[] TargetTypes { get; }

        public ForTypesAttribute(params Type[] targetTypes)
        {
            TargetTypes = targetTypes;
        }
    }   
}