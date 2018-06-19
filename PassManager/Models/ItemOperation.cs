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
            public string Supplement { get; set; }
            public string PasswordString { get; set; }
        }

        public int key = 0; 
        public ObservableCollection<PasswordItem> PasswordItems { get; set; }

        public static List<RecursionItem> ListToRecursion(List<PasswordItem> list)
        {
            var result = new List<RecursionItem>();

            Func<PasswordItem, List<RecursionItem>> listFunction = null;
            listFunction = x =>
            {
                var children = (from i in list
                                where i.ParentKey.Value == x.Key
                                select Functions.Copy(i, new RecursionItem())).ToList();
                foreach(var i in children)
                {
                    i.Children = listFunction(i);
                }
                return children;
            };

            var roots = list.Where(i => !i.ParentKey.HasValue);
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
                foreach(var i in x.Children)
                {
                    var cast = Functions.Copy(i, new PasswordItem());
                    result.Add(cast);
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

        public static List<RecursionItem> DeleteItem(int deleteKey,
                                                     List<RecursionItem> itemList)
        {
            var list = RecursionToList(itemList);
            var target = list.FirstOrDefault(i => i.Key == deleteKey);

            list.Remove(target);

            var result = ListToRecursion(list);
            return result;
        }
    }
}
