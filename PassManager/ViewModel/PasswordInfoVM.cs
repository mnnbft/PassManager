using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewModel
{
    using System.Windows;
    using System.Windows.Controls;
    using Common;

    public class PasswordInfoVM : ViewModelBase
    {
        private MainWindowVM _WindowDC = null;
        public MainWindowVM WindowDC
        {
            get { return _WindowDC; }
            set
            {
                _WindowDC = value;
                OnPropertyChanged("WindowDC");
            }
        }
        public DelegateCommand CommandLoaded
        {
            get
            {
                return new DelegateCommand(
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
