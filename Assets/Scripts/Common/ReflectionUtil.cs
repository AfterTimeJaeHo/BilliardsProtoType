using System;
using System.Collections.Generic;
using System.Linq;

public static class ReflectionUtil
{
    public static Dictionary<Type, T> CreateAllInstances<T>() where T : class
    {
        var interfaceType = typeof(T);
        var derivedTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract && interfaceType.IsAssignableFrom(type))
            .ToList();

        Dictionary<Type, T> instances = new Dictionary<Type, T>();

        foreach (var type in derivedTypes)
        {
            if (Activator.CreateInstance(type) is T instance)
            {
                instances.Add(type, instance);
            }
        }

        return instances;
    }
}
