using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Security;
using Prism.Mvvm;

namespace PassManager.Models
{
    public sealed class ItemOperation : BindableBase
    {
        public static ItemOperation Instance { get; } = new ItemOperation();
        private ItemOperation() { }

        public class FolderItem
        {
            public string Title { get; set; }
            public int Key { get; set; }
            public int? ParentKey { get; set; }
            public List<PasswordItem> Items { get; set; }
        }
        public class PasswordItem
        {
            public PasswordItem(SerializePassword item)
            {
                if (item != null)
                {
                    Title = item.Title;
                    Key = item.Key;
                    FolderKey = item.FolderKey;
                    UserName = item.UserName;
                    Memos = new ObservableCollection<string>(item.Memos);
                    Password = Functions.SecureStringProcessing(item.PasswordString);
                }
            }

            public string Title { get; set; }
            public int Key { get; set; }
            public int FolderKey { get; set; }
            public string UserName { get; set; }
            public ObservableCollection<string> Memos { get; set; }
            public SecureString Password { get; set; } = new SecureString();
        }

        public class RecursionFolder : FolderItem
        {
            public List<RecursionFolder> Children { get; set; } = new List<RecursionFolder>();
        }

        [Serializable]
        public class SerializeFolder
        {
            public string Title { get; set; }
            public int Key { get; set; }
            public int? ParentKey { get; set; }
            public SerializePassword[] Items { get; set; }
        }
        [Serializable]
        public class SerializePassword
        {
            public SerializePassword(PasswordItem item)
            {
                if (item != null)
                {
                    Title = item.Title;
                    Key = item.Key;
                    FolderKey = item.FolderKey;
                    UserName = item.UserName;
                    Memos = item.Memos.ToArray();
                    PasswordString = Functions.SecureStringProcessing(item.Password);
                }
            }

            public string Title { get; set; }
            public int Key { get; set; }
            public int FolderKey { get; set; }
            public string UserName { get; set; }
            public string[] Memos { get; set; }
            public string PasswordString { get; set; }
        }

        private ObservableCollection<RecursionFolder> recursionFolders =
            new ObservableCollection<RecursionFolder>();
        public ObservableCollection<RecursionFolder> RecursionFolders
        {
            get { return recursionFolders; }
            set { SetProperty(ref recursionFolders, value); }
        }

        public static IEnumerable<SerializeFolder> GetSerializeFolders(List<FolderItem> FolderItems)
        {
            foreach (var i in FolderItems)
            {
                var target = Functions.Copy(i, new SerializeFolder());
                target.Items = i.Items.Select(j => new SerializePassword(j)).ToArray();
                yield return target;
            }
        }

        public static IEnumerable<FolderItem> GetDeSerializeFolders(List<SerializeFolder> SerializeFolders)
        {
            foreach (var i in SerializeFolders)
            {
                var target = Functions.Copy(i, new FolderItem());
                var converts = i.Items.Select(j => new PasswordItem(j));
                target.Items = new List<PasswordItem>(converts);
                yield return target;
            }
        }

        public static List<RecursionFolder> ListToRecursion(List<FolderItem> list)
        {
            var result = new List<RecursionFolder>();
            var roots = list.Where(i => !i.ParentKey.HasValue);
            var notRoots = list.Where(i => i.ParentKey.HasValue);

            Func<FolderItem, List<RecursionFolder>> listFunction = null;
            listFunction = x =>
            {
                var children = (from i in notRoots
                                where i.ParentKey.Value == x.Key
                                select Functions.Copy(i, new RecursionFolder())).ToList();
                foreach (var i in children)
                {
                    i.Children = listFunction(i);
                }
                return children;
            };

            foreach (var i in roots)
            {
                var add = Functions.Copy(i, new RecursionFolder());
                add.Children = listFunction(i);
                result.Add(add);
            }

            return result;
        }

        public static List<FolderItem> RecursionToList(List<RecursionFolder> recursion)
        {
            var result = new List<FolderItem>();

            Action<RecursionFolder> recursionAction = null;
            recursionAction = x =>
            {
                var cast = Functions.Copy(x, new FolderItem());
                result.Add(cast);

                foreach (var i in x.Children)
                {
                    recursionAction(i);
                }
            };

            recursion.ForEach(i => recursionAction(i));

            return result;
        }

        public static List<RecursionFolder> InsertItem(RecursionFolder target,
                                                       FolderItem insert,
                                                       List<RecursionFolder> itemList)
        {
            var list = RecursionToList(itemList);

            insert.Key = list.Max(i => i.Key) + 1;
            insert.ParentKey = target.Key;

            list.Add(insert);

            var result = ListToRecursion(list);
            return result;
        }

        public static List<RecursionFolder> DeleteItem(RecursionFolder delete,
                                                       List<RecursionFolder> itemList)
        {
            return DeleteItem(delete.Key, itemList);
        }
        public static List<RecursionFolder> DeleteItem(int deleteKey,
                                                       List<RecursionFolder> itemList)
        {
            if (deleteKey < 0) return null;
            var recurtionItems = RecursionToList(itemList);
            var target = recurtionItems.FirstOrDefault(i => i.Key == deleteKey);
            if (target == null) return null;

            var deleteKeys = new List<int>();
            Action<FolderItem> deleteAction = null;
            deleteAction = x =>
            {
                deleteKeys.Add(x.Key);
                var children = recurtionItems.Where(i =>
                {
                    if(i.ParentKey.HasValue &&
                       i.ParentKey == x.Key)
                    {
                        return true;
                    }
                    return false;
                });
                foreach(var i in children)
                {
                    deleteAction(i);
                }
            };
            deleteAction(target);

            foreach(var i in deleteKeys)
            {
                var delete = recurtionItems.First(j => j.Key == i);
                recurtionItems.Remove(delete);
            }

            var result = ListToRecursion(recurtionItems);
            return result;
        }
    }
}
