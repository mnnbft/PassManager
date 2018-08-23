using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Prism.Mvvm;
using Prism.Commands;

namespace PassManager.ViewModels
{
    public static class CommonViewModel
    {
        public static DelegateCommand<object[]> CommandOpenFileDialog
        {
            get { return new DelegateCommand<object[]>(FunctionOpenFileDialog); }
        }

        private static void FunctionOpenFileDialog(object[] obj)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var instance = (BindableBase)obj[0];
            var propertyName = obj[1].ToString();
            var property = instance.GetType().GetProperty(propertyName);

            property.SetValue(instance, dialog.SelectedPath);
        }
    }
}
