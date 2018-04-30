using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.Common
{
    using Prism.Mvvm;
    using System.ComponentModel;

    public class ViewModelBase : BindableBase
    {
        public new void OnPropertyChanged(string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}
