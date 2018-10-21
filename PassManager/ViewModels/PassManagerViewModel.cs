using MaterialDesignThemes.Wpf;
using PassManager.Models;
using PassManager.Views;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;

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
        public RecursionFolder SelectedFolder { get; set; }

        public DelegateCommand CommandFileFind
        {
            get { return new DelegateCommand(FunctionFileFind); }
        }
        private async void FunctionFileFind()
        {
            var dialog = new FileFindDialog();
            await DialogHost.Show(dialog);
        }

        public DelegateCommand CommandFileNew
        {
            get { return new DelegateCommand(FunctionFileNew); }
        }
        private async void FunctionFileNew()
        {
            var dialog = new FileNewDialog();
            await DialogHost.Show(dialog);
        }

        public DelegateCommand CommandNewItem
        {
            get { return new DelegateCommand(FunctionNewItem); }
        }
        private void FunctionNewItem()
        {
        }

        public DelegateCommand CommandNewFolder
        {
            get { return new DelegateCommand(FunctionNewFolder); }
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

        public DelegateCommand CommandDeleteItem
        {
            get { return new DelegateCommand(FunctionDeleteFolder); }
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
