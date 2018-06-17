using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace PassManager.Model
{
    using Prism.Mvvm;

    public class OpenFileM : BindableBase
    {
        public string _OpenFilePath;
        public string OpenFilePath
        {
            get { return _OpenFilePath; }
            set { SetProperty(ref _OpenFilePath, value); }
        }
        public string OpenKeyPath { get; set; }

        public static List<DataParam> OpenCreateItems (DataParam[] FileItems)
        {
            var rtItems = new List<DataParam>();

            Func<DataParam, List<DataParam>> f = null;
            f = x =>
            {
                var rtItem = new List<DataParam>();
                var FileItems2 = FileItems.Where(i => i.ParentKey == x.Key).ToList();
                foreach (var t in FileItems2)
                {
                    t.Child.AddRange(f(t));
                    rtItem.Add(t);
                }
                return rtItem;
            };

            foreach (var t in FileItems.Where(i => !i.ParentKey.HasValue))
            {
                t.Child.AddRange(f(t));
                rtItems.Add(t);
            }

            return rtItems;
        }
    }
}
