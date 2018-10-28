using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using PassManager.Models;
using System.Security;
using MaterialDesignThemes.Wpf;
using PassManager.Views;

namespace PassManager.ViewModels
{
    public class PassTypeCheckItem
    {
        public PassType PassType { get; set; }
        public bool IsChecked { get; set; }
    }

    class PasswordGenerateDialogViewModel : BindableBase
    {
        public PasswordGenerateDialogViewModel()
        {
            SelectedPassType = new List<PassTypeCheckItem>();

            var passTypes = Enum.GetValues(typeof(PassType));
            foreach(PassType passType in passTypes)
            {
                SelectedPassType.Add(new PassTypeCheckItem() { PassType = passType });
            }
        }

        public List<PassTypeCheckItem> SelectedPassType { get; set; }
        public int PassLength { get; set; }

        public DelegateCommand<PasswordGenerateDialog> CommandGenerate
        {
            get { return new DelegateCommand<PasswordGenerateDialog>(FunctionGenerate); }
        }
        private void FunctionGenerate(PasswordGenerateDialog dialog)
        {
            var passTypes = SelectedPassType
                .Where(i => i.IsChecked)
                .Select(i => i.PassType).ToArray();

            dialog.GeneratedPassword = Password.Instance.GeneratePassword(PassLength, passTypes);

            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
