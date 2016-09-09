using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Meta.TablesMarkers.Base;

namespace Eps.CsToDb.Builder.Common
{
    public class AttributesHelper
    {
        public static T GetAttribute<T>(Type type, T defaultValue = null) where T : Attribute
        {
            object[] attributes = type.GetCustomAttributes(typeof(T), false);
            T attribute = (attributes.Length > 0) ? attributes[0] as T : defaultValue;
            return attribute;
        }

        public static bool IsMarkedBy<T>(Type type) where T : Attribute
        {
            return GetAttribute<T>(type) != null;
        }

        public static T GetAttribute<T>(FieldInfo fieldInfo, T defaultValue = null) where T : Attribute
        {
            object[] attributes = fieldInfo.GetCustomAttributes(typeof(T), false);
            T attribute = (attributes.Length > 0) ? attributes[0] as T : defaultValue;
            return attribute;
        }
        public static bool IsMarkedBy<T>(FieldInfo fieldInfo) where T : Attribute
        {
            return GetAttribute<T>(fieldInfo) != null;
        }

        public static IEnumerable<T> GetAttributes<T>(Type type) where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T)).OfType<T>();
        }
    }
}
