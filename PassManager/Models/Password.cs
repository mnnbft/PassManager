using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Collections.ObjectModel;

namespace PassManager.Models
{
    public sealed class Password
    {
        public static Password Instance { get; } = new Password();
        private Password() { }

        
        public enum PassType : byte
        {
            Decimal,
            Low_Alp,
            Up_Alp,
            Symbol
        }

        Dictionary<PassType, string> passwordDictionary = 
            new Dictionary<PassType, string>()
            {
                { PassType.Decimal, "0123456789" },
                { PassType.Low_Alp, "abcdefghijklmnopqrstuvwxyz" },
                { PassType.Up_Alp, "ABCDEFGHIJKLMNOPQRSTUVWXYZ" },
                { PassType.Symbol, "`~!@#$%^&*()_+-=[]{};:'<>,./" },
            };

        public SecureString GeneratePassword(int length, PassType[] passTypes)
        {
            var result = new List<char>();
            var password = new SecureString();

            var countDictionary = new Dictionary<PassType, int>();
            var typeCount = Functions.IntSlice(length, passTypes.Length);
            for(int i = 0; i < passTypes.Length; i++)
            {
                var type = passTypes[i];
                var count = typeCount[i];
                countDictionary.Add(type, count);
            }

            var rand = new Random();
            foreach(var dictionary in countDictionary)
            {
                var type = dictionary.Key;
                int count = countDictionary[type];
                int maxValue = passwordDictionary[type].Length;

                for(int i = 0; i < dictionary.Value; i++)
                {
                    int randNum = rand.Next(0, maxValue);
                    result.Add(passwordDictionary[type][randNum]);
                }
            }

            result = result.OrderBy(i => Guid.NewGuid()).ToList();
            result.ForEach(i => password.AppendChar(i));

            return password;
        }

        public void CreateNewFile(string fileName, string filePath, string keyPath, SecureString password)
        {
            var fullFilePath = Path.Combine(filePath, fileName + ".pass");
            var fullKeyPath = Path.Combine(filePath, fileName + ".key");

            var folderItems = new ObservableCollection<FolderItem>();
            folderItems.Add(new FolderItem(false) { Title = fileName });

            FileIO.Instance.FileEncrypt(fullFilePath, fullKeyPath, password, folderItems);

            OpenFile(fullFilePath, fullKeyPath, password);
        }

        public void OpenFile(string filePath, string keyPath, SecureString password)
        {
            var folders = FileIO.Instance.FileDecrypt(filePath, keyPath, password);
            FileIO.Instance.OpenedFile.Open(filePath, keyPath, password, folders);
        }
    }
}
