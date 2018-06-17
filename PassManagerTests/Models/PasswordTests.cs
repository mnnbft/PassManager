using Microsoft.VisualStudio.TestTools.UnitTesting;
using PassManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PassManager.Models.Tests
{
    [TestClass()]
    public class PasswordTests
    {
        [TestMethod()]
        public void GeneratePasswordTest()
        {
            var sec = Password.Instance.GeneratePassword(32, new Password.PassType[]
            {
                Password.PassType.Decimal,
                Password.PassType.Low_Alp,
            });

            var bstr = Marshal.SecureStringToBSTR(sec);
            var pass = Marshal.PtrToStringUni(bstr);
            Marshal.ZeroFreeBSTR(bstr);
        }
    }
}