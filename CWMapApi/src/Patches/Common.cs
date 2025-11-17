using System;
using System.Reflection;
#pragma warning disable CS0168 // Variable is declared but never used

namespace CWMapApi.Patches
{
    internal class Common
    {
        internal static object InvokeMethod<T>(T instance, string methodName, object[] args)
        {
            try
            {
                MethodInfo method = typeof(T).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
                return method?.Invoke(instance, args);
            }
            catch (NullReferenceException e)
            {
                LogNRE(typeof(T), methodName, "InvokeMethod");
                throw;
            }
        }

        internal static R GetField<T, R>(T instance, string fieldName)
        {
            try
            {
                FieldInfo field = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                return (R)field?.GetValue(instance);
            }
            catch (NullReferenceException e)
            {
                LogNRE(typeof(T), fieldName, "GetField");
                throw;
            }
        }
        
        internal static void SetField<T>(T instance, string fieldName, object value)
        {
            try
            {
                FieldInfo field = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                field?.SetValue(instance, value);
            }
            catch (NullReferenceException e)
            {
                LogNRE(typeof(T), fieldName, "SetField");
                throw;
            }
        }
        
        internal static void SetStaticProperty(Type type, string fieldName, object value)
        {
            try
            {
                type
                    .GetProperty(fieldName, BindingFlags.Static | BindingFlags.Public)?
                    .SetValue(null, value);
            }
            catch (NullReferenceException e)
            {
                LogNRE(type, fieldName, "SetField");
                throw;
            }
        }
        
        internal static void SetStaticField(Type type, string fieldName, object value)
        {
            try
            {
                FieldInfo field = type.GetField(fieldName,
                    BindingFlags.NonPublic | BindingFlags.Static);
                field?.SetValue(null, value);
            }
            catch (NullReferenceException e)
            {
                LogNRE(type, fieldName, "SetField");
                throw;
            }
        }

        private static void LogNRE(Type type, string subjectName, string methodName)
        {
            MapApi.log.LogError($"NRE on {type.Name}.{subjectName} in {methodName}");
        }
    }
}