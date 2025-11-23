using System;
using System.Collections.Generic;
using System.Reflection;

namespace Waving.Di
{
    public abstract class DIContainerBase
    {
        private static List<DIContainerBase> _containers = new();

        private HashSet<Type> _allowedTypes = new(); // ForTypes에 의해 허용된 타입

        protected DIContainerBase()
        {
            _containers.Add(this);
            LoadAllowedTypes();
        }

        // Load [ForTypes]
        private void LoadAllowedTypes()
        {
            var attr = GetType().GetCustomAttribute<ForTypesAttribute>();
            if (attr == null)
                return;

            foreach (var t in attr.TargetTypes)
                _allowedTypes.Add(t);
        }

        // 외부 객체가 생성될 때 자동으로 호출됨 (DIClass 덕분)
        public static void TryInjectAll(object instance)
        {
            Type instanceType = instance.GetType();

            foreach (var container in _containers)
            {
                // ForTypes와 하위타입까지 허용
                if (!container.IsAllowedType(instanceType))
                    continue;

                container.Inject(instance);
            }
        }

        private bool IsAllowedType(Type t)
        {
            foreach (var allowed in _allowedTypes)
            {
                if (allowed == t)
                    return true;

                if (allowed.IsAssignableFrom(t))
                    return true;
            }

            return false;
        }

        // 실제 Inject 실행
        private void Inject(object target)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (var field in target.GetType().GetFields(flags))
            {
                if (field.IsDefined(typeof(InjectAttribute), true))
                {
                    // 주입 대상이 "컨테이너 자신"일 때만 주입
                    if (field.FieldType.IsAssignableFrom(GetType()))
                    {
                        field.SetValue(target, this);
                    }
                }
            }
        }
    }
}