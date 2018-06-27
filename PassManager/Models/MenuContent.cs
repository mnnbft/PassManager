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
            Generation,
            Setting,
        }
        public Dictionary<MenuButtonItem, string> menuButtonDictionary =
            new Dictionary<MenuButtonItem, string>()
            {
                { MenuButtonItem.View, "表示画面" },
                { MenuButtonItem.Generation, "パスワード作成" },
                { MenuButtonItem.Setting, "設定" },
            };
        public string[] MenuButtonItems
        {
            get { return menuButtonDictionary.Select(i => i.Value).ToArray(); }
        }
    }
}
