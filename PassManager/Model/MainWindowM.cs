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
        public static List<DataParam> ReCreateItems(DataParam SelectedItem, List<DataParam> Items, DataParam AddItem)
        {
            var rtItems = new List<DataParam>();

            Func<DataParam, List<DataParam>, List<DataParam>> f = null;
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

    public class  DataParam
    {
        public string Title { get; set; }
        public int Key { get; set; }
        public int? ParentKey { get; set; }
        public string UserName { get; set; }
        public string Supplement { get; set; }
        public SecureString Password { get; set; } = new SecureString();
        public List<DataParam> Child { get; set; } = new List<DataParam>();
    }

    [Serializable]
    public class SaveParam
    {
        public string Title { get; set; }
        public int Key { get; set; }
        public int? ParentKey { get; set; }
        public string UserName { get; set; }
        public string Supplement { get; set; }
        public string PasswordString { get; set; }
    }
}
