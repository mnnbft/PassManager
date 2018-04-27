using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewModel
{
    using Prism.Commands;
    using Prism.Mvvm;  
    using System.Windows;
    using System.Windows.Controls;
    using Common;

    public class PasswordInfoVM : ViewModelBase
    {
        private MainWindowVM _WindowDC = null;
        public MainWindowVM WindowDC
        {
            get { return _WindowDC; }
            set { SetProperty(ref _WindowDC, value); }
        }

        public DelegateCommand<object> CommandLoaded
        {
            get
            {
                return new DelegateCommand<object>(
                    delegate(object obj)
                    {
                        if(_WindowDC == null && obj != null)
                        {
                            var w = Window.GetWindow((UserControl)obj);
                            WindowDC = (MainWindowVM)w.DataContext;
                        }
                    });
            }
        }
    }
}
