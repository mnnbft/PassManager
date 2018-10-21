using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Forms;

namespace PassManager.ViewModels
{
    public static class CommonViewModel
    {
        public static DelegateCommand<object[]> CommandOpenFolderDialog
        {
            get { return new DelegateCommand<object[]>(FunctionOpenFolderDialog); }
        }
        public static DelegateCommand<object[]> CommandOpenFileDialog
        {
            get { return new DelegateCommand<object[]>(FunctionOpenFileDialog); }
        }

        private static void FunctionOpenFolderDialog(object[] obj)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var instance = (BindableBase)obj[0];
            var propertyName = obj[1].ToString();
            var property = instance.GetType().GetProperty(propertyName);

            property.SetValue(instance, dialog.SelectedPath);
        }

        private static void FunctionOpenFileDialog(object[] obj)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var instance = (BindableBase)obj[0];
            var propertyName = obj[1].ToString();
            var property = instance.GetType().GetProperty(propertyName);

            property.SetValue(instance, dialog.FileName);
        }
    }
}
