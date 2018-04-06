using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace PassManager.Model
{
    using System.Security;

    public class MainWindowM
    {
        public static List<TreeViewParam> ReCreateItems(TreeViewParam SelectedItem, List<TreeViewParam> Items, TreeViewParam AddItem)
        {
            var rtItems = new List<TreeViewParam>();

            Func<TreeViewParam, List<TreeViewParam>, List<TreeViewParam>> f = null;
            f = (x, y) =>
            {
                foreach (var item in y)
                    item.Child = f(item, item.Child);
                if (x.Equals(SelectedItem))
                    y.Add(AddItem);
                return y;
            };
            foreach (var item in Items)
            {
                item.Child = f(item, item.Child);
                rtItems.Add(item);
            }

            return rtItems;
        }
    }

    [Serializable]
    public class TreeViewParam
    {
        public string Title { get; set; }
        public int Key { get; set; }
        public int? ParentKey { get; set; }
        public List<TreeViewParam> Child { get; set; } = new List<TreeViewParam>();
    }

    [Serializable]
    public class BaseParam : TreeViewParam
    {
        public string UserName { get; set; }
        public string Supplement { get; set; }
    }

    [Serializable]
    public class SaveParam : BaseParam
    {
        public string PasswordString { get; set; }
    }

    public class DataParam : BaseParam
    {
        public SecureString Password { get; set; }
    }
}
