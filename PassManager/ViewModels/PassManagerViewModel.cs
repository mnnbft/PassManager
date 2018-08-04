using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Prism.Commands;
using PassManager.Models;

namespace PassManager.ViewModels
{
    public class PassManagerViewModel : BindableBase
    {
        public ObservableCollection<string> tabName = new ObservableCollection<string>();
        public ObservableCollection<string> TabName
        {
            get { return tabName; }
            set { tabName = value; }
        }
    }
}
