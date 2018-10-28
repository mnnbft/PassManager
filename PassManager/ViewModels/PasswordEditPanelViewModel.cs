using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PassManager.Views;
using Prism.Mvvm;
using Prism.Commands;
using PassManager.Models;
using System.ComponentModel;
using MaterialDesignThemes.Wpf;

namespace PassManager.ViewModels
{
    class PasswordEditPanelViewModel : BindableBase
    {
        public PasswordEditPanelViewModel()
        {
            ItemOperation.Instance.PropertyChanged += (d, e) => { OnPropertyChanged(e); };
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
        private void FunctionLoaded()
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(SelectedPassword )));
        }

        public DelegateCommand CommandPasswordGenerate
        {
            get { return new DelegateCommand(FunctionPasswordGenerate); }
        }
        private void FunctionPasswordGenerate()
        {
            var dialog = new PasswordGenerateDialog();
            dialog.Unloaded += (d, e) =>
            {
                SelectedPassword.Password = dialog.GeneratedPassword;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(SelectedPassword)));
            };

            DialogHost.Show(dialog);
        }
    }
}
