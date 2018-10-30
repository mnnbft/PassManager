using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using PassManager.Models;
using Prism.Mvvm;
using Prism.Commands;

namespace PassManager.ViewModels
{
    class PasswordListPanelViewModel : BindableBase
    {
        public PasswordListPanelViewModel()
        {
            ItemOperation.Instance.PropertyChanged += (d, e) => { OnPropertyChanged(e); };
        }

        public FolderItem SelectedFolder
        {
            get { return ItemOperation.Instance.SelectedFolder; }
        }

        public DelegateCommand CommandLoaded
        {
            get { return new DelegateCommand(FunctionLoaded); }
        }
        public void FunctionLoaded()
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(SelectedFolder)));
        }
    }
}
