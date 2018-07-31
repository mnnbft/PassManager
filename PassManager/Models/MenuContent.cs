using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace PassManager.Models
{
    public sealed class MenuContent : BindableBase
    {
        public static MenuContent Instance { get; }
            = new MenuContent();
        private MenuContent() { }

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
    }
}
