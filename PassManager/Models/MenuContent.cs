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

        public Dictionary<MenuButtonItem, string> menuButtonDictionary =
            new Dictionary<MenuButtonItem, string>()
            {
                { MenuButtonItem.View, "Password" },
                { MenuButtonItem.Setting, "Setting" },
            };

        public string[] MenuButtonItems
        {
            get { return menuButtonDictionary.Select(i => i.Value).ToArray(); }
        }
    }
}
