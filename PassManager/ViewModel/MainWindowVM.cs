using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;
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

        public static int CurrentKey = 0;
        private string _OpenFileName;
        public string OpenFileName
        {
            get { return _OpenFileName; }
            set
            {
                _OpenFileName = value;
                OnPropertyChanged("OpenFileName");
                OnPropertyChanged("OpenFileFlg");
            }
        }
        public bool OpenFileFlg
        {
            get { return !string.IsNullOrEmpty(OpenFileName); }
        }
        public ObservableCollection<TreeViewParam> _PasswordItems = new ObservableCollection<TreeViewParam>();
        public ObservableCollection<TreeViewParam> PasswordItems
        {
            get { return _PasswordItems; }
            set { _PasswordItems = value; }
        }
        private TreeViewParam _PasswordSelectedItem;
        public TreeViewParam PasswordSelectedItem
        {
            get { return _PasswordSelectedItem; }
            set
            {
                _PasswordSelectedItem = value;
                IsInEditMode = false;

                var item = PasswordSelectedItem as DataParam;
                if (item != null)
                {
                    SelectedPasswordString = new string('●', item.Password.Length);
                    OnPropertyChanged("PasswordSelectedItem");
                }
                Current_Page = PI_Page;
            }
        }
        private string _SelectedPasswordString;
        public string SelectedPasswordString
        {
            get { return _SelectedPasswordString; }
            set
            {
                _SelectedPasswordString = value;
                OnPropertyChanged("SelectedPasswordString");
            }
        }
        private string BeforeName;
        private bool _IsInEditMode = false;
        public bool IsInEditMode
        {
            get { return _IsInEditMode; }
            set
            {
                _IsInEditMode = value;
                OnPropertyChanged("IsInEditMode");
            }
        }

        private static ViewModelBase _Current_Page;
        public ViewModelBase Current_Page
        {
            get { return _Current_Page; }
            set
            {
                if (_Current_Page != value)
                {
                    _Current_Page = value;
                    OnPropertyChanged("Current_Page");
                }
            }
        }

        private OpenFileVM _PO_Page;
        public OpenFileVM PO_Page
        {
            get { return _PO_Page; }
            set
            {
                _PO_Page = value;
                OnPropertyChanged("PO_Page");
            }
        }
        private PasswordGenVM _PG_Page;
        public PasswordGenVM PG_Page
        {
            get { return _PG_Page; }
            set
            {
                _PG_Page = value;
                OnPropertyChanged("PG_Page");
            }
        }
        private PasswordInfoVM _PI_Page;
        public PasswordInfoVM PI_Page
        {
            get { return _PI_Page; }
            set
            {
                _PI_Page = value;
                OnPropertyChanged("PI_Page");
            }
        }
        private SettingVM _Setting_Page;
        public SettingVM Setting_Page
        {
            get { return _Setting_Page; }
            set
            {
                if (_Setting_Page != value)
                {
                    _Setting_Page = value;
                    OnPropertyChanged("Setting_Page");
                }
            }
        }
        private byte _TabPage;
        public byte TabPage
        {
            get { return _TabPage; }
            set
            {
                _TabPage = value;
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
                OnPropertyChanged("TabPage");
            }
        }

        public DelegateCommand CommandInit
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

        public DelegateCommand CommandSelectedItemChanged
        {
            get
            {
                return new DelegateCommand(
                    delegate (object obj)
                    {
                        PasswordSelectedItem = (TreeViewParam)obj;
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
                        var AddItem = new TreeViewParam();
                        AddItem.Title = "新しいフォルダー";
                        AddItem.Key = CurrentKey++;

                        if (PasswordSelectedItem != null)
                        {
                            AddItem.ParentKey = PasswordSelectedItem.Key;
                            var index = PasswordItems.IndexOf(PasswordSelectedItem);

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
    }
}
