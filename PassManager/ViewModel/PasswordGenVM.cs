using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewModel
{
    using Common;
    using Model;
    using System.Security;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Collections.ObjectModel;

    public class PasswordGenVM : ViewModelBase
    {
        private PasswordGenM model = new PasswordGenM();
        public MainWindowVM mainWindow { get; set; }

        public string PassLength { get; set; }
        public string SaveFolderPath { get; set; }
        public bool DecimalFlg { get; set; }
        public bool a_AlpFlg { get; set; }
        public bool A_AlpFlg { get; set; }
        public bool SymbolFlg { get; set; }

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        private string _UserName;
        public string UserName
        {
            get { return _UserName; }
            set
            {
                if (_UserName != value)
                {
                    _UserName = value;
                    OnPropertyChanged("UserName");
                }
            }
        }

        private bool _PasswordViewFlg = false;
        public bool PasswordViewFlg
        {
            get { return _PasswordViewFlg; }
            set
            {
                _PasswordViewFlg = value;
                var bstr = Marshal.SecureStringToBSTR(SecurePassword);
                PasswordString = PasswordViewFlg ?
                                 Marshal.PtrToStringUni(bstr) :
                                 new string('●', SecurePassword.Length);
                Marshal.ZeroFreeBSTR(bstr);
            }
        }
        private string _PasswordString;
        public string PasswordString
        {
            get { return _PasswordString; }
            set
            {
                _PasswordString = value;
                OnPropertyChanged("PasswordString");
            }
        }
        private SecureString _SecurePassword = new SecureString();
        public SecureString SecurePassword
        {
            get { return _SecurePassword; }
            set
            {
                _SecurePassword = value;
                var bstr = Marshal.SecureStringToBSTR(SecurePassword);
                PasswordString = PasswordViewFlg ?
                                 Marshal.PtrToStringUni(bstr) :
                                 new string('●', SecurePassword.Length);
                Marshal.ZeroFreeBSTR(bstr);
            }
        }

        public ObservableCollection<string> Memos { get; set; } = new ObservableCollection<string>() { "test" };
        public int MemosCnt { get { return Memos.Count; } }

        public DelegateCommand CommandGenPassword
        {
            get
            {
                return new DelegateCommand(
                    delegate
                    {
                        if (PassLength == null)
                        {
                            PassLength = "0";
                        }
                        SecurePassword = model.GeneratePassword(int.Parse(PassLength), new List<bool>()
                        {
                            DecimalFlg,
                            a_AlpFlg,
                            A_AlpFlg,
                            SymbolFlg
                        }.ToArray());
                    });
            }
        }

        public DelegateCommand CommandMemoAdd
        {
            get
            {
                return new DelegateCommand(
                    delegate
                    {
                        Memos.Add(null);
                        OnPropertyChanged("MemosCnt");
                    });
            }
        }
        public DelegateCommand CommandTextChanged
        {
            get
            {
                return new DelegateCommand(
                    delegate (object obj)
                    {
                        var obj2 = obj;
                    });
            }
        }
        public DelegateCommand CommandCreate
        {
            get
            {
                return new DelegateCommand(
                    delegate (object obj)
                    {
                        var AddItem = new DataParam()
                        {
                            Title = this.Title,
                            UserName = this.UserName,
                            Password = this.SecurePassword,
                            Memos = this.Memos
                        };

                        if(obj != null)
                        {
                            var window = Window.GetWindow((UserControl)obj);
                            var dc = (MainWindowVM)window.DataContext;

                            AddItem.Key = dc.CurrentKey++;

                            AddItem.ParentKey = dc.PasswordSelectedItem.Key;
                            var ReCreatedItems = MainWindowM.ReCreateItems(dc.PasswordSelectedItem, dc.PasswordItems.ToList(), AddItem);

                            dc.PasswordItems.Clear();
                            foreach (var item in ReCreatedItems)
                                dc.PasswordItems.Add(item);
                        }
                    });
            }
        }
    }
}
