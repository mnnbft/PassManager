using MaterialDesignThemes.Wpf;
using PassManager.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Security;

namespace PassManager.ViewModels
{
    class FileFindDialogViewModel : BindableBase
    {
        private string fullKeyPath;
        public string FullKeyPath
        {
            get { return fullKeyPath; }
            set { SetProperty(ref fullKeyPath, value); }
        }
        private string fullFilePath;
        public string FullFilePath
        {
            get { return fullFilePath; }
            set { SetProperty(ref fullFilePath, value); }
        }
        public SecureString SecurePassword { get; set; } = new SecureString();

        public DelegateCommand CommandFileOpen
        {
            get { return new DelegateCommand(FunctionFileOpen); }
        }
        private void FunctionFileOpen()
        {
            Password.Instance.OpenFile(FullFilePath, FullKeyPath, SecurePassword);
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public DelegateCommand CommandCancel
        {
            get { return new DelegateCommand(FunctionCancel); }
        }
        private void FunctionCancel()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
