using Microsoft.VisualStudio.TestTools.UnitTesting;
using PassManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.Models.Tests
{
    [TestClass()]
    public class ItemOperationTests
    {
        List<ItemOperation.SerializeItem> testItems = new List<ItemOperation.SerializeItem>()
        {
            new ItemOperation.SerializeItem()
            {
                Key = 0,
                Title = "test1",
                UserName = "user1",
                ParentKey = null,
                Memos = new string[] { "string", "test"},
                PasswordString = "12345",
            },
            new ItemOperation.SerializeItem()
            {
                Key = 1,
                Title = "test2",
                UserName = "user2",
                ParentKey = 0,
                Memos = new string[] { "string", "test"},
                PasswordString = "12345",
            },
            new ItemOperation.SerializeItem()
            {
                Key = 2,
                Title = "test3",
                UserName = "user3",
                ParentKey = 1,
                Memos = new string[] { "string", "test"},
                PasswordString = "12345",
            },
        };

        [TestMethod()]
        public void GetSerializeItemsTest()
        {
            var test = ItemOperation.GetDeserializeItems(testItems);
            var test2 = ItemOperation.GetSerializeItems(test.ToList());
        }

        [TestMethod()]
        public void ListToRecursionTest()
        {
            var test1 = ItemOperation.GetDeserializeItems(testItems);
            var test2 = ItemOperation.ListToRecursion(test1.ToList());
        }

        [TestMethod()]
        public void RecursionToListTest()
        {
            var test1 = ItemOperation.GetDeserializeItems(testItems);
            var test2 = ItemOperation.ListToRecursion(test1.ToList());
            var test3 = ItemOperation.RecursionToList(test2.ToList());
        }

        [TestMethod()]
        public void InsertItemTest()
        {
            var test1 = ItemOperation.GetDeserializeItems(testItems);
            var test2 = ItemOperation.ListToRecursion(test1.ToList());

            var insert = new ItemOperation.RecursionItem()
            {
                Key = 3,
                Title = "insert",
                UserName = "insert",
                ParentKey = 1,
                Memos = new System.Collections.ObjectModel.ObservableCollection<string>(),
                Children = null,
                Password = new System.Security.SecureString(),
            };

            var target = test2.FirstOrDefault(i => i.Key == 0);
            ItemOperation.InsertItem(target, insert, test2);
        }

        [TestMethod()]
        public void DeleteItemTest()
        {
            var test1 = ItemOperation.GetDeserializeItems(testItems);
            var test2 = ItemOperation.ListToRecursion(test1.ToList());

            var target = test2.FirstOrDefault(i => i.Key == 0);

            ItemOperation.DeleteItem(1, test2);
        }
    }
}