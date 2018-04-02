using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.Model
{
    public class OpenFileM
    {
        public static List<DataParam> OpenCreateItems (DataParam[] FileItems)
        {
            var rtItems = new List<DataParam>();

            Func<DataParam, List<DataParam>> f = null;
            f = x =>
            {
                var rtItem = new List<DataParam>();
                var FileItems2 = FileItems.Where(para => para.ParentKey == x.Key).ToList();
                foreach (var t in FileItems2)
                {
                    t.Child.AddRange(f(t));
                    rtItem.Add(t);
                }
                return rtItem;
            };

            foreach (var t in FileItems.Where(para => !para.ParentKey.HasValue))
            {
                t.Child.AddRange(f(t));
                rtItems.Add(t);
            }

            return rtItems;
        }
    }
}
