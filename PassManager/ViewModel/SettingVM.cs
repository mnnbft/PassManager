using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewModel
{
    using Common;
    using System.Windows.Forms;

    public class SettingVM : ViewModelBase
    {
        private string _SaveFolderPath;
        public string SaveFolderPath
        {
            get { return _SaveFolderPath; }
            set
            {
                if (_SaveFolderPath != value)
                {
                    _SaveFolderPath = value;
                    OnPropertyChanged("SaveFolder");
                }
            }
        }

        public DelegateCommand CommandSave
        {
            get
            {
                return new DelegateCommand(
                    delegate
                    {
                        var fbd = new FolderBrowserDialog();
                        fbd.Description = "保存先フォルダを選択してください。";
                        if(fbd.ShowDialog() == DialogResult.OK)
                        {
                            SaveFolderPath = fbd.SelectedPath;
                        }
                    });
            }
        }
    }
}
