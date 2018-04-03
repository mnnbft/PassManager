﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewModel
{
    using System.Windows.Forms;
    using Common;
    using Model;

    public class OpenFileVM : ViewModelBase
    {
        public string _OpenFilePath { get; set; }
        public string OpenFilePath
        {
            get { return _OpenFilePath; }
            set
            {
                _OpenFilePath = value;
                OnPropertyChanged("OpenFilePath");
            }
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
                        {
                            OpenFilePath = fbd.FileName;
                        }
                    });
            }
        }
        public string _OpenKeyPath { get; set; }
        public string OpenKeyPath
        {
            get { return _OpenKeyPath; }
            set
            {
                _OpenKeyPath = value;
                OnPropertyChanged("OpenKeyPath");
            }
        }
        public System.Security.SecureString Password { get; set; } = new System.Security.SecureString();

        public DelegateCommand OpenKeyDialog
        {
            get
            {
                return new DelegateCommand(
                    delegate
                    {
                        var fbd = new OpenFileDialog();
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            OpenKeyPath = fbd.FileName;
                        }
                    });
            }
        }
        public DelegateCommand CommandOK
        {
            get
            {
                return new DelegateCommand(
                    delegate (object obj)
                    {
                        var MainWindow = System.Windows.Window.GetWindow((System.Windows.Controls.UserControl)obj);
                        var WindowDC = (MainWindowVM)MainWindow.DataContext;

                        var CreatedItems = PasswordGenM.FileDecrypt(OpenFilePath, OpenKeyPath, Password);
                        if (CreatedItems == null)
                            return;
                        else
                            WindowDC.OpenFileName = System.IO.Path.GetFileName(OpenFilePath);

                        var CreatedItems2 = OpenFileM.OpenCreateItems(CreatedItems);

                        WindowDC.PasswordItems.Clear();
                        foreach (var item in CreatedItems2)
                            WindowDC.PasswordItems.Add(item);
                    });
            }
        }
    }
}
