using MaterialDesignThemes.Wpf;
using PassManager.Models;
using PassManager.Views;
using Prism.Commands;
using Prism.Mvvm;
using System.ComponentModel;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Windows;

namespace PassManager.ViewModels
{
    class PasswordEditPanelViewModel : ViewModelBase
    {
        public PasswordEditPanelViewModel()
        {
            SelectedPassword = ItemOperation.Instance.ObserveProperty(i => i.SelectedPassword).ToReactiveProperty().AddTo(Disposables);
        }

        public ReactiveProperty<PasswordItem> SelectedPassword { get; }

        private bool isPasswordView;
        public bool IsPasswordView
        {
            get { return isPasswordView; }
            set { SetProperty(ref isPasswordView, value); }
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

            dialog.Unloaded += DialogUnLoaded;
            DialogHost.Show(dialog);
        }

        public DelegateCommand CommandPasswordView
        {
            get { return new DelegateCommand(FunctionPasswordView); }
        }
        private void FunctionPasswordView()
        {
            IsPasswordView = !IsPasswordView;
        }

        public DelegateCommand CommandMemoPlus
        {
            get { return new DelegateCommand(FunctionMemoPlus); }
        }
        private void FunctionMemoPlus()
        {
            ItemOperation.Instance.AddMemos();
        }

        public DelegateCommand CommandMemoMinus
        {
            get { return new DelegateCommand(FunctionMemoMinus); }
        }
        private void FunctionMemoMinus()
        {
            ItemOperation.Instance.AddMemos();
        }

        private void DialogUnLoaded(object sender, RoutedEventArgs e)
        {
            var dialog = (PasswordGenerateDialog)sender;
            ItemOperation.Instance.SetPassword(dialog.GeneratedPassword);
        }
    }
}
