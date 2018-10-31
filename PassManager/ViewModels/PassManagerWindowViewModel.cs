using MaterialDesignThemes.Wpf;
using PassManager.Models;
using PassManager.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Collections.Specialized;

namespace PassManager.ViewModels
{
    class PassManagerWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public PassManagerWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            FileIO.Instance.OpenedFile.PropertyChanged += (d, e) => { OnPropertyChanged(e); };
            MenuContent.Instance.PropertyChanged += (d, e) => { OnPropertyChanged(e); };
            ItemOperation.Instance.PropertyChanged += (d, e) => { OnPropertyChanged(e); };
            ItemOperation.Instance.SelectedPasswords.CollectionChanged += SelectedPasswordsCollectionChanged;
        }

        public ObservableCollection<FolderItem> Folders 
        {
            get { return FileIO.Instance.OpenedFile.Folders; }
            set { FileIO.Instance.OpenedFile.Folders = value; }
        }
        public FolderItem SelectedFolder
        {
            get { return ItemOperation.Instance.SelectedFolder; }
            set
            {
                ItemOperation.Instance.SelectedFolder = value;
                IsInEditMode = false;
                _regionManager.RequestNavigate("PasswordRegion", "PasswordListPanel");
            }
        }
        public string SnackMessage
        {
            get { return MenuContent.Instance.SnackMessage; }
        }
        public bool IsSnackbarActive
        {
            get { return MenuContent.Instance.IsSnackbarActive; }
        }
        public ObservableCollection<PasswordItem> SelectedPasswords
        {
            get { return ItemOperation.Instance.SelectedPasswords; }
        }
        public PasswordItem SelectedPassword
        {
            get { return ItemOperation.Instance.SelectedPassword; }
            set { ItemOperation.Instance.SelectedPassword = value; }
        }
        private bool isInEditMode;
        public bool IsInEditMode
        {
            get { return isInEditMode; }
            set { SetProperty(ref isInEditMode, value); }
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
            if(!FileIO.Instance.OpenedFile.IsOpened)
            {
                MenuContent.Instance.ShowSnackMessage("ファイルを開いてください");
                return;
            }

            var filePath = FileIO.Instance.OpenedFile.FilePath;
            var keyPath = FileIO.Instance.OpenedFile.KeyPath;
            var password = FileIO.Instance.OpenedFile.Password;
            var folders = FileIO.Instance.OpenedFile.Folders;

            FileIO.Instance.FileEncrypt(filePath, keyPath, password, folders);

            MenuContent.Instance.ShowSnackMessage("ファイルを保存しました");
        }

        public DelegateCommand CommandFileClose
        {
            get { return new DelegateCommand(FunctionFileClose); }
        }
        private void FunctionFileClose()
        {
            if(!FileIO.Instance.OpenedFile.IsOpened)
            {
                MenuContent.Instance.ShowSnackMessage("ファイルを開いてください");
                return;
            }

            FileIO.Instance.OpenedFile.Close();

            MenuContent.Instance.ShowSnackMessage("ファイルを閉じました");
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

        public DelegateCommand CommandFolderReName
        {
            get { return new DelegateCommand(FunctionFolderReName); }
        }
        private void FunctionFolderReName()
        {
            IsInEditMode = true;
        }

        public DelegateCommand CommandEditKeyDown
        {
            get { return new DelegateCommand(FunctionEditKeyDown); }
        }
        private void FunctionEditKeyDown()
        {
            IsInEditMode = false;
        }

        public DelegateCommand CommandBack
        {
            get { return new DelegateCommand(FunctionBack); }
        }
        private void FunctionBack()
        {
            _regionManager.RequestNavigate("PasswordRegion", "PasswordListPanel");
        }

        private void SelectedPasswordsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(ItemOperation.Instance.SelectedPasswords.Count == 0)
            { _regionManager.RequestNavigate("PasswordRegion", "PasswordListPanel"); }
        }
    }
}
