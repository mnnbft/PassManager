using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PassManager.Models
{
    public enum MenuButtonItem
    {
        View,
        Setting,
    }

    public enum ContextMenuItem
    {
        New,
        Modify,
        Delete,
    }

    public sealed class MenuContent : BindableBase
    {
        public static MenuContent Instance { get; } = new MenuContent();

        private MenuContent() { }

        public Dictionary<MenuButtonItem, string> menuButtonDictionary =
            new Dictionary<MenuButtonItem, string>()
            {
                { MenuButtonItem.View, "Password" },
                { MenuButtonItem.Setting, "Setting" },
            };

        public Dictionary<ContextMenuItem, string> contextMenuDictionary =
            new Dictionary<ContextMenuItem, string>()
            {
                { ContextMenuItem.New, "新規作成" },
                { ContextMenuItem.Modify, "修正" },
                { ContextMenuItem.Delete, "削除" },
            };

        public IEnumerable<string> MenuButtonItems
        {
            get { return menuButtonDictionary.Select(i => i.Value); }
        }
        public IEnumerable<string> ContextMenuItems
        {
            get { return contextMenuDictionary.Select(i => i.Value); }
        }
        private string snackMessage;
        public string SnackMessage
        {
            get { return snackMessage; }
            set { SetProperty(ref snackMessage, value); }
        }
        private bool isSnackbarActive;
        public bool IsSnackbarActive
        {
            get { return isSnackbarActive; }
            set
            {
                SetProperty(ref isSnackbarActive, value);
                if (IsSnackbarActive)
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(3000);
                        SetProperty(ref isSnackbarActive, false);
                    });
                }
            }
        }
    }
}
