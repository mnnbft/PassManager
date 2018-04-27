using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;
    using Prism.Commands;
    using Prism.Mvvm;
    using Common;
    using Model;

    public class MainWindowVM : ViewModelBase
    {
        public enum PageNum : byte
        {
            Default_Page,
            PG_Page,
            Setting
        }

        public MainWindowVM()
        {
            PO_Page = new OpenFileVM();
            PG_Page = new PasswordGenVM();
            PI_Page = new PasswordInfoVM();
            Setting_Page = new SettingVM();
            Current_Page = PO_Page;
        }

        public int CurrentKey = 0;
        private string _OpenFileName;
        public string CurrentFilePath;
        public string CurrentKeyPath;

        public System.Security.SecureString CurrentFilePassword;

        public ObservableCollection<DataParam> PasswordItems { get; set; } = new ObservableCollection<DataParam>();

        private DataParam _PasswordSelectedItem;
        public DataParam PasswordSelectedItem
        {
            get { return _PasswordSelectedItem; }
            set
            {
                _PasswordSelectedItem = value;
                TabPage = null;
                IsInEditMode = false;

                SelectedPasswordString = string.Empty;
                if(PasswordSelectedItem != null)
                {
                    if (PasswordSelectedItem.Password.Length > 0)
                        SelectedPasswordString = new string('●', PasswordSelectedItem.Password.Length);
                }

                SetProperty(ref _PasswordSelectedItem, value);
                Current_Page = PI_Page;
            }
        }

        public bool OpenFileFlg
        {
            get { return !string.IsNullOrEmpty(OpenFileName); }
        }
        public string OpenFileName
        {
            get { return _OpenFileName; }
            set
            {
                _OpenFileName = value;
                SetProperty(ref _OpenFileName, value);
                OnPropertyChanged("OpenFileFlg");
            }
        }
        private string _SelectedPasswordString;
        public string SelectedPasswordString
        {
            get { return _SelectedPasswordString; }
            set { SetProperty(ref _SelectedPasswordString, value); }
        }
        private string BeforeName;
        private bool _IsInEditMode = false;
        public bool IsInEditMode
        {
            get { return _IsInEditMode; }
            set { SetProperty(ref _IsInEditMode, value); }
        }
        private static BindableBase _Current_Page;
        public BindableBase Current_Page
        {
            get { return _Current_Page; }
            set { SetProperty(ref _Current_Page, value); }
        }
        private OpenFileVM _PO_Page;
        public OpenFileVM PO_Page
        {
            get { return _PO_Page; }
            set { SetProperty(ref _PO_Page, value); }
        }
        private PasswordGenVM _PG_Page;
        public PasswordGenVM PG_Page
        {
            get { return _PG_Page; }
            set { SetProperty(ref _PG_Page, value); }
        }
        private PasswordInfoVM _PI_Page;
        public PasswordInfoVM PI_Page
        {
            get { return _PI_Page; }
            set { SetProperty(ref _PI_Page, value); }
        }
        private SettingVM _Setting_Page;
        public SettingVM Setting_Page
        {
            get { return _Setting_Page; }
            set { SetProperty(ref _Setting_Page, value); }
        }

        private byte? _TabPage;
        public byte? TabPage
        {
            get { return _TabPage; }
            set
            {
                SetProperty(ref _TabPage, value);

                switch (_TabPage)
                {
                    case (byte)PageNum.Default_Page:
                        Current_Page = PO_Page;
                        break;
                    case (byte)PageNum.PG_Page:
                        Current_Page = PG_Page;
                        break;
                    case (byte)PageNum.Setting:
                        Current_Page = Setting_Page;
                        break;
                }
            }
        }

        public DelegateCommand CommandLoaded
        {
            get
            {
                return new DelegateCommand(
                    delegate
                    {
                        TabPage = (byte)PageNum.Default_Page;
                    });
            }
        }

        public DelegateCommand GenCanncelCommand
        {
            get
            {
                return new DelegateCommand(
                    delegate
                    {
                        Current_Page = null;
                        TabPage = (byte)PageNum.Default_Page;
                    });
            }
        }

        public DelegateCommand<object> CommandSelectedItemChanged
        {
            get
            {
                return new DelegateCommand<object>(
                    delegate (object obj)
                    {
                        PasswordSelectedItem = (DataParam)obj;
                    });
            }
        }
        public DelegateCommand CommandItemCreate
        {
            get
            {
                return new DelegateCommand(
                    delegate
                    {
                        var AddItem = new DataParam();
                        AddItem.Title = "新しいフォルダー";
                        AddItem.Key = CurrentKey++;

                        if (PasswordSelectedItem != null)
                        {
                            AddItem.ParentKey = PasswordSelectedItem.Key;
                            var ReCreatedItems = MainWindowM.ReCreateItems(PasswordSelectedItem, PasswordItems.ToList(), AddItem);

                            PasswordItems.Clear();
                            foreach (var item in ReCreatedItems)
                                PasswordItems.Add(item);
                        }
                        else
                        {
                            PasswordItems.Add(AddItem);
                            AddItem.ParentKey = null;
                        }
                    },
                    delegate
                    {
                        if (!OpenFileFlg) return false;
                        return true;
                    });
            }
        }
        public DelegateCommand CommandRename
        {
            get
            {
                return new DelegateCommand(
                    delegate
                    {
                        IsInEditMode = true;
                        if (PasswordSelectedItem != null)
                            BeforeName = PasswordSelectedItem.Title;
                    },
                    delegate
                    {
                        if (!OpenFileFlg) return false;
                        return true;
                    });
            }
        }
        public DelegateCommand CommandEditKeyDown
        {
            get
            {
                return new DelegateCommand(
                    delegate
                    {
                        if (IsInEditMode)
                            IsInEditMode = false;
                    });
            }
        }
        public DelegateCommand CommandItemDelete
        {
            get
            {
                return new DelegateCommand(
                    delegate
                    {
                        PasswordItems.Remove(PasswordSelectedItem);
                    },
                    delegate
                    {
                        if (!OpenFileFlg) return false;
                        return true;
                    });
            }
        }
        public DelegateCommand CommandFileClose
        {
            get
            {
                return new DelegateCommand(
                    delegate
                    {
                        OpenFileName = null;
                        PasswordItems.Clear();
                        PasswordSelectedItem = null;
                        TabPage = (int)PageNum.Default_Page;
                    });
            }
        }
        public DelegateCommand CommandFileSave
        {
            get
            {
                return new DelegateCommand(
                    delegate
                    {
                        Model.PasswordGenM.FileEncrypt(CurrentFilePath, CurrentKeyPath,
                            CurrentFilePassword, PasswordItems.Select(i => Common.Copy(i, new DataParam())).ToArray());
                    });
            }
        }
    }
}
