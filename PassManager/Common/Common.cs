using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.Common
{
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;

    public class Common
    {
        public static T DeepCopy<T>(T target)
        {
            T result;
            BinaryFormatter bf = new BinaryFormatter();
            using(MemoryStream ms = new MemoryStream())
            {
                try
                {
                    bf.Serialize(ms, target);
                    ms.Position = 0;
                    result = (T)bf.Deserialize(ms);
                }
                finally { }
            }
            return result;
        }
        public static T2 DeepCopy2<T1, T2>(T1 target)
        {
            T2 result;
            BinaryFormatter bf = new BinaryFormatter();
            using(MemoryStream ms = new MemoryStream())
            {
                try
                {
                    bf.Serialize(ms, target);
                    ms.Position = 0;
                    result = (T2)bf.Deserialize(ms);
                }
                finally { }
            }
            return result;
        }
        public static T Copy<T>(object src, T dest)
        {
            if (src == null || dest == null) return dest;
            var sProperties = src.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(i => i.CanRead && i.CanWrite);
            var dProperties = dest.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(i => i.CanRead && i.CanWrite);
            var properties = sProperties.Join(dProperties, x => new { x.Name, x.PropertyType }, x => new { x.Name, x.PropertyType }, (x, y) => new { x, y });
            foreach (var p in properties)
            {
                p.y.SetValue(dest, p.x.GetValue(src));
            }
            return dest;
        }
    }
}
