using Microsoft.VisualStudio.TestTools.UnitTesting;
using PassManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.Model.Tests
{
    [TestClass()]
    public class PasswordGenMTests
    {
        Model.PasswordGenM model = new PasswordGenM();
        string dbPath = "C:\\Users\\user\\Desktop\\test.db";
        string keyPath = "C:\\Users\\user\\Desktop\\test.key";

        [TestMethod()]
        public void GeneratePasswordTest()
        {
            model.GeneratePassword(100, new bool[] { true, true, true, true });
            var tmp = new List<DataParam>();
            var pass = new System.Security.SecureString();

            pass.AppendChar('1');

            tmp.Add(new DataParam()
            {
                Key = 0,
                Title = "test",
                UserName = "user",
                Password = pass,
                Supplement = "supplement",
                ParentKey = null
            });
            tmp.Add(new DataParam()
            {
                Key = 1,
                Title = "test",
                UserName = "user",
                Password = pass,
                Supplement = "supplement",
                ParentKey = 0
            });
            tmp.Add(new DataParam()
            {
                Key = 2,
                Title = "test",
                UserName = "user",
                Password = pass,
                Supplement = "supplement",
                ParentKey = 1
            });

            var password = new System.Security.SecureString();

            foreach(var c in "12345".ToCharArray())
            {
                password.AppendChar(c);
            }

            Assert.AreEqual(true, PasswordGenM.FileEncrypt(dbPath, keyPath, password, new DataParam[0], tmp.ToArray()));
        }
        [TestMethod()]
        public void DecryptTest()
        {
            var password = new System.Security.SecureString();
            foreach(var c in "12345".ToCharArray())
            {
                password.AppendChar(c);
            }

            Assert.AreEqual(null, PasswordGenM.FileDecrypt(dbPath, keyPath, password));
        }
    }
}