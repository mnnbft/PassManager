﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using PassManager.Models;
using Prism.Mvvm;
using Prism.Commands;
using Prism.Regions;

namespace PassManager.ViewModels
{
    class PasswordListPanelViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public PasswordListPanelViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            ItemOperation.Instance.PropertyChanged += (d, e) => { OnPropertyChanged(e); };
        }

        public FolderItem SelectedFolder
        {
            get { return ItemOperation.Instance.SelectedFolder; }
        }
        public PasswordItem SelectedPassword
        {
            get { return ItemOperation.Instance.SelectedPassword; }
            set { ItemOperation.Instance.SelectedPassword = value; }
        }

        public DelegateCommand CommandLoaded
        {
            get { return new DelegateCommand(FunctionLoaded); }
        }
        public void FunctionLoaded()
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(SelectedFolder)));
        }

        public DelegateCommand CommandModify
        {
            get { return new DelegateCommand(FunctionModify); }
        }
        private void FunctionModify()
        {
            if(SelectedPassword == null)
            { return; }

            ItemOperation.Instance.SelectedPasswords.Add(SelectedPassword);
            _regionManager.RequestNavigate("PasswordRegion", "PasswordEditTabControl");
        }

        public DelegateCommand CommandDelete
        {
            get { return new DelegateCommand(FunctionDelete); }
        }
        private void FunctionDelete()
        {
            if(SelectedPassword == null)
            { return; }

            ItemOperation.Instance.SelectedFolder.Passwords.Remove(SelectedPassword);
        }

        public DelegateCommand CommandClipBoard
        {
            get { return new DelegateCommand(FunctionClipBoard); }
        }
        private void FunctionClipBoard()
        {
            ClipBoard.Instance.PasswordCopy();
        }
    }
}
