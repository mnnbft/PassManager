﻿using MaterialDesignThemes.Wpf;
using PassManager.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.IO;

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
        public string PasswordString { get; set; }

        public DelegateCommand CommandCreate
        {
            get { return new DelegateCommand(FunctionCreate); }
        }
        private void FunctionCreate()
        {
            Password.Instance.CreateNewFile(FileName, FilePath, KeyPath, PasswordString);
            DialogHost.CloseDialogCommand.Execute(null, null);

            var fullFilePath = Path.Combine(filePath, fileName + ".pass");
            MenuContent.Instance.ShowSnackMessage(fullFilePath + "を開きました", 3000);
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
