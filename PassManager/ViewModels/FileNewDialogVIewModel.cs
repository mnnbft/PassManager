using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Security;
using Prism.Mvvm;
using Prism.Commands;
using MaterialDesignThemes.Wpf;

namespace PassManager.ViewModels
{
    class FileNewDialogViewModel : BindableBase
    {
        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { SetProperty(ref filePath, value); }
        }
        private string keyPath;
        public string KeyPath
        {
            get { return keyPath; }
            set { SetProperty(ref keyPath, value); }
        }
        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set { SetProperty(ref fileName, value); }
        }
        private string dummyPassword;
        public string DummyPassword
        {
            get { return dummyPassword; }
            set
            {
                SecurePassword.Clear();
                foreach(var c in value)
                {
                    SecurePassword.AppendChar(c);
                }
                SetProperty(ref dummyPassword, new string('●', value.Length));
            }
        }
        public SecureString SecurePassword { get; set; } = new SecureString();

        public DelegateCommand CommandCreate
        {
            get { return new DelegateCommand(FunctionCreate); }
        }
        public DelegateCommand CommandCancel
        {
            get { return new DelegateCommand(FunctionCancel); }
        }

        private void FunctionCreate()
        {
            Models.Password.Instance.CreateNewFile(FileName, FilePath, KeyPath, SecurePassword);
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
        private void FunctionCancel()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private bool CanExecuteFunctionCreate()
        {
            return true;
        }
    }
}
