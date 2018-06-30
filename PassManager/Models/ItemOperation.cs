using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Security;

namespace PassManager.Models
{
    public sealed class ItemOperation
    {
        public ItemOperation Instance { get; } = new ItemOperation();
        private ItemOperation() { }

        public class PasswordItem
        {
            public string Title { get; set; }
            public int Key { get; set; }
            public int? ParentKey { get; set; }
            public string UserName { get; set; }
            public ObservableCollection<string> Memos { get; set; }
            public SecureString Password { get; set; } = new SecureString();
        }

        public class RecursionItem : PasswordItem
        {
            public List<RecursionItem> Children { get; set; } = new List<RecursionItem>();
        }

        [Serializable]
        public class SerializeItem
        {
            public string Title { get; set; }
            public int Key { get; set; }
            public int? ParentKey { get; set; }
            public string UserName { get; set; }
            public string[] Memos { get; set; }
            public string PasswordString { get; set; }
        }

        public int Key { get; set; } = 0;
        public ObservableCollection<PasswordItem> PasswordItems = 
            new ObservableCollection<PasswordItem>();

        public static IEnumerable<SerializeItem> GetSerializeItems(List<PasswordItem> passwordItems)
        {
            foreach(var i in passwordItems)
            {
                var target = Functions.Copy(i, new SerializeItem());
                target.Memos = i.Memos.ToArray();
                target.PasswordString = Functions.SecureStringProcessing(i.Password);

                yield return target;
            }
        }

        public static IEnumerable<PasswordItem> GetDeserializeItems(List<SerializeItem> serializeItems)
        {
            foreach(var i in serializeItems)
            {
                var target = Functions.Copy(i, new PasswordItem());
                target.Memos = new ObservableCollection<string>(i.Memos);
                target.Password = new SecureString();
                foreach(var j in i.PasswordString.ToCharArray())
                {
                    target.Password.AppendChar(j);
                }

                yield return target;
            }
        }

        public static List<RecursionItem> ListToRecursion(List<PasswordItem> list)
        {
            var result = new List<RecursionItem>();
            var roots = list.Where(i => !i.ParentKey.HasValue);
            var notRoots = list.Where(i => i.ParentKey.HasValue);

            Func<PasswordItem, List<RecursionItem>> listFunction = null;
            listFunction = x =>
            {
                var children = (from i in notRoots
                                where i.ParentKey.Value == x.Key
                                select Functions.Copy(i, new RecursionItem())).ToList();
                foreach(var i in children)
                {
                    i.Children = listFunction(i);
                }
                return children;
            };

            foreach(var i in roots)
            {
                var add = new RecursionItem();
                add.Children = listFunction(i);
                result.Add(add);
            }

            return result;
        }

        public static List<PasswordItem> RecursionToList(List<RecursionItem> recursion)
        {
            var result = new List<PasswordItem>();

            Action<RecursionItem> recursionAction = null;
            recursionAction = x =>
            {
                var cast = Functions.Copy(x, new PasswordItem());
                result.Add(cast);

                foreach(var i in x.Children)
                {
                    recursionAction(i);
                }
            };

            recursion.ForEach(i => recursionAction(i));

            return result;
        }

        public static List<RecursionItem> InsertItem(RecursionItem target,
                                                     PasswordItem insert,
                                                     List<RecursionItem> itemList)
        {
            var list = RecursionToList(itemList);
            insert.ParentKey = target.Key;

            list.Add(insert);

            var result = ListToRecursion(list);
            return result;
        }

        public static List<RecursionItem> DeleteItem(RecursionItem delete,
                                                     List<RecursionItem> itemList)
        {
            return DeleteItem(delete.Key, itemList);
        }
        public static List<RecursionItem> DeleteItem(int deleteKey,
                                                     List<RecursionItem> itemList)
        {
            if (deleteKey <= 0) return null;
            var recurtionItems = RecursionToList(itemList);
            var target = recurtionItems.FirstOrDefault(i => i.Key == deleteKey);
            if (target == null) return null;

            var deleteKeys = new List<int>();
            Action<PasswordItem> deleteAction = null;
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
