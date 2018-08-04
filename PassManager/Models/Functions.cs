using System.Linq;
using System.IO;
using System.Security;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace PassManager.Models
{
    public sealed class Functions 
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

        public static int[] IntSlice(int value, int count)
        {
            int division = value / count;
            int surplus = value % count;

            var result = new List<int>();
            for(int i = 0; i < count; i++)
            {
                int target = division;
                target += surplus-- > 0 ? 1 : 0;
                result.Add(target);
            }

            return result.ToArray();
        }

        public static string SecureStringProcessing(SecureString secure)
        {
            var bstr = Marshal.SecureStringToBSTR(secure);
            var charString = Marshal.PtrToStringUni(bstr);
            Marshal.ZeroFreeBSTR(bstr);
            return charString;
        }

        public static SecureString SecureStringProcessing(string password)
        {
            var result = new SecureString();
            foreach(var passChar in password)
            {
                result.AppendChar(passChar);
            }
            return result;
        }
    }
}
