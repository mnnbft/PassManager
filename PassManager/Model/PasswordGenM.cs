using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.Model
{
    using System.IO;
    using System.IO.Compression;
    using System.Security;
    using System.Security.Cryptography;
    using System.Runtime.InteropServices;

    public enum PassType : byte
    {
        Decimal,
        a_Alp,
        A_Alp,
        Symbol
    }

    public class PasswordGenM
    {
        private readonly string password_Decimal = "0123456789";
        private readonly string password_a_Alp = "abcdefghijklmnopqrstuvwxyz";
        private readonly string password_A_Alp = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private readonly string password_Symbol = "`~!@#$%^&*()_+-=[]{};:'<>,./";

        public string BCHash { get; set; }

        public SecureString GeneratePassword(int length, bool[] genparam)
        {
            var c_a = new List<char>();
            var t_list = new List<int>();
            byte e_length = (byte)Enum.GetNames(typeof(PassType)).Length;
            Random r = new Random();
            SecureString password = new SecureString();

            for (byte i = (byte)PassType.Decimal; i < e_length; i++)
            {
                if (genparam[i])
                {
                    t_list.Add(i);
                }
            }

            var t_a = t_list.ToArray();
            int[] t_a2 = t_a.OrderBy(i => Guid.NewGuid()).ToArray();
            int cnt = t_a2.Count();
            int[] passlen = new int[e_length];

            switch (cnt)
            {
                case 0:
                    return password;
                case 1:
                    passlen[t_a2[0]] = length;
                    break;
                default:
                    for (int i = 0; i < cnt; i++)
                    {
                        if (i != cnt - 1)
                        {
                            passlen[t_a2[i]] = length / cnt;
                        }
                        else
                        {
                            passlen[t_a2[i]] = length / cnt + length % cnt;
                        }
                    }
                    break;
            }

            for (int i = 0; i < cnt; i++)
            {
                switch ((PassType)t_a2[i])
                {
                    case PassType.Decimal:
                        for (int j = 0; j < passlen[(byte)PassType.Decimal]; j++)
                        {
                            c_a.Add(password_Decimal[r.Next(password_Decimal.Length)]);
                        }
                        break;
                    case PassType.a_Alp:
                        for (int j = 0; j < passlen[(byte)PassType.a_Alp]; j++)
                        {
                            c_a.Add(password_a_Alp[r.Next(password_a_Alp.Length)]);
                        }
                        break;
                    case PassType.A_Alp:
                        for (int j = 0; j < passlen[(byte)PassType.A_Alp]; j++)
                        {
                            c_a.Add(password_A_Alp[r.Next(password_A_Alp.Length)]);
                        }
                        break;
                    case PassType.Symbol:
                        for (int j = 0; j < passlen[(byte)PassType.Symbol]; j++)
                        {
                            c_a.Add(password_Symbol[r.Next(password_Symbol.Length)]);
                        }
                        break;
                }
            }

            foreach(var c in new string(c_a.OrderBy(i => Guid.NewGuid()).ToArray()))
            {
                password.AppendChar(c);
            }

            return password;
        }

        public static bool FileEncrypt(string filePath, string keyPath, SecureString password, DataParam[] nowData_a, DataParam[] addData_a)
        {
            int len;
            IntPtr bstr;
            byte[] buffer = new byte[4096];
            byte[] fileBuffer = new byte[0];

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

            foreach(var d in nowData_a)
            {
                bstr = Marshal.SecureStringToBSTR(d.Password);
                var ParentKeyStr = d.ParentKey.HasValue ? d.ParentKey.Value.ToString() : "NULL";

                fileBuffer = fileBuffer.Concat(Encoding.Unicode
                            .GetBytes(string.Format("{0},{1},{2},{3},{4},{5}" + Environment.NewLine, 
                                      d.Key, d.Title, d.UserName, Marshal.PtrToStringUni(bstr), d.Supplement, ParentKeyStr))).ToArray();

                Marshal.ZeroFreeBSTR(bstr);
            }
            foreach(var d in addData_a)
            {
                bstr = Marshal.SecureStringToBSTR(d.Password);
                var ParentKeyStr = d.ParentKey.HasValue ? d.ParentKey.Value.ToString() : "NULL";

                fileBuffer = fileBuffer.Concat(Encoding.Unicode
                            .GetBytes(string.Format("{0},{1},{2},{3},{4},{5}" + Environment.NewLine, 
                                      d.Key, d.Title, d.UserName, Marshal.PtrToStringUni(bstr), d.Supplement, ParentKeyStr))).ToArray();

                Marshal.ZeroFreeBSTR(bstr);
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
                        using (DeflateStream ds = new DeflateStream(cse, CompressionMode.Compress))
                        {
                            outFileFs.Write(salt, 0, 32);
                            outFileFs.Write(rij.IV, 0, 32);

                            using (MemoryStream ms = new MemoryStream(fileBuffer))
                            {
                                while((len = ms.Read(buffer, 0, 4096)) > 0)
                                {
                                    ds.Write(buffer, 0, len);
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        public static DataParam[] FileDecrypt(string filePath, string keyPath, SecureString password)
        {
            IntPtr bstr;
            int len;
            byte[] fileBuffer = new byte[4096];
            var rtBuffer = new List<byte>();
            string rtString = string.Empty;

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
                                using (DeflateStream ds = new DeflateStream(cse, CompressionMode.Decompress))
                                {
                                    while ((len = ds.Read(fileBuffer, 0, 4096)) > 0)
                                    {
                                        outKeyMS.Write(fileBuffer, 0, len);
                                    }
                                    rtBuffer.AddRange(outKeyMS.ToArray());
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    System.Windows.MessageBox.Show("keyファイルが見つかりません"
                        ,"Error", System.Windows.MessageBoxButton.OK);

                    return null;
                }
            }
            var rtItems = new List<DataParam>();
            using (StringReader sr = new StringReader(Encoding.Unicode.GetString(rtBuffer.ToArray())))
            {
                while (sr.Peek() > -1)
                {
                    var tmpItem = new DataParam(sr.ReadLine());
                    if (ViewModel.MainWindowVM.CurrentKey <= tmpItem.Key)
                        ViewModel.MainWindowVM.CurrentKey = tmpItem.Key + 1;
                    rtItems.Add(tmpItem);
                }
            }
            return rtItems.ToArray();
        }
    }
}
