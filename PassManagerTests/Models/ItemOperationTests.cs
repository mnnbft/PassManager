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
        List<ItemOperation.SerializeFolder> testItems = new List<ItemOperation.SerializeFolder>()
        {
            new ItemOperation.SerializeFolder()
            {
                Key = 0,
                Title = "test1",
                ParentKey = null,
                Items = new ItemOperation.SerializePassword[3], 
            },
            new ItemOperation.SerializeFolder()
            {
                Key = 1,
                Title = "test2",
                ParentKey = 0,
                Items = new ItemOperation.SerializePassword[3], 
            },
            new ItemOperation.SerializeFolder()
            {
                Key = 2,
                Title = "test3",
                ParentKey = 1,
                Items = new ItemOperation.SerializePassword[3], 
            },
        };

        [TestMethod()]
        public void GetSerializeFoldersTest()
        {
            var test = ItemOperation.GetDeSerializeFolders(testItems);
            var test2 = ItemOperation.GetSerializeFolders(test.ToList());
        }

        [TestMethod()]
        public void ListToRecursionTest()
        {
            var test1 = ItemOperation.GetDeSerializeFolders(testItems);
            var test2 = ItemOperation.ListToRecursion(test1.ToList());
        }

        [TestMethod()]
        public void RecursionToListTest()
        {
            var test1 = ItemOperation.GetDeSerializeFolders(testItems);
            var test2 = ItemOperation.ListToRecursion(test1.ToList());
            var test3 = ItemOperation.RecursionToList(test2.ToList());
        }

        [TestMethod()]
        public void InsertItemTest()
        {
            var test1 = ItemOperation.GetDeSerializeFolders(testItems);
            var test2 = ItemOperation.ListToRecursion(test1.ToList());

            var insert = new ItemOperation.RecursionFolder()
            {
                Key = 3,
                Title = "insert",
                ParentKey = 1,
                Items = new List<ItemOperation.PasswordItem>(3),
                Children = null,
            };

            var target = test2.FirstOrDefault(i => i.Key == 0);
            var result = ItemOperation.InsertItem(target, insert, test2);
        }

        [TestMethod()]
        public void DeleteItemTest()
        {
            var test1 = ItemOperation.GetDeSerializeFolders(testItems);
            var test2 = ItemOperation.ListToRecursion(test1.ToList());

            var target = test2.FirstOrDefault(i => i.Key == 0);

            ItemOperation.DeleteItem(1, test2);
        }
    }
}