using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;

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
            var sProperties = src.GetType()
                              .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                              .Where(i => i.CanRead && i.CanWrite);
            var dProperties = dest.GetType()
                             .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             .Where(i => i.CanRead && i.CanWrite);

            var properties = sProperties.Join(dProperties, x => 
                new { x.Name, x.PropertyType }, x => 
                    new { x.Name, x.PropertyType }, (x, y) => 
                        new { x, y });

            foreach (var p in properties)
            { p.y.SetValue(dest, p.x.GetValue(src)); }
                
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

        public static bool SecureStringEquals(SecureString secure1, SecureString secure2)
        {
            if (secure1 == null && secure2 == null) 
            { return true; }
            if (secure1 == null || secure2 == null) 
            { return false; }
            if (secure1.Length != secure2.Length) 
            { return false; }

            var ptr1 = Marshal.SecureStringToBSTR(secure1);
            var ptr2 = Marshal.SecureStringToBSTR(secure2);

            try
            {
                return Enumerable.Range(0, secure1.Length)
                       .All(i => Marshal.ReadInt16(ptr1, i) == Marshal.ReadInt16(ptr2, i));
            }
            finally
            {
                Marshal.ZeroFreeBSTR(ptr1);
                Marshal.ZeroFreeBSTR(ptr2);
            }
        }

        public static string SecureStringToString(SecureString secure)
        {
            var bstr = Marshal.SecureStringToBSTR(secure);
            var charString = Marshal.PtrToStringUni(bstr);
            Marshal.ZeroFreeBSTR(bstr);
            return charString;
        }

        public static SecureString StringToSecureString(string password)
        {
            var result = new SecureString();

            foreach(var passChar in password)
            { result.AppendChar(passChar); }

            return result;
        }
    }
}
