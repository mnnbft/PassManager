using MaterialDesignThemes.Wpf;
using PassManager.Models;
using PassManager.Views;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;

namespace PassManager.ViewModels
{
    class PassManagerWindowViewModel : BindableBase
    {
        public PassManagerWindowViewModel()
        {
            FileIO.Instance.OpenedFile.PropertyChanged += (d, e) => { OnPropertyChanged(e); };
        }

        public ObservableCollection<FolderItem> Folders 
        {
            get { return FileIO.Instance.OpenedFile.Folders; }
            set { FileIO.Instance.OpenedFile.Folders = value; }
        }
        private FolderItem selectedFolder;
        public FolderItem SelectedFolder
        {
            get { return selectedFolder; }
            set { SetProperty(ref selectedFolder, value); }
        }
        public PasswordItem SelectedPassword
        {
            get { return ItemOperation.Instance.SelectedPassword; }
            set { ItemOperation.Instance.SelectedPassword = value; }
        }

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
            SelectedFolder.Passwords.Add(new PasswordItem());
        }

        public DelegateCommand CommandNewFolder
        {
            get { return new DelegateCommand(FunctionNewFolder); }
        }
        private void FunctionNewFolder()
        {
            SelectedFolder.Children.Add(new FolderItem());
        }

        public DelegateCommand CommandDeleteFolder
        {
            get { return new DelegateCommand(FunctionDeleteFolder); }
        }
        private void FunctionDeleteFolder()
        {
            if(!SelectedFolder.CanDelete)
            { return; }

            DeleteFolderItem(Folders);
        }
        private void DeleteFolderItem(ObservableCollection<FolderItem> children)
        {
            foreach(var folder in children)
            {
                if (SelectedFolder == folder)
                {
                    children.Remove(SelectedFolder);
                    break;
                }
                DeleteFolderItem(folder.Children);
            }
        }

        public DelegateCommand CommandSave
        {
            get { return new DelegateCommand(FunctionSave); }
        }
        private void FunctionSave()
        {
            var filePath = FileIO.Instance.OpenedFile.FilePath;
            var keyPath = FileIO.Instance.OpenedFile.KeyPath;
            var password = FileIO.Instance.OpenedFile.Password;
            var folders = FileIO.Instance.OpenedFile.Folders;

            FileIO.Instance.FileEncrypt(filePath, keyPath, password, folders);
        }

        public DelegateCommand CommandFileClose
        {
            get { return new DelegateCommand(FunctionFileClose); }
        }
        private void FunctionFileClose()
        {
            FileIO.Instance.OpenedFile.Close();
        }

        public DelegateCommand CommandSettings
        {
            get { return new DelegateCommand(FunctionSettings); }
        }
        private void FunctionSettings()
        {
            var dialog = new SettingsDialog();
            DialogHost.Show(dialog);
        }
    }
}
