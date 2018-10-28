using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace PassManager.Models
{
    public class OpenedFileInfo : BindableBase
    {
        public OpenedFileInfo() { }

        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { SetProperty(ref filePath, value); }
        }
        private string keyPath;
        public string KeyPath
        {
            get { return keyPath; }
            set { SetProperty(ref keyPath, value); }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }
        private ObservableCollection<FolderItem> folders;
        public ObservableCollection<FolderItem> Folders
        {
            get { return folders; }
            set { SetProperty(ref folders, value); }
        }

        public void Open(string filePath, string keyPath, string password, ObservableCollection<FolderItem> folders)
        {
            FilePath = filePath;
            KeyPath = keyPath;
            Password = password;
            Folders = folders;
        }
        public void Close()
        {
            FilePath = "";
            KeyPath = "";
            password = "";
            Folders.Clear();
        }
    }

    public sealed class FileIO
    {
        public static FileIO Instance { get; } = new FileIO();

        private FileIO() { }

        public OpenedFileInfo OpenedFile { get; set; } = new OpenedFileInfo();

        private void WriteBcryptHash(string bcryptHash, string keyPath)
        {
            try
            {
                using (var fs = new FileStream(keyPath, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs, Encoding.Unicode))
                { sw.Write(bcryptHash); }
            }
            catch (Exception)
            {
            }
        }

        public bool FileEncrypt(string filePath, string keyPath, string password, ObservableCollection<FolderItem> FolderItems)
        {
            var bcryptHash = BCrypt.Net.BCrypt.HashPassword(password, 10, true);

            WriteBcryptHash(bcryptHash, keyPath);

            try
            {
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                using (var rij = new RijndaelManaged())
                {
                    rij.BlockSize = 256;
                    rij.KeySize = 256;
                    rij.Mode = CipherMode.CBC;
                    rij.Padding = PaddingMode.PKCS7;

                    var deriveBytes = new Rfc2898DeriveBytes(password, 32);

                    byte[] salt = new byte[32];
                    salt = deriveBytes.Salt;

                    byte[] bufferKey = deriveBytes.GetBytes(32);

                    rij.Key = bufferKey;
                    rij.GenerateIV();

                    var encryptor = rij.CreateEncryptor(rij.Key, rij.IV);

                    using (var cse = new CryptoStream(fs, encryptor, CryptoStreamMode.Write))
                    using (var ds = new DeflateStream(cse, CompressionMode.Compress))
                    {
                        var bf = new BinaryFormatter();

                        fs.Write(salt, 0, 32);
                        fs.Write(rij.IV, 0, 32);

                        var json = JsonConvert.SerializeObject(FolderItems);
                        bf.Serialize(ds, json);
                    }
                }
            }
            catch(Exception)
            {
            }

            return true;
        }

        private bool ReadBcryptHash(string passString, string keyPath)
        {
            try
            {
                using (var fs= new FileStream(keyPath, FileMode.Open, FileAccess.Read))
                using (var sr = new StreamReader(fs, Encoding.Unicode))
                {
                    var bcryptHash = sr.ReadToEnd();
                    if (BCrypt.Net.BCrypt.EnhancedVerify(passString, bcryptHash))
                    { return true; }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public ObservableCollection<FolderItem> FileDecrypt(string filePath, string keyPath, string password)
        {
            if (string.Compare(Path.GetExtension(filePath), ".pass", true) != 0)
            { return null; }
            if (string.Compare(Path.GetExtension(keyPath), ".key", true) != 0)
            { return null; }

            if (ReadBcryptHash(password, keyPath) == false)
            { return null; }

            var FolderItems = new ObservableCollection<FolderItem>();
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (var rij = new RijndaelManaged())
                {
                    rij.BlockSize = 256;
                    rij.KeySize = 256;
                    rij.Mode = CipherMode.CBC;
                    rij.Padding = PaddingMode.PKCS7;

                    byte[] salt = new byte[32];
                    fs.Read(salt, 0, 32);

                    byte[] iv = new byte[32];
                    fs.Read(iv, 0, 32);
                    rij.IV = iv;

                    var deriveBytes = new Rfc2898DeriveBytes(password, salt);

                    byte[] bufferKey = deriveBytes.GetBytes(32);
                    rij.Key = bufferKey;

                    var decryptor = rij.CreateDecryptor(rij.Key, rij.IV);

                    using (var cse = new CryptoStream(fs, decryptor, CryptoStreamMode.Read))
                    using (var ds = new DeflateStream(cse, CompressionMode.Decompress))
                    {
                        var bf = new BinaryFormatter();

                        var json = (string)bf.Deserialize(ds);
                        var tmp = JsonConvert.DeserializeObject(json.ToString(), typeof(ObservableCollection<FolderItem>));

                        FolderItems = (ObservableCollection<FolderItem>)tmp;
                    }
                }
            }
            catch (Exception)
            {
            }
            return FolderItems;
        }
    }
}
