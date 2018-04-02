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
            foreach(var item in Items)
            {
                item.Child = f(item, item.Child);
                rtItems.Add(item);
            }

            return rtItems;
        }
    }

    public class TreeViewParam
    {
        public string Title { get; set; }
        public int Key;
        public int? ParentKey;
        public List<TreeViewParam> Child { get; set; } = new List<TreeViewParam>();
    }
    public class DataParam : TreeViewParam
    {
        public DataParam()
        {
        }
        public DataParam(string param)
        {
            int i = 0;
            char[] split = ",".ToCharArray();

            foreach (var p in param.Split(split))
            {
                switch (i)
                {
                    case 0:
                        if (!int.TryParse(p, out Key))
                            Key = -1;
                        break;
                    case 1:
                        Title = p;
                        break;
                    case 2:
                        UserName = p;
                        break;
                    case 3:
                        Password = new SecureString();
                        foreach (var s in p.ToCharArray())
                        {
                            Password.AppendChar(s);
                        }
                        break;
                    case 4:
                        Supplement = p;
                        break;
                    case 5:
                        int tmp;
                        ParentKey = int.TryParse(p, out tmp) ? tmp : (int?)null;

                        break;
                }
                i++;
            }
        }

        public string UserName { get; set; }
        public SecureString Password;
        public string Supplement { get; set; }
    }
}
