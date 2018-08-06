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
using BCrypt.Net;
using FolderItem = PassManager.Models.ItemOperation.FolderItem;
using SerializeFolder = PassManager.Models.ItemOperation.SerializeFolder;

namespace PassManager.Models
{
    public sealed class FileIO
    {
        public FileIO Instance { get; } = new FileIO();
        private FileIO() { }

        public string SelectedFilePath { get; set; }
        public string SelectedKeyPath { get; set; }

        public static bool FileEncrypt(string filePath, 
                                       string keyPath, 
                                       SecureString password,
                                       List<FolderItem> FolderItems)
        {
            var passString = Functions.SecureStringProcessing(password);

            try
            {
                using (var outKeyFS = new FileStream(keyPath, FileMode.Create, FileAccess.Write))
                {
                    using (var sw = new StreamWriter(outKeyFS, Encoding.Unicode))
                    {
                        var bcryptHash = BCrypt.Net.BCrypt.HashPassword(passString, 10, true);
                        sw.Write(bcryptHash);
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
                return false;
            }

            try
            {
                using (var outFileFs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    using (var rij = new RijndaelManaged())
                    {
                        rij.BlockSize = 256;
                        rij.KeySize = 256;
                        rij.Mode = CipherMode.CBC;
                        rij.Padding = PaddingMode.PKCS7;

                        var deriveBytes = new Rfc2898DeriveBytes(passString, 32);

                        byte[] salt = new byte[32];
                        salt = deriveBytes.Salt;

                        byte[] bufferKey = deriveBytes.GetBytes(32);

                        rij.Key = bufferKey;
                        rij.GenerateIV();

                        var encryptor = rij.CreateEncryptor(rij.Key, rij.IV);

                        using (var cse = new CryptoStream(outFileFs, encryptor, CryptoStreamMode.Write))
                        {
                            var bf = new BinaryFormatter();
                            using (var ds = new DeflateStream(cse, CompressionMode.Compress))
                            {
                                outFileFs.Write(salt, 0, 32);
                                outFileFs.Write(rij.IV, 0, 32);

                                var SerializeFolders = ItemOperation.GetSerializeFolders(FolderItems).ToList();
                                bf.Serialize(ds, SerializeFolders);
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
                return false;
            }

            return true;
        }

        public static List<FolderItem> FileDecrypt(string filePath,
                                                   string keyPath, 
                                                   SecureString password)
        {
            var passString = Functions.SecureStringProcessing(password);

            if (string.Compare(Path.GetExtension(filePath), ".pass", true) != 0)
            {
                return null;
            }
            if (string.Compare(Path.GetExtension(keyPath), ".key", true) != 0)
            {
                return null;
            }

            try
            {
                using (FileStream readKeyFS = new FileStream(keyPath, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(readKeyFS, Encoding.Unicode))
                    {
                        var bcryptHash = sr.ReadToEnd();
                        if (!BCrypt.Net.BCrypt.EnhancedVerify(passString, bcryptHash))
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
                return null;
            }

            var FolderItems = new List<FolderItem>();
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

                            var deriveBytes = new Rfc2898DeriveBytes(passString, salt);

                            byte[] bufferKey = deriveBytes.GetBytes(32);
                            rij.Key = bufferKey;

                            var decryptor = rij.CreateDecryptor(rij.Key, rij.IV);

                            using (var cse = new CryptoStream(readKeyFS, decryptor, CryptoStreamMode.Read))
                            {
                                var bf = new BinaryFormatter();
                                using (var ds = new DeflateStream(cse, CompressionMode.Decompress))
                                {
                                    try
                                    {
                                        var SerializeFolders = (List<SerializeFolder>)bf.Deserialize(ds);
                                        FolderItems = ItemOperation.GetDeSerializeFolders(SerializeFolders).ToList();
                                    }
                                    catch(Exception e)
                                    {
                                        System.Windows.MessageBox.Show(e.Message);
                                        return null;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.Message);
                    return null;
                }
            }
            return FolderItems;
        }
    }
}
