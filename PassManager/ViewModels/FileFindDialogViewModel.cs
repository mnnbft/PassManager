﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using MaterialDesignThemes.Wpf;

namespace PassManager.ViewModels
{
    class FileFindDialogViewModel : BindableBase
    {
        public DelegateCommand CommandFileOpen
        {
            get { return new DelegateCommand(FunctionFileOpen); }
        }
        public DelegateCommand CommandCancel
        {
            get { return new DelegateCommand(FunctionCancel); }
        }

        private void FunctionFileOpen()
        {
        }
        private void FunctionCancel()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
