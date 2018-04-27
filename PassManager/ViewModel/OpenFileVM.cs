using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewModel
{
    using System.Windows.Forms;
    using Prism.Commands;
    using Common;
    using Model;

    public class OpenFileVM : ViewModelBase
    {
        public System.Security.SecureString Password { get; set; } = new System.Security.SecureString();

        private bool _NewFileFlg = false;
        public bool NewFileFlg
        {
            get { return _NewFileFlg; }
            set { SetProperty(ref _NewFileFlg, value); }
        }
        private string _OpenFilePath;
        public string OpenFilePath
        {
            get { return _OpenFilePath; }
            set { SetProperty(ref _OpenFilePath, value); }
        }
        private string _OpenKeyPath;
        public string OpenKeyPath
        {
            get { return _OpenKeyPath; }
            set { SetProperty(ref _OpenKeyPath, value); }
        }

        public DelegateCommand OpenFileDialog
        {
            get
            {
                return new DelegateCommand(
                    delegate
                    {
                        var fbd = new OpenFileDialog();
                        if (fbd.ShowDialog() == DialogResult.OK)
                            OpenFilePath = fbd.FileName;
                    });
            }
        }
        public DelegateCommand OpenKeyDialog
        {
            get
            {
                return new DelegateCommand(
                    delegate
                    {
                        var fbd = new OpenFileDialog();
                        if (fbd.ShowDialog() == DialogResult.OK)
                            OpenKeyPath = fbd.FileName;
                    });
            }
        }
        public DelegateCommand<object> CommandOK
        {
            get
            {
                return new DelegateCommand<object>(
                    delegate (object obj)
                    {
                        var MainWindow = System.Windows.Window.GetWindow((System.Windows.Controls.UserControl)obj);
                        var WindowDC = (MainWindowVM)MainWindow.DataContext;

                        if (NewFileFlg)
                        {
                            var RootItem = new DataParam()
                            {
                                Title = OpenFilePath,
                                Key = -1,
                                ParentKey = null
                            };
                            WindowDC.PasswordItems.Clear();
                            WindowDC.PasswordItems.Add(RootItem);
                            
                            WindowDC.OpenFileName = System.IO.Path.GetFileName(OpenFilePath);
                        }
                        else
                        {
                            var CreatedItems = PasswordGenM.FileDecrypt(OpenFilePath, OpenKeyPath, Password);
                            if (CreatedItems != null)
                            {
                                var CreatedItems2 = OpenFileM.OpenCreateItems(CreatedItems);
                                WindowDC.PasswordItems.Clear();

                                foreach (var item in CreatedItems2)
                                    WindowDC.PasswordItems.Add(item);

                                WindowDC.OpenFileName = System.IO.Path.GetFileName(OpenFilePath);
                            }
                            else
                                return;
                        }
                        WindowDC.CurrentFilePath = OpenFilePath;
                        WindowDC.CurrentKeyPath = OpenKeyPath;
                        WindowDC.CurrentFilePassword = Password;
                    });
            }
        }
    }
}
