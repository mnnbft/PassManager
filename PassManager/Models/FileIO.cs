using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace PassManager.Models
{
    public sealed class FileIO
    {
        public FileIO Instance { get; } = new FileIO();
        private FileIO() { }

        public string OpenFilePath { get; set; }
        public string OpenKeyPath { get; set; }

        public static bool FileEncrypt(string filePath, string keyPath, SecureString password, ItemOperation.DataParam[] Data_a)
        {
            IntPtr bstr;

            using (FileStream outKeyFS = new FileStream(keyPath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(outKeyFS, Encoding.Unicode))
                {
                    bstr = Marshal.SecureStringToBSTR(password);
                    var BCHash = BCrypt.Net.BCrypt.HashPassword(Marshal.PtrToStringUni(bstr), workFactor: 10, enhancedEntropy: true);
                    sw.Write(BCHash);
                    Marshal.ZeroFreeBSTR(bstr);
                }
            }

            using (FileStream outFileFs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (RijndaelManaged rij = new RijndaelManaged())
                {
                    rij.BlockSize = 256;
                    rij.KeySize = 256;
                    rij.Mode = CipherMode.CBC;
                    rij.Padding = PaddingMode.PKCS7;

                    bstr = Marshal.SecureStringToBSTR(password);
                    Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(Marshal.PtrToStringUni(bstr), 32);
                    Marshal.ZeroFreeBSTR(bstr);

                    byte[] salt = new byte[32];
                    salt = deriveBytes.Salt;

                    byte[] bufferKey = deriveBytes.GetBytes(32);

                    rij.Key = bufferKey;
                    rij.GenerateIV();

                    ICryptoTransform encryptor = rij.CreateEncryptor(rij.Key, rij.IV);

                    using (CryptoStream cse = new CryptoStream(outFileFs, encryptor, CryptoStreamMode.Write))
                    {
                        var bf = new BinaryFormatter();
                        using (DeflateStream ds = new DeflateStream(cse, CompressionMode.Compress))
                        {
                            outFileFs.Write(salt, 0, 32);
                            outFileFs.Write(rij.IV, 0, 32);

                            var WriteObject = new List<ItemOperation.SaveParam>();
                            Func<ItemOperation.DataParam[], ItemOperation.DataParam[]> f = null;
                            f = x =>
                            {
                                var rtItems = new List<ItemOperation.DataParam>();

                                foreach(var y in x)
                                {
                                    if (y.Child.Count > 0)
                                        rtItems.AddRange(f(y.Child.Select(i => Functions.Copy(i, new ItemOperation.DataParam())).ToArray()));
                                    y.Child.Clear();
                                    rtItems.Add(y);
                                }
                                return rtItems.ToArray();
                            };

                            foreach (var d in f(Data_a).OrderBy(i => i.Key))
                            {
                                var obj = Functions.Copy(d, new ItemOperation.SaveParam());

                                if(d.Password != null)
                                {
                                    bstr = Marshal.SecureStringToBSTR(d.Password);
                                    obj.PasswordString = Marshal.PtrToStringUni(bstr);
                                    Marshal.ZeroFreeBSTR(bstr);
                                }
                                WriteObject.Add(obj);
                            }

                            bf.Serialize(ds, WriteObject);
                        }
                    }
                }
            }
            return true;
        }

        public static ItemOperation.DataParam[] FileDecrypt(string filePath, string keyPath, SecureString password)
        {
            IntPtr bstr;
            List<ItemOperation.SaveParam> ReadObject;
            List<ItemOperation.DataParam> rtItems = new List<ItemOperation.DataParam>(); ;

            if (string.Compare(Path.GetExtension(filePath), ".db", true) != 0)
            {
                System.Windows.MessageBox.Show("dbファイルを選択してください"
                    ,"Error", System.Windows.MessageBoxButton.OK);
                return null;
            }
            if (string.Compare(Path.GetExtension(keyPath), ".key", true) != 0)
            {
                System.Windows.MessageBox.Show("keyファイルを選択してください"
                    ,"Error", System.Windows.MessageBoxButton.OK);
                return null;
            }

            string BCHash = string.Empty;

            try
            {
                using (FileStream readKeyFS = new FileStream(keyPath, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(readKeyFS, Encoding.Unicode))
                    {
                        BCHash = sr.ReadToEnd();
                        if (string.IsNullOrEmpty(BCHash))
                        {
                            System.Windows.MessageBox.Show("keyファイル内のHashが空です"
                                ,"Error", System.Windows.MessageBoxButton.OK);

                            return null;
                        }

                        bstr = Marshal.SecureStringToBSTR(password);
                        if (!BCrypt.Net.BCrypt.EnhancedVerify(Marshal.PtrToStringUni(bstr), BCHash))
                        {
                            System.Windows.MessageBox.Show("パスワードが違います"
                                ,"Error", System.Windows.MessageBoxButton.OK);

                            return null;
                        }
                        Marshal.ZeroFreeBSTR(bstr);
                    }
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("keyファイルが見つかりません"
                    ,"Error", System.Windows.MessageBoxButton.OK);

                return null;
            }

            using (MemoryStream outKeyMS = new MemoryStream())
            {
                try
                {
                    using (FileStream readKeyFS = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (RijndaelManaged rij = new RijndaelManaged())
                        {
                            rij.BlockSize = 256;
                            rij.KeySize = 256;
                            rij.Mode = CipherMode.CBC;
                            rij.Padding = PaddingMode.PKCS7;

                            byte[] salt = new byte[32];
                            readKeyFS.Read(salt, 0, 32);

                            byte[] iv = new byte[32];
                            readKeyFS.Read(iv, 0, 32);
                            rij.IV = iv;

                            bstr = Marshal.SecureStringToBSTR(password);
                            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(Marshal.PtrToStringUni(bstr), salt);
                            Marshal.ZeroFreeBSTR(bstr);

                            byte[] bufferKey = deriveBytes.GetBytes(32);
                            rij.Key = bufferKey;

                            ICryptoTransform decryptor = rij.CreateDecryptor(rij.Key, rij.IV);

                            using (CryptoStream cse = new CryptoStream(readKeyFS, decryptor, CryptoStreamMode.Read))
                            {
                                var bf = new BinaryFormatter();
                                using (DeflateStream ds = new DeflateStream(cse, CompressionMode.Decompress))
                                {
                                    try
                                    {
                                        ReadObject = (List<ItemOperation.SaveParam>)bf.Deserialize(ds);
                                        foreach(var r in ReadObject)
                                        {
                                            var addItem = Functions.Copy(r, new ItemOperation.DataParam());
                                            addItem.Password = new SecureString();

                                            if(!string.IsNullOrEmpty(r.PasswordString))
                                            {
                                                addItem.Password = new SecureString();
                                                foreach (var c in r.PasswordString.ToCharArray())
                                                    addItem.Password.AppendChar(c);
                                            }

                                            rtItems.Add(addItem);
                                        }
                                    }
                                    catch(Exception e)
                                    {
                                        System.Windows.MessageBox.Show(string.Format("{0}", e.Message)
                                            ,"Error", System.Windows.MessageBoxButton.OK);

                                        return null;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(string.Format("{0}", e.Message)
                        ,"Error", System.Windows.MessageBoxButton.OK);

                    return null;
                }
            }
            if (rtItems.Count != 0)
                return rtItems.ToArray();
            else
                return null;
        }
    }
}
