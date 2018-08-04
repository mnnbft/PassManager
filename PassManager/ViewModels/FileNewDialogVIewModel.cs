using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Prism.Mvvm;
using Prism.Commands;
using MaterialDesignThemes.Wpf;

namespace PassManager.ViewModels
{
    class FileNewDialogViewModel
    {
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
        }
        private void FunctionCancel()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
