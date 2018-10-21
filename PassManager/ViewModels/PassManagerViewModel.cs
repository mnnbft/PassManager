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

        public ObservableCollection<RecursionFolder> RecursionFolders
        {
            get { return ItemOperation.Instance.RecursionFolders; }
            set { ItemOperation.Instance.RecursionFolders = value; }
        }
        public DelegateCommand CommandFileFind
        {
            get { return new DelegateCommand(FunctionFileFind); }
        }
        public DelegateCommand CommandFileNew
        {
            get { return new DelegateCommand(FunctionFileNew); }
        }
        public DelegateCommand CommandNewItem
        {
            get { return new DelegateCommand(FunctionNewItem); }
        }
        public DelegateCommand CommandNewFolder
        {
            get { return new DelegateCommand(FunctionNewFolder); }
        }
        public DelegateCommand CommandDeleteItem
        {
            get { return new DelegateCommand(FunctionDeleteFolder); }
        }
        public RecursionFolder SelectedFolder { get; set; }

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
        private void FunctionNewItem()
        {
        }
        private void FunctionNewFolder()
        {
            var newFolder = new FolderItem();
            newFolder.Title = "新しいフォルダー";
            var insertedList = ItemOperation.InsertItem(SelectedFolder,
                                                        newFolder,
                                                        RecursionFolders.ToList());
            RecursionFolders.Clear();
            foreach (var i in insertedList)
            {
                RecursionFolders.Add(i);
            }
        }
        private void FunctionDeleteFolder()
        {
            var deletedList = ItemOperation.DeleteItem(SelectedFolder, RecursionFolders.ToList());

            RecursionFolders.Clear();
            foreach (var i in deletedList)
            {
                RecursionFolders.Add(i);
            }
        }
    }
}
