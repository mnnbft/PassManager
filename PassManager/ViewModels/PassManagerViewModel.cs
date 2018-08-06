using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Prism.Commands;
using PassManager.Models;
using MaterialDesignThemes.Wpf;
using PassManager.Views;

namespace PassManager.ViewModels
{
    class PassManagerViewModel : BindableBase
    {
        public PassManagerViewModel()
        {
            ItemOperation.Instance.PropertyChanged += (d, e) => { OnPropertyChanged(e); };
        }

        public ObservableCollection<ItemOperation.RecursionFolder> RecursionFolders
        {
            get { return ItemOperation.Instance.RecursionFolders; }
        }

        public DelegateCommand CommandFileFind
        {
            get { return new DelegateCommand(FunctionFileFind); }
        }
        public DelegateCommand CommandFileNew
        {
            get { return new DelegateCommand(FunctionFileNew); }
        }

        private async void FunctionFileFind()
        {
            var dialog = new FileFindDialog();
            await DialogHost.Show(dialog);
        }
        private async void FunctionFileNew()
        {
            var dialog = new FileNewDialog();
            await DialogHost.Show(dialog);
        }
    }
}
